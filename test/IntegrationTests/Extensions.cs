using System.Text.Json;
using System.Text.Json.Serialization;

namespace MoneyGroup.IntegrationTests;

public static class Extensions
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
    };

    public static string ToJson<T>(this T obj)
    {
        return JsonSerializer.Serialize(obj, Options);
    }
}