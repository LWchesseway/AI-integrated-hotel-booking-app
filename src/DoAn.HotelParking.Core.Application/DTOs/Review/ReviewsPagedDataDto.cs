using DoAn.HotelParking.Core.Application.DTOs.Base;

namespace DoAn.HotelParking.Core.Application.DTOs.Review;

public class ReviewsPagedDataDto
{
    public List<ReviewDto> Reviews { get; set; } = new();
    public ReviewStatisticsDto Statistics { get; set; } = new();
}

public class ReviewsPagedResponse : ApiResponse<ReviewsPagedDataDto>
{
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public int TotalRecords { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalRecords / PageSize);
    public bool HasPreviousPage => PageIndex > 1;
    public bool HasNextPage => PageIndex < TotalPages;

    public ReviewsPagedResponse(
        ReviewsPagedDataDto data,
        int pageIndex,
        int pageSize,
        int totalRecords,
        string message = "",
        int statusCode = 200)
        : base(true, message, data, statusCode)
    {
        PageIndex = pageIndex;
        PageSize = pageSize;
        TotalRecords = totalRecords;
    }

    public static ReviewsPagedResponse Ok(
        ReviewsPagedDataDto data,
        int pageIndex,
        int pageSize,
        int totalRecords,
        string message = "")
    => new ReviewsPagedResponse(data, pageIndex, pageSize, totalRecords, message, 200);
}
