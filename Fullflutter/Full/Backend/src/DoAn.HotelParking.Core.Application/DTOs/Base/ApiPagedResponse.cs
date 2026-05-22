namespace DoAn.HotelParking.Core.Application.DTOs.Base;

public class ApiPagedResponse<T> : ApiResponse<IEnumerable<T>>
{
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public int TotalRecords { get; set; }
    public int TotalPages => PageSize <= 0 ? 0 : (int)Math.Ceiling((double)TotalRecords / PageSize);
    public bool HasPreviousPage => PageIndex > 1;
    public bool HasNextPage => PageIndex < TotalPages;

    public static ApiPagedResponse<T> Ok(
        IEnumerable<T> data,
        int pageIndex,
        int pageSize,
        int totalRecords,
        string message = "Success",
        int statusCode = 200)
    {
        return new ApiPagedResponse<T>
        {
            Success = true,
            Message = message,
            StatusCode = statusCode,
            Data = data,
            PageIndex = pageIndex,
            PageSize = pageSize,
            TotalRecords = totalRecords
        };
    }
}