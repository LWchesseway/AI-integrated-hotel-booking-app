import streamlit as st
import os
from pathlib import Path
from dotenv import load_dotenv
from agent import graph
from langchain_core.messages import HumanMessage, AIMessage

load_dotenv()

# Set page config
st.set_page_config(
    page_title="TravelBuddy - AI Travel Assistant",
    page_icon="✈️",
    layout="wide",
    initial_sidebar_state="expanded"
)

# Custom CSS
st.markdown("""
<style>
    .main-header {
        color: #1f77b4;
        font-size: 2.5rem;
        font-weight: bold;
        margin-bottom: 10px;
    }
    .subtitle {
        color: #666;
        font-size: 1.1rem;
        margin-bottom: 20px;
    }
    .chat-message {
        padding: 12px 16px;
        border-radius: 8px;
        margin-bottom: 12px;
        display: flex;
        gap: 12px;
    }
    .user-message {
        background-color: #111;
        justify-content: flex-end;
    }
    .assistant-message {
        background-color: #111;
    }
    .message-content {
        max-width: 80%;
        word-wrap: break-word;
    }
</style>
""", unsafe_allow_html=True)

# Header
col1, col2 = st.columns([3, 1])
with col1:
    st.markdown('<div class="main-header">✈️ TravelBuddy</div>', unsafe_allow_html=True)
    st.markdown('<div class="subtitle">Trợ lý du lịch thông minh - Kế hoạch chuyến đi của bạn</div>', unsafe_allow_html=True)

# Sidebar
with st.sidebar:
    st.header("⚙️ Cài đặt")
    st.write("### ℹ️ Về Agent")
    st.info("""
    **TravelBuddy** là một AI Agent thông minh giúp:
    - 🔍 Tìm chuyến bay
    - 🏨 Tìm khách sạn
    - 💰 Tính toán ngân sách
    - 📋 Lên kế hoạch chuyến đi
    """)
    
    st.write("### 💡 Gợi ý")
    st.write("""
    Hãy thử hỏi:
    - "Tôi muốn đi Đà Nẵng với budget 5 triệu"
    - "Tìm chuyến bay Hà Nội → Phú Quốc"
    - "Đặt khách sạn ở Huế, 2 đêm"
    """)

# Initialize session state for chat history
if "messages" not in st.session_state:
    st.session_state.messages = [
        {
            "role": "assistant",
            "content": "👋 Xin chào! Tôi là TravelBuddy, trợ lý du lịch thông minh của bạn. Tôi có thể giúp bạn:\n\n✅ Tìm chuyến bay\n✅ Tìm khách sạn\n✅ Tính toán chi phí du lịch\n✅ Lên kế hoạch chuyến đi\n\nBạn muốn đi du lịch ở đâu? Vui lòng cho tôi biết thông tin như:\n- Thành phố khởi hành\n- Điểm đến\n- Ngân sách\n- Thời gian"
        }
    ]

# Initialize state for agent conversation
if "agent_state" not in st.session_state:
    st.session_state.agent_state = {"messages": []}

# Display chat history
st.write("### 💬 Cuộc trò chuyện")
chat_container = st.container()

with chat_container:
    for message in st.session_state.messages:
        if message["role"] == "user":
            st.markdown(f"""
            <div class="chat-message user-message">
                <div class="message-content">
                    <b>👤 Bạn:</b><br>
                    {message["content"]}
                </div>
            </div>
            """, unsafe_allow_html=True)
        else:
            st.markdown(f"""
            <div class="chat-message assistant-message">
                <div class="message-content">
                    <b>🤖 TravelBuddy:</b><br>
                    {message["content"]}
                </div>
            </div>
            """, unsafe_allow_html=True)

# Input area
st.write("---")
user_input = st.text_input(
    "Nhập câu hỏi của bạn:",
    placeholder="ví dụ: Tôi muốn đi Đà Nẵng với budget 5 triệu...",
    key="user_input"
)

col1, col2, col3 = st.columns([2, 1, 1])

with col1:
    submit_button = st.button("📤 Gửi", use_container_width=True)

with col2:
    clear_button = st.button("🗑️ Xóa lịch sử", use_container_width=True)

with col3:
    st.write("")  # Placeholder

# Process user input
if submit_button and user_input.strip():
    # Add user message to history
    st.session_state.messages.append({
        "role": "user",
        "content": user_input
    })
    
    # Get response from agent
    with st.spinner("🤔 TravelBuddy đang suy nghĩ..."):
        try:
            # Add human message to agent state
            st.session_state.agent_state["messages"].append(HumanMessage(content=user_input))
            
            # Invoke the graph
            result = graph.invoke(st.session_state.agent_state)
            
            # Get the last message from the result
            last_message = result["messages"][-1]
            response_text = last_message.content if hasattr(last_message, 'content') else str(last_message)
            
            # Add to session state
            st.session_state.agent_state["messages"] = result["messages"]
            st.session_state.messages.append({
                "role": "assistant",
                "content": response_text
            })
            st.rerun()
        except Exception as e:
            st.error(f"❌ Lỗi: {str(e)}")
            import traceback
            st.error(traceback.format_exc())

# Clear chat history
if clear_button:
    st.session_state.messages = [
        {
            "role": "assistant",
            "content": "👋 Xin chào! Tôi là TravelBuddy, trợ lý du lịch thông minh của bạn. Tôi có thể giúp bạn:\n\n✅ Tìm chuyến bay\n✅ Tìm khách sạn\n✅ Tính toán chi phí du lịch\n✅ Lên kế hoạch chuyến đi\n\nBạn muốn đi du lịch ở đâu? Vui lòng cho tôi biết thông tin như:\n- Thành phố khởi hành\n- Điểm đến\n- Ngân sách\n- Thời gian"
        }
    ]
    st.rerun()

# Footer
st.write("---")
st.markdown("""
<div style="text-align: center; color: #111; font-size: 0.9rem;">
    <p>🚀 TravelBuddy v1.0 | Powered by LangChain + Groq</p>
</div>
""", unsafe_allow_html=True)
