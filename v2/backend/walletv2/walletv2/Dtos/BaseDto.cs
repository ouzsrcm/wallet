namespace walletv2.Dtos;

public enum ApiResponseStatus
{
    Success,
    Error,
    ServerError,
    NotFound,
    Unauthorized
}

public class BaseResponseDto
{
    public ApiResponseStatus Status { get; set; } = ApiResponseStatus.Success;
    public string? Message { get; set; }
}

public class BaseRequestDto
{
    //TODO: something like a request ID or correlation ID can be added here
}