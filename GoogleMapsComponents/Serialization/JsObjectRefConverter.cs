using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GoogleMapsComponents.Serialization;

internal class JsObjectRefConverter<T> : JsonConverter<T>
    where T : IJsObjectRef
{
    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "Values should not be trimmed.")]
    public override void Write(Utf8JsonWriter writer, T? value, JsonSerializerOptions options)
    {
        if (value is null) return;
        
        writer.WriteStartObject();
        using var doc = JsonSerializer.SerializeToDocument(new JsObjectRef1(value.Guid), typeof(JsObjectRef1), options);

        foreach (var prop in doc.RootElement.EnumerateObject())
        {
            prop.WriteTo(writer);
        }

        writer.WriteEndObject();
    }
}