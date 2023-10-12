namespace NServiceBus.Newtonsoft.Json
{
    using System;
    using global::Newtonsoft.Json;

    class DataBusPropertyConverter : JsonConverter<IDataBusProperty>
    {
        public override void WriteJson(JsonWriter writer, IDataBusProperty value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName(nameof(value.Key));
            writer.WriteValue(value.Key);
            writer.WritePropertyName(nameof(value.HasValue));
            writer.WriteValue(value.HasValue);
            writer.WriteEndObject();
        }

        public override IDataBusProperty ReadJson(JsonReader reader, Type objectType, IDataBusProperty existingValue, bool hasExistingValue, JsonSerializer serializer) => throw new NotImplementedException();

        public override bool CanRead => false;
    }
}
