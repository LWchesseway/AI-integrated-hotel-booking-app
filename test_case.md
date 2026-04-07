PS D:\Day4_2A202600479> python agent.py
============================================================
TravelBuddy — Trợ lý Du lịch Thông minh
 Gõ 'quit' để thoát
============================================================

Bạn: Xin chào! Tôi đang muốn đi du lịch nhưng chưa biết đi đâu

TravelBuddy đang suy nghĩ...
Trả lời trực tiếp

TravelBuddy: Chào bạn! Để mình tư vấn chuyến đi phù hợp, bạn có thể cho mình biết:

- Thành phố khởi hành (địa điểm xuất phát)
- Ngân sách dự kiến (VNĐ)
- Thời gian du lịch (ngày tháng, hoặc khoảng thời gian)
- Sở thích, điểm đến mong muốn (ví dụ: biển, núi, văn hoá, ẩm thực…)

Khi có đủ thông tin, mình sẽ nhanh chóng tìm chuyến bay, khách sạn và tính tổng chi phí cho bạn.

Bạn: Tìm giúp tôi chuyến bay từ Hà Nội đi Đà Nẵng

TravelBuddy đang suy nghĩ...
Gọi tool: search_flight({'destination': 'Đà Nẵng', 'origin': 'Hà Nội'})
Trả lời trực tiếp

TravelBuddy: **Chuyến bay:**

- *Vietnam Airlines* – 06:00 → 07:20 – 1,450,000 VND (hạng economy)  
- *VietJet Air* – 08:30 → 09:50 – 890,000 VND (hạng economy)  
- *Bamboo Airways* – 11:00 → 12:20 – 1,200,000 VND (hạng economy)  
- *Vietnam Airlines* – 14:00 → 15:20 – 2,800,000 VND (hạng business)  
- *VietJet Air* – 17:45 → 19:05 – 990,000 VND (hạng economy)  

> **Lưu ý**: Nếu bạn muốn đi vào cuối tuần (thứ 6/7), giá vé thường cao hơn. Bạn có thể chọn ngày trong tuần để tiết kiệm.

---

Để lên kế hoạch hoàn chỉnh, mình cần thêm một vài thông tin:

1. Ngân sách tổng cộng (VNĐ)  
2. Số ngày dự kiến ở Đà Nẵng  
3. Sở thích/điểm du lịch quan trọng (ví dụ: bãi biển, di tích lịch sử, ẩm thực địa phương)  
4. Thời gian khởi hành (ngày/tháng)  

Bạn cho mình biết nhé, mình sẽ tìm khách sạn phù hợp, tính tổng chi phí và đề xuất kế hoạch chi tiết!

Bạn: Tôi ở Hà Nội, muốn đi Phú Quốc 2 đêm, budget 5 triệu. Tư vấn giúp!

TravelBuddy đang suy nghĩ...
Gọi tool: search_flight({'destination': 'Phú Quốc', 'origin': 'Hà Nội'})
Gọi tool: search_hotel({'city': 'Phú Quốc', 'max_price_per_night': 1800000})
Trả lời trực tiếp

TravelBuddy: **Chuyến bay:**

- **VietJet Air** – 10:00 → 12:15 (Hà Nội → Phú Quốc) – 1 350 000 VND  
- **VietJet Air** – 13:45 → 15:50 (Phú Quốc → Hà Nội) – 1 350 000 VND  
  *(Lưu ý: giá vé có thể thay đổi theo thời điểm đặt, bạn nên đặt sớm để giữ giá này)*  

**Khách sạn:**

- **Palm Garden Lodge** – 3 sao – 720 000 VND/đêm  
  - Đánh giá: 4.0/5  
  - Địa chỉ: 123 Đường Trần Hưng Đạo, Phú Quốc  
  - Gợi ý: Khu vực gần biển, tiện lợi cho các hoạt động vui chơi, ăn uống.

**Tổng chi phí ước tính:**

| Hạng mục | Chi phí |
|----------|---------|
| Vé máy bay (đoạn khứ hồi) | 2 700 000 VND |
| Khách sạn 2 đêm | 1 440 000 VND |
| **Tổng cộng** | **4 140 000 VND** |

