import os
import sqlite3
from typing import Annotated, TypedDict, Literal
from dotenv import load_dotenv
load_dotenv()
from langgraph.graph import StateGraph, START, END
from langgraph.graph.message import add_messages
from langgraph.prebuilt import ToolNode, tools_condition
from langgraph.checkpoint.sqlite import SqliteSaver
from langchain_groq import ChatGroq
from langchain_core.messages import SystemMessage, HumanMessage, AIMessage, RemoveMessage, ToolMessage

# Import tools của bạn
from testapi import search_hotels, search_hotelsbyname, search_hotelsbyprovince, search_roomtypebyHotelID


# --- CẤU HÌNH SQLITE ---
DB_PATH = "state.db"
conn = sqlite3.connect(DB_PATH, check_same_thread=False)
#conn.execute("PRAGMA journal_mode=WAL;")
#conn.execute("PRAGMA synchronous=NORMAL;")
memory = SqliteSaver(conn)

# --- CẤU TRÚC AGENT ---
class AgentState(TypedDict):
    messages: Annotated[list, add_messages]
    summary: str
    retry_count: int  # Thêm biến đếm số lần thử lại

llm = ChatGroq(model="llama-3.3-70b-versatile", temperature=0.5) 
tools_list = [search_hotels, search_hotelsbyname, search_hotelsbyprovince, search_roomtypebyHotelID]
llm_with_tools = llm.bind_tools(tools_list)

with open("Promt.txt", "r", encoding="utf-8") as f:
    SYSTEMPROMPT = f.read()

def agent_node(state: AgentState):
    messages = state["messages"]
    summary = state.get("summary", "")
    retry_count = state.get("retry_count", 0)
    
    filtered_messages = [m for m in messages if isinstance(m, (HumanMessage, AIMessage, SystemMessage, ToolMessage))]

    current_prompt = SYSTEMPROMPT
    if summary:
        current_prompt += f"\n\nNội dung quan trọng từ quá khứ (đã tóm tắt): {summary}"
    
    # Nếu đang trong quá trình sửa lỗi, nhắc nhở AI chặt chẽ hơn
    if retry_count > 0:
        current_prompt += f"\n\nLƯU Ý: Bạn đã gọi sai định dạng công cụ ở lượt trước. Hãy chỉ sử dụng {tools_list} với đúng tham số JSON. Nếu không cần công cụ."

    input_messages = [SystemMessage(content=current_prompt)] + filtered_messages
    
    try:
        response = llm_with_tools.invoke(input_messages)
        return {"messages": [response], "retry_count": retry_count}
    except Exception as e:
        # Nếu Groq trả về lỗi 400 (do bịa tool sai định dạng)
        print(f"--- PHÁT HIỆN LỖI API (CỐ GẮNG SỬA): {e} ---")
        return {
            "messages": [HumanMessage(content="Lỗi hệ thống: Định dạng gọi công cụ của bạn không hợp lệ. Vui lòng thử lại hoặc trả lời bằng văn bản thuần túy.")],
            "retry_count": retry_count + 1
        }

def validator_node(state: AgentState):
    """Kiểm tra xem Tool AI gọi có tồn tại không"""
    messages = state["messages"]
    last_message = messages[-1]
    retry_count = state.get("retry_count", 0)

    if not hasattr(last_message, "tool_calls") or not last_message.tool_calls:
        return {"retry_count": 0} # Reset nếu AI trả lời bình thường

    valid_tool_names = [t.name for t in tools_list]
    for tool_call in last_message.tool_calls:
        if tool_call["name"] not in valid_tool_names:
            print(f"--- AI BỊA TOOL: {tool_call['name']} ---")
            return {
                "messages": [HumanMessage(content=f"Lỗi: Công cụ '{tool_call['name']}' không tồn tại. Chỉ sử dụng: {valid_tool_names}")],
                "retry_count": retry_count + 1
            }
    return {"retry_count": 0}

