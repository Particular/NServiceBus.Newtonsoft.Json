using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NServiceBus.MessageInterfaces;
using NServiceBus.Serialization;
using NewtonSerializer = Newtonsoft.Json.JsonSerializer;

namespace NServiceBus.Newtonsoft.Json
{
    class JsonMessageSerializer : IMessageSerializer
    {
        IMessageMapper messageMapper;
        MessageContractResolver messageContractResolver;
        Func<Stream, JsonReader> readerCreator;
        JsonSerializerSettings settings;
        Func<Stream, JsonWriter> writerCreator;

        public JsonMessageSerializer(
            IMessageMapper messageMapper,
            Func<Stream, JsonReader> readerCreator,
            Func<Stream, JsonWriter> writerCreator,
            JsonSerializerSettings settings,
            string contentType)
        {
            this.messageMapper = messageMapper;
            messageContractResolver = new MessageContractResolver(messageMapper);

            this.settings = settings ?? new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                Converters =
                {
                    new IsoDateTimeConverter
                    {
                        DateTimeStyles = DateTimeStyles.RoundtripKind
                    }
                }
            };

            this.writerCreator = writerCreator ?? (stream =>
            {
                var streamWriter = new StreamWriter(stream, Encoding.UTF8);
                return new JsonTextWriter(streamWriter)
                {
                    Formatting = Formatting.None
                };
            });

            this.readerCreator = readerCreator ?? (stream =>
            {
                var streamReader = new StreamReader(stream, Encoding.UTF8);
                return new JsonTextReader(streamReader);
            });

            if (contentType == null)
            {
                ContentType = ContentTypes.Json;
            }
            else
            {
                ContentType = contentType;
            }
        }

        public void Serialize(object message, Stream stream)
        {
            Guard.AgainstNull(stream, "stream");
            Guard.AgainstNull(message, "message");
            var jsonSerializer = NewtonSerializer.Create(settings);
            jsonSerializer.Binder = new MessageSerializationBinder(messageMapper);
            var jsonWriter = writerCreator(stream);
            jsonSerializer.Serialize(jsonWriter, message);
            jsonWriter.Flush();
        }

        public object[] Deserialize(Stream stream, IList<Type> messageTypes)
        {
            Guard.AgainstNull(stream, "stream");
            Guard.AgainstNull(messageTypes, "messageTypes");
            var jsonSerializer = NewtonSerializer.Create(settings);
            jsonSerializer.ContractResolver = messageContractResolver;
            jsonSerializer.Binder = new MessageSerializationBinder(messageMapper, messageTypes);

            if (IsArrayStream(stream))
            {
                var arrayReader = readerCreator(stream);
                return jsonSerializer.Deserialize<object[]>(arrayReader);
            }

            if (messageTypes.Any())
            {
                return DeserializeMultipleMesageTypes(stream, messageTypes, jsonSerializer).ToArray();
            }

            var simpleReader = readerCreator(stream);
            return new[]
            {
                jsonSerializer.Deserialize<object>(simpleReader)
            };
        }

        public string ContentType { get; }

        IEnumerable<object> DeserializeMultipleMesageTypes(Stream stream, IList<Type> messageTypes, NewtonSerializer jsonSerializer)
        {
            foreach (var messageType in FindRootTypes(messageTypes))
            {
                stream.Seek(0, SeekOrigin.Begin);
                var reader = readerCreator(stream);
                yield return jsonSerializer.Deserialize(reader, messageType);
            }
        }

        bool IsArrayStream(Stream stream)
        {
            var reader = readerCreator(stream);
            reader.Read();
            stream.Seek(0, SeekOrigin.Begin);
            return reader.TokenType == JsonToken.StartArray;
        }

        static IEnumerable<Type> FindRootTypes(IEnumerable<Type> messageTypesToDeserialize)
        {
            Type currentRoot = null;
            foreach (var type in messageTypesToDeserialize)
            {
                if (currentRoot == null)
                {
                    currentRoot = type;
                    yield return currentRoot;
                    continue;
                }
                if (!type.IsAssignableFrom(currentRoot))
                {
                    currentRoot = type;
                    yield return currentRoot;
                }
            }
        }

    }
}