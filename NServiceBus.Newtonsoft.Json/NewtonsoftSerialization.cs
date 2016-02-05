using System;
using System.IO;
using Newtonsoft.Json;
using NServiceBus;
using NServiceBus.Features;
using NServiceBus.MessageInterfaces;
using NServiceBus.MessageInterfaces.MessageMapper.Reflection;
using NServiceBus.Newtonsoft.Json;
using NServiceBus.ObjectBuilder;
using NServiceBus.Serialization;
using NServiceBus.Settings;

namespace NServiceBus
{
    /// <summary>
    /// Enables Newtonsoft Json serialization.
    /// </summary>
    public class NewtonsoftSerialization : SerializationDefinition
    {
        /// <summary>
        /// Provides a factory method for building a message serializer.
        /// </summary>
        protected override Func<IMessageMapper, IMessageSerializer> Configure(ReadOnlySettings settings)
        {
            return mapper =>
            {
                var readerCreator = settings.GetOrDefault<Func<Stream, JsonReader>>("NServiceBus.Newtonsoft.Json.ReaderCreator");
                var writerCreator = settings.GetOrDefault<Func<Stream, JsonWriter>>("NServiceBus.Newtonsoft.Json.WriterCreator");
                var serializerSettings = settings.GetOrDefault<JsonSerializerSettings>("NServiceBus.Newtonsoft.Json.Settings");

                var serializer = new JsonMessageSerializer(mapper, readerCreator, writerCreator, serializerSettings);
                return serializer;
            };
        }
    }
}