namespace NServiceBus.Newtonsoft.Json
{
    using System;
    using global::Newtonsoft.Json;

    class ClaimCheckPropertyConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            dynamic dynamicValue = value;

            writer.WriteStartObject();
            writer.WritePropertyName(nameof(dynamicValue.Key));
            writer.WriteValue(dynamicValue.Key);
            writer.WritePropertyName(nameof(dynamicValue.HasValue));
            writer.WriteValue(dynamicValue.HasValue);
            writer.WriteEndObject();
        }

        public override bool CanConvert(Type objectType) => objectType.Name.Equals("ClaimCheckProperty`1", StringComparison.Ordinal);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) => throw new NotImplementedException();

        public override bool CanRead => false;
    }
}
