# TravelBuddy Smart Travel Agent

Một ví dụ Python agent sử dụng `langchain`, `groq` và module mock `langgrap` để xây dựng trợ lý du lịch.

## File structure

- `systemprompt.txt`: prompt hệ thống cho agent.
- `tools.py`: mock dữ liệu chuyến bay và khách sạn, viết các tool
- `agent.py`: agent chính dùng `langchain` và `groq`.
- `requirements.txt`: gói Python cần cài.

## Cài đặt

1. Cài dependencies:
   ```bash
   pip install -r requirements.txt
   ```

2. Thiết lập API key Groq:
   - Tạo file `.env` từ `.env.example` và thêm key:
     ```
     GROQ_API_KEY=your_groq_api_key_here
     ```
   - Hoặc Windows PowerShell:
     ```powershell
     setx GROQ_API_KEY "your_api_key_here"
     ```

## Chạy agent

### Chế độ dòng lệnh (Terminal)

```bash
python cli.py
```

Giao diện tương tác trong terminal, dễ sử dụng và không cần trình duyệt.

### Chế độ Web UI (Streamlit) - Khuyên dùng

```bash
streamlit run app.py
```

Mở trình duyệt tại `http://localhost:8501` để sử dụng giao diện web hiện đại và đẹp mắt.

### Chế độ script Python

```bash
python agent.py
```

Chạy một lần với câu hỏi cố định trong file.

