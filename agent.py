from typing import Annotated
from typing_extensions import TypedDict
from langgraph.graph import StateGraph, START, END
from langgraph.graph.message import add_messages
from langgraph.prebuilt import ToolNode, tools_condition
from langchain_groq import ChatGroq
from langchain_core.messages import SystemMessage
from tools import search_flight, search_hotel, calculate_budget
from dotenv import load_dotenv

load_dotenv()

with open ("systemprompt.txt", "r", encoding="utf-8") as f:
    SYSTEMPROMPT = f.read()
class AgentState(TypedDict):
    messages : Annotated[list, add_messages]
tools_list = [search_flight, search_hotel, calculate_budget]
llm = ChatGroq(model="openai/gpt-oss-20b")
llm_with_tools = llm.bind_tools(tools_list)
# 4. Agent Node
def agent_node(state: AgentState):
    messages = state["messages"]
    if not isinstance(messages[0], SystemMessage):
        messages = [SystemMessage(content=SYSTEMPROMPT)] + messages
    
    response = llm_with_tools.invoke(messages)
 
    # === LOGGING ===
    if response.tool_calls:
        for tc in response.tool_calls:
            print(f"Gọi tool: {tc['name']}({tc['args']})")
    else:
        print(f"Trả lời trực tiếp")
        
    return {"messages": [response]}

# 5. Xây dựng Graph
builder = StateGraph(AgentState)
builder.add_node("agent", agent_node)

tool_node = ToolNode(tools_list)
builder.add_node("tools", tool_node)

# TODO: Sinh viên khai báo edges

# Bắt đầu từ START -> agent
builder.add_edge(START, "agent")

# Agent quyết định:
# - Nếu cần tool -> đi tới tools
# - Nếu không -> kết thúc
builder.add_conditional_edges(
    "agent",
    tools_condition
)

# Sau khi tool chạy xong -> quay lại agent
builder.add_edge("tools", "agent")

graph = builder.compile()

# 6. Chat loop
if __name__ == "__main__":
    print("=" * 60)
    print("TravelBuddy — Trợ lý Du lịch Thông minh")
    print(" Gõ 'quit' để thoát")
    print("=" * 60)

    messages = []

    while True:
        user_input = input("\nBạn: ").strip()
        if user_input.lower() in ("quit", "exit", "q"):
            break

        # Lưu message user
        messages.append(("human", user_input))

        print("\nTravelBuddy đang suy nghĩ...")

        result = graph.invoke({
            "messages": messages
        })

        final = result["messages"][-1]

        print(f"\nTravelBuddy: {final.content}")

        # Lưu response của AI
        messages.append(final)


