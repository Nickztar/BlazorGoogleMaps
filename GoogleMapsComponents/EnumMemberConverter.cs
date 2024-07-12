using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GoogleMapsComponents;

internal class EnumMemberConverter<[DynamicallyAccessedMembers(Helper.JsonSerialized)] T> : JsonConverter<T> where T : IComparable, IFormattable, IConvertible
{
    public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var jsonValue = reader.GetString();

#pragma warning disable IL2070
        foreach (var fi in typeToConvert.GetFields())
#pragma warning restore IL2070
        {
            var description = (EnumMemberAttribute?)fi.GetCustomAttribute(typeof(EnumMemberAttribute), false);

            if (description == null) continue;
            if (string.Equals(description.Value, jsonValue, StringComparison.OrdinalIgnoreCase))
            {
                return (T?)fi.GetValue(null);
            }
        }

        throw new JsonException($"string {jsonValue} was not found as a description in the enum {typeToConvert}");
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        var valueName = value.ToString();
        if (valueName is null) return;
        var fi = value.GetType().GetField(valueName);
        var description = (EnumMemberAttribute?)fi?.GetCustomAttribute(typeof(EnumMemberAttribute), false);

        if (description is null) return;
        writer.WriteStringValue(description.Value);
    }
}