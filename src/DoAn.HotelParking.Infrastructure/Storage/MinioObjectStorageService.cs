using DoAn.HotelParking.Core.Application.Interfaces.Storage;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using System.Threading;

namespace DoAn.HotelParking.Infrastructure.Storage;

public class MinioObjectStorageService : IObjectStorageService
{
    private readonly IMinioClient _minioClient;
    private readonly MinioSettings _settings;
    private readonly string _endpointForUrl;
    private readonly bool _effectiveUseSsl;
    private readonly SemaphoreSlim _bucketInitLock = new(1, 1);
    private bool _bucketInitialized;

    public MinioObjectStorageService(IOptions<MinioSettings> options)
    {
        _settings = options.Value;

        if (string.IsNullOrWhiteSpace(_settings.Endpoint)
            || string.IsNullOrWhiteSpace(_settings.AccessKey)
            || string.IsNullOrWhiteSpace(_settings.SecretKey)
            || string.IsNullOrWhiteSpace(_settings.BucketName))
        {
            throw new InvalidOperationException("MinioSettings is missing required values.");
        }

        var endpoint = NormalizeEndpoint(_settings.Endpoint, out _endpointForUrl, out var useSslFromEndpoint);
        _effectiveUseSsl = _settings.UseSsl || useSslFromEndpoint;

        _minioClient = new MinioClient()
            .WithEndpoint(endpoint)
            .WithCredentials(_settings.AccessKey, _settings.SecretKey)
            .WithSSL(_effectiveUseSsl)
            .Build();
    }

    public async Task<string> UploadAsync(
        Stream stream,
        long fileSize,
        string objectKey,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        await EnsureBucketExistsAsync(cancellationToken);

        if (stream.CanSeek)
        {
            stream.Position = 0;
        }

        await _minioClient.PutObjectAsync(
            new PutObjectArgs()
                .WithBucket(_settings.BucketName)
                .WithObject(objectKey)
                .WithStreamData(stream)
                .WithObjectSize(fileSize)
                .WithContentType(string.IsNullOrWhiteSpace(contentType) ? "application/octet-stream" : contentType),
            cancellationToken);

        return BuildObjectUrl(objectKey);
    }

    public async Task DeleteAsync(string objectKey, CancellationToken cancellationToken = default)
    {
        await _minioClient.RemoveObjectAsync(
            new RemoveObjectArgs()
                .WithBucket(_settings.BucketName)
                .WithObject(objectKey),
            cancellationToken);
    }

    private async Task EnsureBucketExistsAsync(CancellationToken cancellationToken)
    {
        if (_bucketInitialized)
        {
            return;
        }

        await _bucketInitLock.WaitAsync(cancellationToken);
        try
        {
            if (_bucketInitialized)
            {
                return;
            }

            var bucketExists = await _minioClient.BucketExistsAsync(
                new BucketExistsArgs().WithBucket(_settings.BucketName),
                cancellationToken);

            if (!bucketExists)
            {
                if (!_settings.AutoCreateBucket)
                {
                    throw new InvalidOperationException($"MinIO bucket '{_settings.BucketName}' does not exist.");
                }

                await _minioClient.MakeBucketAsync(
                    new MakeBucketArgs().WithBucket(_settings.BucketName),
                    cancellationToken);
            }

            if (_settings.SetBucketPublicRead)
            {
                await _minioClient.SetPolicyAsync(
                    new SetPolicyArgs()
                        .WithBucket(_settings.BucketName)
                        .WithPolicy(BuildPublicReadPolicy(_settings.BucketName)),
                    cancellationToken);
            }

            _bucketInitialized = true;
        }
        finally
        {
            _bucketInitLock.Release();
        }
    }

    private string BuildObjectUrl(string objectKey)
    {
        if (!string.IsNullOrWhiteSpace(_settings.PublicBaseUrl))
        {
            return $"{_settings.PublicBaseUrl.TrimEnd('/')}/{_settings.BucketName}/{objectKey}";
        }

                var scheme = _effectiveUseSsl ? "https" : "http";
                return $"{scheme}://{_endpointForUrl}/{_settings.BucketName}/{objectKey}";
        }

        private static string NormalizeEndpoint(string endpoint, out string endpointForUrl, out bool useSslFromEndpoint)
        {
                var normalized = endpoint.Trim().TrimEnd('/');

                if (normalized.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
                        || normalized.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                {
                        var uri = new Uri(normalized, UriKind.Absolute);
                        if (string.IsNullOrWhiteSpace(uri.Host))
                        {
                                throw new InvalidOperationException("MinIO endpoint host is invalid.");
                        }

                        useSslFromEndpoint = uri.Scheme.Equals(Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase);
                        endpointForUrl = uri.IsDefaultPort ? uri.Host : $"{uri.Host}:{uri.Port}";
                        return endpointForUrl;
                }

                useSslFromEndpoint = false;
                endpointForUrl = normalized;
                return normalized;
        }

        private static string BuildPublicReadPolicy(string bucketName)
        {
                return $$"""
                {
                    "Version": "2012-10-17",
                    "Statement": [
                        {
                            "Effect": "Allow",
                            "Principal": {
                                "AWS": ["*"]
                            },
                            "Action": ["s3:GetObject"],
                            "Resource": ["arn:aws:s3:::{{bucketName}}/*"]
                        }
                    ]
                }
                """;
    }
}