def critique_node(state: AgentState):
    messages = state["messages"]
    last_message = messages[-1]
    
    if isinstance(last_message, ToolMessage):
        if not last_message.content or "error" in last_message.content.lower():
             return {"messages": [HumanMessage(content="Kết quả tìm kiếm trống. Hãy thử tham số khác hoặc thông báo cho tôi.")]}
    
    return {"messages": []}

def summarize_node(state: AgentState):
    messages = state["messages"]
    summary = state.get("summary", "")
    human_messages = [m for m in messages if isinstance(m, HumanMessage)]
    count_human = len(human_messages)

    if count_human > 0 and count_human % 10 == 0:
        if isinstance(messages[-1], AIMessage):
            print(f"\n--- ĐÃ ĐẠT {count_human} CÂU HỎI: ĐANG TỔNG HỢP BỘ NHỚ ---")
            recent_context = messages[-15:] 
            prompt = (f"Tóm tắt cũ: {summary}\nCập nhật dựa trên: {recent_context}. Chỉ giữ ý chính.")
            try:
                response = llm.invoke(prompt)
                delete_messages = [RemoveMessage(id=m.id) for m in messages[:-2]]
                return {"summary": response.content, "messages": delete_messages}
            except: pass
    return {"messages": []}

# --- LOGIC ĐIỀU HƯỚNG ---
def should_continue(state: AgentState) -> Literal["tools", "agent", "summarize"]:
    messages = state["messages"]
    last_message = messages[-1]
    retry_count = state.get("retry_count", 0)

    # Nếu thử lại quá 3 lần mà vẫn lỗi, bắt buộc kết thúc để tránh treo
    if retry_count > 3:
        print("--- QUÁ GIỚI HẠN THỬ LẠI - CHUYỂN HƯỚNG KẾT THÚC ---")
        return "summarize"

    # Nếu tin nhắn cuối là HumanMessage (do Validator hoặc Agent_node tạo ra để nhắc lỗi)
    if isinstance(last_message, HumanMessage) and "Lỗi" in last_message.content:
        return "agent"

    if hasattr(last_message, "tool_calls") and last_message.tool_calls:
        return "tools"
    
    return "summarize"

# --- XÂY DỰNG GRAPH ---
builder = StateGraph(AgentState)
builder.add_node("agent", agent_node)
builder.add_node("validator", validator_node) # Node bảo vệ
builder.add_node("tools", ToolNode(tools_list))
builder.add_node("critique", critique_node)
builder.add_node("summarize", summarize_node)

builder.add_edge(START, "agent")

# Sau khi Agent chạy -> Qua validator để kiểm tra tool
builder.add_edge("agent", "validator")

# Từ validator -> Quyết định đi tiếp hay quay lại sửa
builder.add_conditional_edges("validator", should_continue)

builder.add_edge("tools", "critique")
builder.add_edge("critique", "agent")
builder.add_edge("summarize", END)

graph = builder.compile(checkpointer=memory)

# --- CHƯƠNG TRÌNH CHÍNH ---
if __name__ == "__main__":
    print("=" * 60)
    print("TravelBuddy — Bảo mật Tool & Tự sửa lỗi 400")
    print("=" * 60)

    user_id = input("Nhập ID của bạn: ").strip()
    config = {"configurable": {"thread_id": user_id}}

    while True:
        user_input = input("\nBạn: ").strip()
        if user_input.lower() in ("quit", "exit", "q"): break

        print("\nTravelBuddy đang suy nghĩ...")
        
        final_state = None
        for event in graph.stream(
            {"messages": [HumanMessage(content=user_input)], "retry_count": 0}, 
            config, 
            stream_mode="values"
        ):
            final_state = event

        messages = final_state.get("messages", [])
        if messages:
            ai_msgs = [m for m in messages if isinstance(m, AIMessage)]
            if ai_msgs:
                print(f"\nTravelBuddy: {ai_msgs[-1].content}")

        h_count = len([m for m in messages if isinstance(m, HumanMessage)])
        print(f" (Thread: {user_id} | Câu hỏi: {h_count})")