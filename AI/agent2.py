import sqlite3
import json
import logging
from typing import Dict, Any, Set

from pydantic import Field
from dotenv import load_dotenv

from langchain_groq import ChatGroq

from langchain.agents import (
    create_agent,
    AgentState
)

from langchain.agents.middleware import (
    wrap_tool_call
)

from langchain.messages import (
    AIMessage,
    HumanMessage,
    ToolMessage,
    SystemMessage,
    RemoveMessage
)

from langgraph.checkpoint.sqlite import SqliteSaver

load_dotenv()


logging.basicConfig(
    filename="tools_call.log",
    level=logging.INFO,
    format="%(asctime)s | %(levelname)s | %(message)s",
    encoding="utf-8"
)
class CustomState(AgentState):

    # Summary hội thoại
    summary: str = ""

    # User preferences
    user_preferences: Dict[str, Any] = Field(
        default_factory=dict
    )

    # Business state
    travel_context: Dict[str, Any] = Field(
        default_factory=dict
    )

def init_db():

    conn = sqlite3.connect(
        "memory.db",
        check_same_thread=False
    )

    conn.execute(
        "PRAGMA journal_mode=WAL;"
    )

    conn.commit()

    return conn

conn = init_db()

checkpointer = SqliteSaver(conn)


def load_system_prompt():

    with open(
        "Promt.txt",
        "r",
        encoding="utf-8"
    ) as f:

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

ALLOWED_TOOLS: Set[str] = {
    t.name for t in TOOLS
}


@wrap_tool_call
def tool_guard(request, handler):

    tool_name = request.tool_call["name"]



    if tool_name not in ALLOWED_TOOLS:

        return ToolMessage(
            content=f"Tool '{tool_name}' không hợp lệ.",
            tool_call_id=request.tool_call["id"]
        )


    try:

        logging.info(
            json.dumps(
                {
                    "tool_name": tool_name,
                    "args": request.tool_call.get(
                        "args",
                        {}
                    )
                },
                ensure_ascii=False
            )
        )

    except Exception as e:

        logging.error(
            f"LOG ERROR: {str(e)}"
        )


    try:

        return handler(request)

    except Exception as e:

        logging.error(
            f"TOOL ERROR | {tool_name} | {str(e)}"
        )

        return ToolMessage(
            content=f"Tool error: {str(e)}",
            tool_call_id=request.tool_call["id"]
        )


llm = ChatGroq(
    model="llama-3.3-70b-versatile",
    temperature=0
)


agent = create_agent(
    model=llm,
    tools=TOOLS,
    system_prompt=SYSTEM_PROMPT,
    middleware=[
        tool_guard
    ],
    checkpointer=checkpointer,
    state_schema=CustomState
)


def format_messages(
    messages,
    limit=12
):

    recent = messages[-limit:]

    lines = []

    for m in recent:

        role = m.__class__.__name__

        content = getattr(
            m,
            "content",
            ""
        )

        if isinstance(content, list):
            content = str(content)

        lines.append(
            f"{role}: {content}"
        )

    return "\n".join(lines)


def update_travel_context(
    thread_id: str
):
    snapshot = agent.get_state({
        "configurable": {
            "thread_id": thread_id
        }
    })

    state = snapshot.values

    current_context = state.get(
        "travel_context",
        {}
    )

    messages = state.get(
        "messages",
        []
    )

    conversation = format_messages(
        messages,
        limit=8
    )

    prompt = f"""
    Bạn là AI chuyên extract thông tin booking khách sạn.

    Hãy đọc hội thoại và cập nhật business state.

    Hội thoại:
    {conversation}

    Business state cũ:
    {json.dumps(current_context, ensure_ascii=False)}

    Trả về JSON hợp lệ:

    {{
        "destination": ""(Điểm đến mong muốn),
        "hotel_name": ""(Tên khách sạn mà bạn đã hoặc đang tư vấn),
        "province": ""(Tỉnh thành nơi khách sạn tọa lạc),
        "checkin_date": ""(Ngày check-in dự kiến),
        "checkout_date": ""(Ngày check-out dự kiến),
        "budget": ""(Ngân sách dự kiến),
        "room_type": ""(Loại phòng muốn đặt),
        "special_request": ""(Yêu cầu đặc biệt),
        "booking_intent": ""(Mức độ mong muốn đặt phòng, ví dụ: thấp, trung bình, cao),
        "selected_hotel": ""(Khách sạn đã được user chọn, nếu có),
        "selected_room": ""(Loại phòng đã được user chọn, nếu có)
    }}

    Rules:
    - Chỉ update field nếu có thông tin mới
    - Không tự đoán
    - Không tạo dữ liệu giả
    - Chỉ trả JSON
    """

    try:

        response = llm.invoke(
            prompt
        ).content

        # =================================================
        # CLEAN JSON
        # =================================================

        response = (
            response
            .replace("```json", "")
            .replace("```", "")
            .strip()
        )

        extracted = json.loads(
            response
        )

        # =================================================
        # MERGE CONTEXT
        # =================================================

        merged = {
            **current_context,
            **{
                k: v
                for k, v in extracted.items()
                if v not in ["", None]
            }
        }

        # =================================================
        # UPDATE STATE
        # =================================================

        agent.update_state(
            {
                "configurable": {
                    "thread_id": thread_id
                }
            },
            {
                "travel_context": merged
            }
        )

        print(
            "\n========== TRAVEL CONTEXT UPDATED =========="
        )

        print(
            json.dumps(
                merged,
                indent=2,
                ensure_ascii=False
            )
        )

        print(
            "============================================\n"
        )

    except Exception as e:

        print(
            "Travel context error:",
            e
        )


