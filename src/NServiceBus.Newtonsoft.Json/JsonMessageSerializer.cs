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
            var jsonWriter = writerCreator(stream);
            var inputMessageType = message.GetType();
            var mappedType = messageMapper.GetMappedTypeFor(inputMessageType);
            if (mappedType != null)
            {
                //TODO: push back into core to pass the interface and not the impl so we can avoid the cast
                message = message.Cast(mappedType);
            }
            jsonSerializer.Serialize(jsonWriter, message);
            jsonWriter.Flush();
        }

        public object[] Deserialize(Stream stream, IList<Type> messageTypes)
        {
            Guard.AgainstNull(stream, "stream");
            Guard.AgainstNull(messageTypes, "messageTypes");
            var jsonSerializer = NewtonSerializer.Create(settings);
            jsonSerializer.ContractResolver = messageContractResolver;

            if (IsArrayStream(stream))
            {
                throw new Exception("Multiple messages in the same stream are not supported.");
            }

            if (messageTypes.Any())
            {
                return DeserializeMultipleMesageTypes(stream, messageTypes, jsonSerializer);
            }

            var simpleReader = readerCreator(stream);
            return new[]
            {
                jsonSerializer.Deserialize<object>(simpleReader)
            };
        }

        public string ContentType { get; }

        object[] DeserializeMultipleMesageTypes(Stream stream, IList<Type> messageTypes, NewtonSerializer jsonSerializer)
        {
            var rootTypes = FindRootTypes(messageTypes).ToList();
            var messages = new object[rootTypes.Count];
            for (var index = 0; index < rootTypes.Count; index++)
            {
                var messageType = rootTypes[index];
                stream.Seek(0, SeekOrigin.Begin);
                var reader = readerCreator(stream);
                messages[index] = jsonSerializer.Deserialize(reader, messageType);
            }
            return messages;
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