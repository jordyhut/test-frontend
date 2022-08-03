using System.Net;

namespace WebApplication1.Models;

public class ApiResult
{
    public bool IsSuccess { get; set; }
    public HttpStatusCode StatusCode { get; set; }
    public string? ResultApi { get; set; }
    public string? ErrorMessage { get; set; }

    public ApiResult(bool isSuccess, HttpStatusCode statusCode, string? resultApi, string? errorMessage)
    {
        IsSuccess = isSuccess;
        StatusCode = statusCode;
        ResultApi = resultApi;
        ErrorMessage = errorMessage;
    }

    public ApiResult()
    {
    }
}

