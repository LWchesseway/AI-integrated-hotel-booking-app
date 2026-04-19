namespace DoAn.HotelParking.Core.Application.Interfaces.Storage;

public interface IObjectStorageService
{
    Task<string> UploadAsync(
        Stream stream,
        long fileSize,
        string objectKey,
        string contentType,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(string objectKey, CancellationToken cancellationToken = default);
}
