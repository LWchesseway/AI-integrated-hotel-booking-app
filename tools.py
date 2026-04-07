from langchain_core.tools import tool

FLIGHTS_DB = {
    ("Hà Nội", "Đà Nẵng"): [
        {"airline": "Vietnam Airlines", "departure": "06:00", "arrival": "07:20", "price": 1_450_000, "class": "economy"},
        {"airline": "Vietnam Airlines", "departure": "14:00", "arrival": "15:20", "price": 2_800_000, "class": "business"},
        {"airline": "VietJet Air", "departure": "08:30", "arrival": "09:50", "price": 890_000, "class": "economy"},
        {"airline": "VietJet Air", "departure": "17:45", "arrival": "19:05", "price": 990_000, "class": "economy"},
        {"airline": "Bamboo Airways", "departure": "11:00", "arrival": "12:20", "price": 1_200_000, "class": "economy"},
    ],
    ("Hà Nội", "Phú Quốc"): [
        {"airline": "Vietnam Airlines", "departure": "07:00", "arrival": "09:15", "price": 2_100_000, "class": "economy"},
        {"airline": "Vietnam Airlines", "departure": "20:30", "arrival": "22:45", "price": 2_300_000, "class": "business"},
        {"airline": "VietJet Air", "departure": "10:00", "arrival": "12:15", "price": 1_350_000, "class": "economy"},
        {"airline": "Bamboo Airways", "departure": "15:20", "arrival": "17:35", "price": 1_500_000, "class": "economy"},
    ],
    ("Hồ Chí Minh", "Đà Nẵng"): [
        {"airline": "VietJet Air", "departure": "09:30", "arrival": "11:00", "price": 1_000_000, "class": "economy"},
        {"airline": "Vietnam Airlines", "departure": "13:00", "arrival": "14:30", "price": 1_550_000, "class": "economy"},
        {"airline": "Bamboo Airways", "departure": "06:45", "arrival": "08:15", "price": 1_100_000, "class": "economy"},
    ],
    ("Hồ Chí Minh", "Phú Quốc"): [
        {"airline": "VietJet Air", "departure": "08:00", "arrival": "09:50", "price": 1_280_000, "class": "economy"},
        {"airline": "Vietnam Airlines", "departure": "12:30", "arrival": "14:20", "price": 1_950_000, "class": "economy"},
        {"airline": "Bamboo Airways", "departure": "16:10", "arrival": "18:00", "price": 1_390_000, "class": "economy"},
    ],
    ("Hà Nội", "Huế"): [
        {"airline": "Vietnam Airlines", "departure": "06:45", "arrival": "08:25", "price": 1_250_000, "class": "economy"},
        {"airline": "VietJet Air", "departure": "14:15", "arrival": "15:55", "price": 1_050_000, "class": "economy"},
    ],
    ("Hà Nội", "Nha Trang"): [
        {"airline": "Vietnam Airlines", "departure": "09:00", "arrival": "11:10", "price": 1_750_000, "class": "economy"},
        {"airline": "Bamboo Airways", "departure": "18:20", "arrival": "20:35", "price": 1_850_000, "class": "economy"},
    ],
}

