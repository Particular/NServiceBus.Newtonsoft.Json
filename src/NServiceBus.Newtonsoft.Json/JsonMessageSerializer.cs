using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
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
                TypeNameHandling = TypeNameHandling.Auto
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
            using (var writer = writerCreator(stream))
            {
                writer.CloseOutput = false;
                jsonSerializer.Serialize(writer, message);
                writer.Flush();
            }
        }

        public object[] Deserialize(Stream stream, IList<Type> messageTypes)
        {
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