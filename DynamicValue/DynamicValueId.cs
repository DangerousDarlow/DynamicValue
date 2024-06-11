using System.Text.Json;
using System.Text.Json.Serialization;

namespace DynamicValue;

[JsonConverter(typeof(DynamicValueIdJsonConverter))]
public readonly record struct DynamicValueId(Guid Value)
{
    public static implicit operator Guid(DynamicValueId userId) => userId.Value;
}

public static class DynamicValueIdExtensions
{
    public static DynamicValueId ToDynamicValueId(this Guid guid) => new(guid);
}

public class DynamicValueIdJsonConverter : JsonConverter<DynamicValueId>
{
    public override DynamicValueId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => new(reader.GetGuid());

    public override void Write(Utf8JsonWriter writer, DynamicValueId value, JsonSerializerOptions options) => writer.WriteStringValue(value.Value);
}