HOTELS_DB = {
    "Đà Nẵng": [
        {"name": "Mường Thanh Luxury", "stars": 5, "price_per_night": 1_800_000, "area": "Mỹ Khê", "rating": 4.5},
        {"name": "Sala Danang Beach", "stars": 4, "price_per_night": 1_200_000, "area": "Mỹ Khê", "rating": 4.3},
        {"name": "Fivitel Danang", "stars": 3, "price_per_night": 650_000, "area": "Sơn Trà", "rating": 4.1},
        {"name": "Memory Hostel", "stars": 2, "price_per_night": 250_000, "area": "Hải Châu", "rating": 4.6},
        {"name": "Christina's Homestay", "stars": 2, "price_per_night": 350_000, "area": "An Thượng", "rating": 4.7},
        {"name": "Diamond Sea Hotel", "stars": 4, "price_per_night": 980_000, "area": "Ngũ Hành Sơn", "rating": 4.4},
        {"name": "Lotus Riverside", "stars": 3, "price_per_night": 560_000, "area": "Hải Châu", "rating": 4.0},
        {"name": "Sunny Beach Hotel", "stars": 4, "price_per_night": 1_350_000, "area": "Sơn Trà", "rating": 4.2},
    ],
    "Phú Quốc": [
        {"name": "Sunset Resort", "stars": 5, "price_per_night": 2_300_000, "area": "Bãi Sao", "rating": 4.8},
        {"name": "Sea Breeze Hotel", "stars": 4, "price_per_night": 1_100_000, "area": "Dương Đông", "rating": 4.2},
        {"name": "Ocean Pearl Villa", "stars": 5, "price_per_night": 2_000_000, "area": "An Thới", "rating": 4.6},
        {"name": "Palm Garden Lodge", "stars": 3, "price_per_night": 720_000, "area": "Dương Tơ", "rating": 4.0},
    ],
    "Huế": [
        {"name": "Imperial Hue Hotel", "stars": 5, "price_per_night": 1_900_000, "area": "Phú Nhuận", "rating": 4.7},
        {"name": "Royal Centre", "stars": 4, "price_per_night": 1_100_000, "area": "Vĩnh Ninh", "rating": 4.3},
        {"name": "Lotus Hue Homestay", "stars": 3, "price_per_night": 520_000, "area": "Phú Hậu", "rating": 4.2},
    ],
    "Nha Trang": [
        {"name": "Coral Bay Hotel", "stars": 5, "price_per_night": 2_100_000, "area": "Bãi Dài", "rating": 4.8},
        {"name": "Sunny Sea Resort", "stars": 4, "price_per_night": 1_250_000, "area": "Nha Trang City", "rating": 4.4},
        {"name": "Budget Stay Inn", "stars": 2, "price_per_night": 420_000, "area": "Vĩnh Phước", "rating": 4.1},
    ],
}

@tool 
def search_flight(origin: str, destination: str) -> str:
    """Tìm kiếm các chuyến bay giữa các thành phố ở Việt Nam.
    Args:
        origin (str): Thành phố khởi hành
        destination (str): Thành phố đến
    Returns:
        Danh sách các chuyến bay phù hợp bao gồm chuyến bay,hãng bay,giờ bay và giá vé.
        Nếu không tìm thấy chuyến bay nào, trả về thông báo phù hợp.
    """
    key = (origin, destination)
    if key in FLIGHTS_DB:
        flights = FLIGHTS_DB[key]
        return "\n".join([f"{f['airline']} - {f['departure']} -> {f['arrival']} - {f['price']:,} VND ({f['class']})" for f in flights])
    else:
        return "Không tìm thấy chuyến bay phù hợp giữa {} và {}.".format(origin, destination)
@tool
def search_hotel(city: str, max_price_per_night: int = 99999999) -> str:
    """Tìm kiếm khách sạn trong thành phố với mức giá tối đa mỗi đêm.
    Args:
        city (str): Thành phố cần tìm khách sạn
        max_price_per_night (int): Mức giá tối đa mỗi đêm (VNĐ),mặc định không giới hạn
    Returns:
        Danh sách các khách sạn phù hợp bao gồm tên, số sao, giá mỗi đêm và đánh giá.
        Nếu không tìm thấy khách sạn nào, trả về thông báo phù hợp.
    """
    if city in HOTELS_DB:
        hotels = HOTELS_DB[city]
        if max_price_per_night is not None:
            hotels = [h for h in hotels if h["price_per_night"] <= max_price_per_night]
        if hotels:
            return "\n".join([f"{h['name']} - {h['stars']} sao - {h['price_per_night']:,} VND/đêm - Đánh giá: {h['rating']}" for h in hotels])
        else:
            return "Không tìm thấy khách sạn nào ở {} với mức giá tối đa {} VND mỗi đêm.".format(city, max_price_per_night)
    else:
        return "Không tìm thấy khách sạn nào ở {}.".format(city)
@tool
def calculate_budget(total_budget:int,expense:str) -> str:
    """Tính toán ngân sách còn lại sau khi trừ đi chi phí đã biết.
    Args:
        total_budget (int): Ngân sách tổng cộng
        expense (str): Chi phí đã biết (có thể là giá vé máy bay hoặc giá khách sạn)
    Returns:
        Ngân sách còn lại sau khi trừ đi chi phí đã biết.
        Nếu vượt ngân sách thông báo chuyến đi không phù hợp.
    """
    try:
        expense_amount = int(expense.replace(".", "").replace(",", ""))
        remaining_budget = total_budget - expense_amount
        if remaining_budget < 0:
            return "Ngân sách của bạn không đủ để chi trả cho chi phí này."
        return f"Ngân sách còn lại sau khi trừ đi {expense_amount:,} VND là {remaining_budget:,} VND."
    except ValueError:
        return "Không thể tính toán ngân sách vì chi phí không hợp lệ: {}".format(expense)
   