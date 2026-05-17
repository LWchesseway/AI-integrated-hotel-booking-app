import json
import logging
import re
import sqlite3
from datetime import datetime, timezone
from typing import Dict, Any, Set
from langchain_groq import ChatGroq
from langchain.agents import create_agent, AgentState
from langchain.agents.middleware import wrap_tool_call, wrap_model_call
from langchain.messages import HumanMessage, ToolMessage, AIMessage, SystemMessage
from dotenv import load_dotenv
from langgraph.checkpoint.sqlite import SqliteSaver
load_dotenv()

class CustomState(AgentState):
    user_preferences: Dict[str, Any] = {}
    summary: str = ""
    hotel_context: Dict[str, Any] = {}


logging.basicConfig(
    level=logging.INFO,
    format="%(asctime)s %(levelname)s %(name)s - %(message)s",
)
logger = logging.getLogger("travelbuddy.agent")
logger.setLevel(logging.INFO)
if not any(
    isinstance(handler, logging.FileHandler)
    and handler.baseFilename.endswith("tools_call.log")
    for handler in logger.handlers
):
    tools_call_file_handler = logging.FileHandler("tools_call.log", encoding="utf-8")
    tools_call_file_handler.setFormatter(
        logging.Formatter("%(asctime)s %(levelname)s %(name)s - %(message)s")
    )
    logger.addHandler(tools_call_file_handler)

HOTEL_TOOL_NAMES = {
    "search_hotels",
    "search_hotelsbyname",
    "search_hotelsbyprovince",
    "search_roomtypebyHotelID",
}

IMPORTANT_HOTEL_FIELDS = (
    "id",
    "name",
    "provinceName",
    "wardName",
    "street",
    "phone",
    "status",
)
MAX_CONTEXT_HOTELS = 3


def init_db():
    conn = sqlite3.connect("memory.db", check_same_thread=False)
    conn.execute("PRAGMA journal_mode=WAL;")
    conn.commit()
    return conn

conn = init_db()
checkpointer = SqliteSaver(conn)

def load_system_prompt():
    with open("Promt.txt", "r", encoding="utf-8") as f:
        return f.read()

SYSTEM_PROMPT = load_system_prompt()

from testapi import (
    search_hotels,
    search_hotelsbyname,
    search_hotelsbyprovince,
    search_roomtypebyHotelID
)

TOOLS = [
    search_hotels,
    search_hotelsbyname,
    search_hotelsbyprovince,
    search_roomtypebyHotelID
]

ALLOWED_TOOLS: Set[str] = {t.name for t in TOOLS}


def _safe_json(value: Any) -> str:
    try:
        return json.dumps(value, ensure_ascii=False, default=str)
    except TypeError:
        return str(value)


def _parse_formatted_items(content: str) -> list[dict[str, str]]:
    items = []
    for line in content.splitlines():
        item = {}
        for part in line.split(" | "):
            if ": " not in part:
                continue
            key, value = part.split(": ", 1)
            item[key.strip()] = value.strip()
        if item:
            items.append(item)
    return items


def _compact_hotel(item: dict[str, Any]) -> dict[str, Any]:
    hotel = {}
    for field in IMPORTANT_HOTEL_FIELDS:
        value = item.get(field)
        if value not in (None, ""):
            hotel[field] = value
    return hotel


def _find_hotel_by_id(hotels: list[dict[str, Any]], hotel_id: Any) -> dict[str, Any] | None:
    hotel_id_text = str(hotel_id)
    for hotel in hotels:
        if str(hotel.get("id")) == hotel_id_text:
            return hotel
    return None


def _update_hotel_context(
    state: dict[str, Any],
    tool_name: str,
    tool_args: dict[str, Any],
    tool_result: ToolMessage,
) -> None:
    if tool_name not in HOTEL_TOOL_NAMES:
        return

    context = dict(state.get("hotel_context") or {})
    previous_hotels = list(context.get("candidate_hotels") or [])
    parsed_hotels = [
        _compact_hotel(item)
        for item in _parse_formatted_items(tool_result.content or "")
    ]
    parsed_hotels = [hotel for hotel in parsed_hotels if hotel]

    if tool_name == "search_hotelsbyprovince":
        province = tool_args.get("province")
        if province:
            context["active_area"] = {"province": province}

    if tool_name in {"search_hotels", "search_hotelsbyname", "search_hotelsbyprovince"}:
        if parsed_hotels:
            context["candidate_hotels"] = parsed_hotels[:MAX_CONTEXT_HOTELS]
            if len(parsed_hotels) == 1:
                context["active_hotel"] = parsed_hotels[0]

    if tool_name == "search_roomtypebyHotelID":
        hotel_id = tool_args.get("hotelId")
        if hotel_id is not None:
            active_hotel = _find_hotel_by_id(previous_hotels, hotel_id) or {"id": hotel_id}
            context["active_hotel"] = active_hotel
            context["selected_hotel_id"] = hotel_id

    context["last_tool"] = {
        "name": tool_name,
        "args": tool_args,
    }
    context["updated_at"] = datetime.now(timezone.utc).isoformat()
    state["hotel_context"] = context


