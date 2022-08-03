using System.Net;
using System.Text;
using WebApplication1.Models;

namespace WebApplication1.Helpers;

public static class ApiHelper
{
    private const string JSON_MEDIA_TYPE = "application/json";

    private static HttpClient _client = null!;

    public static void Initialize()
    {
        if (_client == null)
            _client = new();
    }

    public static async Task<ApiResult> GetApi(string url)
    {
        ApiResult result = new();

        try
        {
            using (HttpResponseMessage response = await _client.GetAsync(url))
            {
                result.IsSuccess = response.IsSuccessStatusCode;
                result.StatusCode = response.StatusCode;

                if (response.IsSuccessStatusCode)
                    result.ResultApi = await response.Content.ReadAsStringAsync();
                else
                    result.ErrorMessage = await response.Content.ReadAsStringAsync();
            }

            return result;
        }
        catch (Exception ex)
        {
            return new ApiResult(false, HttpStatusCode.InternalServerError, null, ex.Message);
        }
    }

    public static async Task<ApiResult> PostApi(string url, string postData)
    {
        ApiResult result = new();

        try
        {
            using HttpResponseMessage response = await _client.PostAsync(url, new StringContent(postData, Encoding.UTF8, JSON_MEDIA_TYPE));
            result.IsSuccess = response.IsSuccessStatusCode;
            result.StatusCode = response.StatusCode;

            if (response.IsSuccessStatusCode)
                result.ResultApi = await response.Content.ReadAsStringAsync();
            else
                result.ErrorMessage = await response.Content.ReadAsStringAsync();

            return result;
        }
        catch (Exception ex)
        {
            return new ApiResult(false, HttpStatusCode.InternalServerError, null, ex.Message);
        }
    }
}
