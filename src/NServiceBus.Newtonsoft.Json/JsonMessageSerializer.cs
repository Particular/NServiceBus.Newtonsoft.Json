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
        Func<Stream, JsonReader> readerCreator;
        Func<Stream, JsonWriter> writerCreator;
        NewtonSerializer jsonSerializer;

        public JsonMessageSerializer(
            IMessageMapper messageMapper,
            Func<Stream, JsonReader> readerCreator,
            Func<Stream, JsonWriter> writerCreator,
            JsonSerializerSettings settings,
            string contentType)
        {
            this.messageMapper = messageMapper;

            settings = settings ?? new JsonSerializerSettings
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
            jsonSerializer = NewtonSerializer.Create(settings);
        }

        public void Serialize(object message, Stream stream)
        {
            Guard.AgainstNull(stream, nameof(stream));
            Guard.AgainstNull(message, nameof(message));

            message = CastToCorrectType(message);
            using (var writer = writerCreator(stream))
            {
                writer.CloseOutput = false;
                jsonSerializer.Serialize(writer, message);
                writer.Flush();
            }
        }

        object CastToCorrectType(object message)
        {
            var inputMessageType = message.GetType();
            //TODO: push back into core to pass the interface and not the impl so we can avoid the cast
            if (!inputMessageType.IsInterface)
            {
                var mappedType = messageMapper.GetMappedTypeFor(inputMessageType);
                if (mappedType != null)
                {
                    message = message.Cast(mappedType);
                }
            }
            return message;
        }

        public object[] Deserialize(Stream stream, IList<Type> messageTypes)
        {
            Guard.AgainstNull(stream, nameof(stream));
            Guard.AgainstNull(messageTypes, nameof(messageTypes));

            if (IsArrayStream(stream))
            {
                throw new Exception("Multiple messages in the same stream are not supported.");
            }

            if (messageTypes.Any())
            {
                return DeserializeMultipleMesageTypes(stream, messageTypes);
            }

            using (var reader = readerCreator(stream))
            {
                reader.CloseInput = false;
                return new[]
                {
                    jsonSerializer.Deserialize<object>(reader)
                };
            }
        }

        public string ContentType { get; }

        object[] DeserializeMultipleMesageTypes(Stream stream, IList<Type> messageTypes)
        {
            var rootTypes = FindRootTypes(messageTypes).ToList();
            var messages = new object[rootTypes.Count];
            for (var index = 0; index < rootTypes.Count; index++)
            {
                var messageType = rootTypes[index];
                stream.Seek(0, SeekOrigin.Begin);

                messageType = GetMappedType(messageType);
                using (var reader = readerCreator(stream))
                {
                    reader.CloseInput = false;
                    messages[index] = jsonSerializer.Deserialize(reader, messageType);
                }
            }
            return messages;
        }

        Type GetMappedType(Type messageType)
        {
            if (messageType.IsInterface)
            {
                var mappedTypeFor = messageMapper.GetMappedTypeFor(messageType);
                if (mappedTypeFor != null)
                {
                    return mappedTypeFor;
                }
            }
            return messageType;
        }

        bool IsArrayStream(Stream stream)
        {
            using (var reader = readerCreator(stream))
            {
                reader.CloseInput = false;
                reader.Read();
                stream.Seek(0, SeekOrigin.Begin);
                return reader.TokenType == JsonToken.StartArray;
            }
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