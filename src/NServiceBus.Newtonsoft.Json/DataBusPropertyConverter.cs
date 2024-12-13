namespace NServiceBus.Newtonsoft.Json
{
    using System;
    using global::Newtonsoft.Json;

    class DataBusPropertyConverter : JsonConverter<IClaimCheckProperty>
    {
        public override void WriteJson(JsonWriter writer, IClaimCheckProperty value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName(nameof(value.Key));
            writer.WriteValue(value.Key);
            writer.WritePropertyName(nameof(value.HasValue));
            writer.WriteValue(value.HasValue);
            writer.WriteEndObject();
        }

        public override IClaimCheckProperty ReadJson(JsonReader reader, Type objectType, IClaimCheckProperty existingValue, bool hasExistingValue, JsonSerializer serializer) => throw new NotImplementedException();

        public override bool CanRead => false;
    }
}
