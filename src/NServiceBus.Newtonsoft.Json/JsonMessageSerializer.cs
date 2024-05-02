namespace NServiceBus.Newtonsoft.Json
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using global::Newtonsoft.Json;
    using Logging;
    using MessageInterfaces;
    using Serialization;
    using NewtonSerializer = global::Newtonsoft.Json.JsonSerializer;

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

            settings ??= new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.None
            };

            settings.Converters.Add(new DataBusPropertyConverter());

            if (settings.TypeNameHandling == TypeNameHandling.Auto)
            {
                log.Warn($"Use of TypeNameHandling.Auto is a potential security vulnerability and it is recommended to use TypeNameHandling.None if possible.");
            }

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
                if(TryGetJsonConverter(message.GetType(), out var converter))
                {
                    converter.WriteJson(writer, message, jsonSerializer);
                }
                else
                {
                    jsonSerializer.Serialize(writer, message);
                }
                writer.Flush();
            }
        }

        public object[] Deserialize(ReadOnlyMemory<byte> body, IList<Type> messageTypes)
        {
            var stream = new ReadOnlyStream(body);

            var isArrayStream = IsArrayStream(stream);

            if (messageTypes.Any())
            {
                return DeserializeMultipleMessageTypes(stream, messageTypes, isArrayStream);
            }

            return new[]
            {
                ReadObject(stream, isArrayStream, typeof(object))
            };
        }

        object ReadObject(Stream stream, bool isArrayStream, Type type)
        {
            using (var reader = readerCreator(stream))
            {
                reader.CloseInput = false;

                if (TryGetJsonConverter(type, out var converter))
                {
                    return converter.ReadJson(reader, type, null, jsonSerializer);
                }
                else if (isArrayStream)
                {
                    var objects = (object[])jsonSerializer.Deserialize(reader, type.MakeArrayType());
                    if (objects.Length > 1)
                    {
                        throw new Exception("Multiple messages in the same stream are not supported.");
                    }
                    return objects[0];
                }

                return jsonSerializer.Deserialize(reader, type);
            }
        }

        public string ContentType { get; }

        bool TryGetJsonConverter(Type type, out JsonConverter converter)
        {
            converter = null!;

            var attribute = type.GetCustomAttribute<JsonConverterAttribute>(false);

            if(attribute != null)
            {
                converter = (JsonConverter)Activator.CreateInstance(attribute.ConverterType, attribute.ConverterParameters)!;
            }
            else
            {
                foreach(var _converter in NewtonSerializer.Converters)
                {
                    if (_converter.CanConvert(type))
                    {
                        converter = _converter;
                        break;
                    }
                }
            }

            return converter != null;
        }
        
        object[] DeserializeMultipleMessageTypes(Stream stream, IList<Type> messageTypes, bool isArrayStream)
        {
            var rootTypes = FindRootTypes(messageTypes).ToList();
            var messages = new object[rootTypes.Count];
            for (var index = 0; index < rootTypes.Count; index++)
            {
                var messageType = rootTypes[index];
                stream.Seek(0, SeekOrigin.Begin);
                messageType = GetMappedType(messageType);
                messages[index] = ReadObject(stream, isArrayStream, messageType);
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

        static ILog log = LogManager.GetLogger<JsonMessageSerializer>();
    }
}