def summarize_conversation(
    thread_id: str,
    messages
):

    # =====================================================
    # LOAD CURRENT STATE
    # =====================================================

    snapshot = agent.get_state({
        "configurable": {
            "thread_id": thread_id
        }
    })

    state = snapshot.values

    old_summary = state.get(
        "summary",
        ""
    )

    context_text = format_messages(
        messages
    )

    prompt = f"""
    Bạn là AI chuyên tóm tắt hội thoại.

    Summary cũ:
    {old_summary}

    Hội thoại mới:
    {context_text}

    Hãy tạo summary ngắn gọn nhưng giữ:
    - nhu cầu user
    - địa điểm
    - khách sạn
    - ngân sách
    - lịch sử tư vấn
    - quyết định cuối cùng

    Chỉ trả summary.
    """

    try:

        summary = llm.invoke(
            prompt
        ).content

        # =================================================
        # FILTER SAFE MESSAGES
        # =================================================

        filtered_messages = []

        for m in messages:

            # skip tool message
            if isinstance(
                m,
                ToolMessage
            ):
                continue

            # skip ai tool call
            if (
                isinstance(m, AIMessage)
                and getattr(
                    m,
                    "tool_calls",
                    None
                )
            ):
                continue

            filtered_messages.append(m)

        # =================================================
        # KEEP RECENT 3 MESSAGES
        # =================================================

        delete_messages = [
            RemoveMessage(id=m.id)
            for m in filtered_messages[:-3]
        ]

        # =================================================
        # UPDATE STATE
        # =================================================

        agent.update_state(
            {
                "configurable": {
                    "thread_id": thread_id
                }
            },
            {
                "summary": summary,
                "messages": delete_messages
            }
        )

        print(
            "\n========== SUMMARY UPDATED =========="
        )

        print(summary)

        print(
            "=====================================\n"
        )

    except Exception as e:

        print(
            "Summarize error:",
            e
        )


def build_business_rules(
    travel_context: dict
):

    rules = []

    # =====================================================
    # EXAMPLE WORKFLOW LOGIC
    # =====================================================
    if travel_context.get(
        "destination"
    ):

        rules.append(
            """
            User đã có điểm đến mong muốn.
            Hãy ưu tiên gợi ý khách sạn tại điểm đến đó.
            """
        )

    if travel_context.get(
        "selected_hotel"
    ):

        rules.append(
            """
            User đã chọn khách sạn.
            Hãy tập trung tư vấn phòng phù hợp.
            """
        )

    if (
        travel_context.get("budget")
        and not travel_context.get(
            "hotel_name"
        )
    ):

        rules.append(
            """
            User đã có ngân sách.
            Hãy ưu tiên gợi ý khách sạn phù hợp ngân sách.
            """
        )

    if (
        travel_context.get(
            "hotel_name"
        )
        and not travel_context.get(
            "room_type"
        )
    ):

        rules.append(
            """
            User đã chọn khách sạn.
            Hãy hỗ trợ chọn loại phòng.
            """
        )

    if (
        travel_context.get(
            "room_type"
        )
        and not travel_context.get(
            "checkin_date"
        )
    ):

        rules.append(
            """
            Hãy hỏi ngày checkin/check-out.
            """
        )

    return "\n".join(rules)


def chat(
    thread_id: str,
    message: str
):


    snapshot = agent.get_state({
        "configurable": {
            "thread_id": thread_id
        }
    })

    state = snapshot.values

    summary = state.get(
        "summary",
        ""
    )

    travel_context = state.get(
        "travel_context",
        {}
    )


    business_rules = build_business_rules(
        travel_context
    )


    input_messages = []

    if summary:

        input_messages.append(
            SystemMessage(
                content=f"""
                Đây là summary hội thoại trước:

                {summary}

                Business rules:
                {business_rules}
                """
            )
        )

    input_messages.append(
        HumanMessage(content=message)
    )


    result = agent.invoke(
        {
            "messages": input_messages
        },
        config={
            "configurable": {
                "thread_id": thread_id
            }
        }
    )

    ai_reply = result[
        "messages"
    ][-1].content


    update_travel_context(
        thread_id
    )


    latest_snapshot = agent.get_state({
        "configurable": {
            "thread_id": thread_id
        }
    })

    latest_state = latest_snapshot.values

    messages = latest_state.get(
        "messages",
        []
    )


    human_messages = [
        m for m in messages
        if isinstance(
            m,
            HumanMessage
        )
    ]

    count_human = len(
        human_messages
    )

    print(
        f"\nCURRENT MESSAGES: {len(messages)}"
    )


    if (
        count_human > 0
        and count_human % 5 == 0
    ):

        summarize_conversation(
            thread_id,
            messages
        )


    final_snapshot = agent.get_state({
        "configurable": {
            "thread_id": thread_id
        }
    })

    final_state = final_snapshot.values

    print(
        "\n========== FINAL TRAVEL CONTEXT =========="
    )

    print(
        json.dumps(
            final_state.get(
                "travel_context",
                {}
            ),
            indent=2,
            ensure_ascii=False
        )
    )

    print(
        "==========================================\n"
    )

    return ai_reply


if __name__ == "__main__":

    print(
        "=== Local Chat Test ==="
    )

    thread_id = input(
        "thread ID: "
    ).strip()

    while True:

        msg = input("User: ")

        if msg.lower() in [
            "exit",
            "quit"
        ]:
            break

        ai_reply = chat(
            thread_id,
            msg
        )

        print(
            "AI:",
            ai_reply
        )