def _build_memory_message(state: dict[str, Any]) -> SystemMessage | None:
    summary = state.get("summary") or ""
    hotel_context = state.get("hotel_context") or {}

    if not summary and not hotel_context:
        return None

    content = [
        "Ngu canh quan trong da luu cho cuoc hoi thoai nay.",
        "Hay dung ngu canh nay khi khach hoi tiep bang cac cum nhu khach san do, cho nay, khu vuc do, phong o do.",
        "Khong hien thi ID khach san cho khach neu khong can thiet.",
    ]
    if summary:
        content.append(f"Tom tat hoi thoai: {summary}")
    if hotel_context:
        content.append(f"Ngu canh khach san/khu vuc: {_safe_json(hotel_context)}")

    return SystemMessage(content="\n".join(content))


def _call_model_with_memory(request, handler, messages, state):
    memory_message = _build_memory_message(state)
    if memory_message:
        request = request.override(messages=[memory_message, *messages])
    return handler(request)


@wrap_tool_call
def tool_guard(request, handler):
    tool_name = request.tool_call["name"]
    tool_args = request.tool_call.get("args", {})

    logger.info(
        "[tools_call] requested name=%s args=%s id=%s",
        tool_name,
        _safe_json(tool_args),
        request.tool_call.get("id"),
    )

    # Validate tool
    if tool_name not in ALLOWED_TOOLS:
        logger.warning("[tools_call] rejected invalid name=%s", tool_name)
        return ToolMessage(
            content=f"Tool '{tool_name}' không hợp lệ. Hãy chọn tool đúng.Chỉ dùng {ALLOWED_TOOLS}",
            tool_call_id=request.tool_call["id"]
        )

    # Handle runtime error
    try:
        result = handler(request)
        logger.info(
            "[tools_call] completed name=%s result_preview=%s",
            tool_name,
            re.sub(r"\s+", " ", (result.content or ""))[:500],
        )
        _update_hotel_context(request.state, tool_name, tool_args, result)
        logger.info(
            "[hotel_context] %s",
            _safe_json(request.state.get("hotel_context", {})),
        )
        return result
    except Exception as e:
        logger.exception("[tools_call] failed name=%s", tool_name)
        return ToolMessage(
            content=f"Tool error: {str(e)}",
            tool_call_id=request.tool_call["id"]
        )

@wrap_model_call
def summarization_middleware(request, handler):
    messages = request.messages
    state = request.state
    human_messages = [m for m in messages if isinstance(m, HumanMessage)]
    count_human = len(human_messages)

    if count_human == 0 or count_human % 5 != 0:
        return _call_model_with_memory(request, handler, messages, state)

    if not isinstance(messages[-1], AIMessage):
        return _call_model_with_memory(request, handler, messages, state)

    print(f"\n--- ĐANG TÓM TẮT ({count_human}) ---")

    llm = request.model
    old_summary = state.get("summary", "")

    recent_context = messages[-10:]

    prompt = f"""
    Tóm tắt trước đó:
    {old_summary}

    Hội thoại gần nhất:
    {recent_context}

    Hãy tạo bản tóm tắt ngắn gọn, giữ thông tin quan trọng.
    """

    try:
        summary = llm.invoke(prompt).content
        state["summary"] = summary
        request = request.override(messages=messages[-2:])

    except Exception as e:
        print("Summarize error:", e)

    return _call_model_with_memory(request, handler, request.messages, state)

llm = ChatGroq(
    model="openai/gpt-oss-120b",
    temperature=0
)

agent = create_agent(
    model=llm,
    tools=TOOLS,
    system_prompt=SYSTEM_PROMPT,
    middleware=[
        tool_guard,
        summarization_middleware
    ],
    checkpointer=checkpointer,
    state_schema=CustomState
)

def chat(thread_id: str, message: str):
    result = agent.invoke(
        {
            "messages": [HumanMessage(content=message)]
        },
        config={
            "configurable": {
                "thread_id": thread_id
            }
        }
    )
    ai_reply = result["messages"][-1].content

    return ai_reply

if __name__ == "__main__":
    print("=== Local Chat Test ===")

    thread_id = input("thread ID: ").strip()

    while True:
        msg = input("User: ")

        if msg.lower() in ["exit", "quit"]:
            break

        ai_reply = chat(thread_id, msg)
        print("AI:", ai_reply)
        
        
