using System.Text.Json;

namespace WebApplication1.Helpers;

public static class JsonHelper
{
    private static JsonSerializerOptions _options = default!;

    public static void Initialize()
    {
        if (_options == null)
            _options = new(JsonSerializerDefaults.Web);
    }

    public static string Serialize<T>(T classData)
    {
        try
        {
            string result = JsonSerializer.Serialize(classData, _options);

            return result;
        }
        catch
        {
            return string.Empty;
        }

    }

    public static T? Deserialize<T>(string json)
    {
        try
        {
            T? result = JsonSerializer.Deserialize<T>(json, _options);

            return result;
        }
        catch
        {
            return default;
        }
    }
}