**Phân bổ ngân sách còn lại:**

- Ngân sách dự kiến: 5 000 000 VND  
- Chi phí đã tính: 4 140 000 VND  
- **Ngân sách còn lại**: 860 000 VND

Bạn có thể dùng 860 000 VND này để:

- Thưởng thức ẩm thực địa phương (cơm tấm, bánh xèo, hải sản tươi sống).  
- Tham gia các hoạt động giải trí (đi lặn, thuê xe máy, tham quan bãi đá, thăm đảo lặn).  
- Mua quà lưu niệm hoặc các dịch vụ bổ sung (đặt tour tham quan, thuê xe).

> **Lưu ý**: Nếu bạn muốn đi vào cuối tuần (thứ 6/7), giá vé và khách sạn thường cao hơn. Để tiết kiệm, bạn có thể lựa chọn ngày trong tuần hoặc đặt sớm.  

Nếu muốn thay đổi điểm đến, thời gian, hoặc có ngân sách khác, cứ cho mình biết nhé!

Bạn: Tôi muốn đặt khách sạn

TravelBuddy đang suy nghĩ...
Gọi tool: search_hotel({'city': 'Phú Quốc', 'max_price_per_night': 720000})
Trả lời trực tiếp

TravelBuddy: **Chuyến bay:**  
- **VietJet Air** – 10:00 → 12:15 (Hà Nội → Phú Quốc) – 1 350 000 VND  
- **VietJet Air** – 13:45 → 15:50 (Phú Quốc → Hà Nội) – 1 350 000 VND  

**Khách sạn:**  
- **Palm Garden Lodge** – 3 sao – 720 000 VND/đêm (đánh giá 4.0/5, địa chỉ 123 Đường Trần Hưng Đạo, Phú Quốc)

**Tổng chi phí ước tính:**  
- Vé máy bay (đoạn khứ hồi): 2 700 000 VND  
- Khách sạn 2 đêm: 1 440 000 VND  
- **Tổng cộng:** 4 140 000 VND  

Bạn còn **860 000 VND** trong ngân sách để thưởng thức ẩm thực, tham gia các hoạt động vui chơi, mua quà lưu niệm hoặc thuê xe máy. Nếu muốn điều chỉnh thời gian hoặc thêm dịch vụ, cứ cho mình biết nhé!

Bạn: exit
PS D:\Day4_2A202600479> python agent.py
============================================================
TravelBuddy — Trợ lý Du lịch Thông minh
 Gõ 'quit' để thoát
============================================================

Bạn: Tôi muốn đặt khách sạn

TravelBuddy đang suy nghĩ...
Trả lời trực tiếp

TravelBuddy: Bạn muốn đặt khách sạn ở đâu và trong khoảng thời gian nào?  
- Thành phố/địa điểm  
- Ngày check‑in và check‑out  
- Ngân sách tối đa mỗi đêm (hoặc ngân sách tổng cho toàn bộ kỳ nghỉ)  

Với những thông tin này mình sẽ tìm kiếm khách sạn phù hợp và đưa ra ước tính chi phí.

Bạn: Giải giúp tôi bài tập lập trình Python về linked list

TravelBuddy đang suy nghĩ...
Trả lời trực tiếp

TravelBuddy: Xin lỗi, tôi chỉ hỗ trợ về các yêu cầu liên quan đến du lịch như tìm chuyến bay, giá vé, đặt vé, tìm phòng khách sạn. Nếu bạn có bất kỳ câu hỏi nào trong lĩnh vực này, mình rất sẵn lòng giúp đỡ!

Bạn: ẽit

TravelBuddy đang suy nghĩ...
Trả lời trực tiếp

TravelBuddy: Xin lỗi, tôi chỉ hỗ trợ các yêu cầu liên quan đến du lịch như tìm chuyến bay, giá vé, đặt vé, tìm phòng khách sạn. Nếu bạn cần giúp đỡ trong lĩnh vực này, vui lòng cho biết chi tiết nhé!

Bạn: exit