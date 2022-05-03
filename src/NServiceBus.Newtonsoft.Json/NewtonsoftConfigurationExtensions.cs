namespace NServiceBus
{
    using System;
    using System.IO;
    using global::Newtonsoft.Json;
    using NServiceBus.Configuration.AdvancedExtensibility;
    using NServiceBus.Serialization;
    using NServiceBus.Newtonsoft.Json;
    using NServiceBus.Settings;

    /// <summary>
    /// Extensions for <see cref="SerializationExtensions{T}"/> to manipulate how messages are serialized via Json.net.
    /// </summary>
    public static class NewtonsoftConfigurationExtensions
    {
        /// <summary>
        /// Configures the <see cref="JsonReader"/> creator of JSON stream.
        /// </summary>
        /// <param name="config">The <see cref="SerializationExtensions{T}"/> instance.</param>
        /// <param name="readerCreator">A delegate that creates a <see cref="JsonReader"/> for a <see cref="Stream"/>.</param>
        public static void ReaderCreator(this SerializationExtensions<NewtonsoftSerializer> config, Func<Stream, JsonReader> readerCreator)
        {
            Guard.AgainstNull(config, nameof(config));
            Guard.AgainstNull(readerCreator, nameof(readerCreator));
            config.GetSettings().Set("NServiceBus.Newtonsoft.Json.ReaderCreator", readerCreator);
        }

        internal static Func<Stream, JsonReader> GetReaderCreator(this ReadOnlySettings settings)
        {
            return settings.GetOrDefault<Func<Stream, JsonReader>>("NServiceBus.Newtonsoft.Json.ReaderCreator");
        }

        /// <summary>
        /// Configures the <see cref="JsonWriter"/> creator of JSON stream.
        /// </summary>
        /// <param name="config">The <see cref="SerializationExtensions{T}"/> instance.</param>
        /// <param name="writerCreator">A delegate that creates a <see cref="JsonWriter"/> for a <see cref="Stream"/>.</param>
        public static void WriterCreator(this SerializationExtensions<NewtonsoftSerializer> config, Func<Stream, JsonWriter> writerCreator)
        {
            Guard.AgainstNull(config, nameof(config));
            Guard.AgainstNull(writerCreator, nameof(writerCreator));
            config.GetSettings().Set("NServiceBus.Newtonsoft.Json.WriterCreator", writerCreator);
        }

        internal static Func<Stream, JsonWriter> GetWriterCreator(this ReadOnlySettings settings)
        {
            return settings.GetOrDefault<Func<Stream, JsonWriter>>("NServiceBus.Newtonsoft.Json.WriterCreator");
        }

        /// <summary>
        /// Configures the <see cref="JsonSerializerSettings"/> to use.
        /// </summary>
        /// <param name="config">The <see cref="SerializationExtensions{T}"/> instance.</param>
        /// <param name="settings">The <see cref="JsonSerializerSettings"/> to use.</param>
        public static void Settings(this SerializationExtensions<NewtonsoftSerializer> config, JsonSerializerSettings settings)
        {
            Guard.AgainstNull(config, nameof(config));
            Guard.AgainstNull(settings, nameof(settings));
            config.GetSettings().Set("NServiceBus.Newtonsoft.Json.Settings", settings);
        }

        internal static JsonSerializerSettings GetSettings(this ReadOnlySettings settings)
        {
            return settings.GetOrDefault<JsonSerializerSettings>("NServiceBus.Newtonsoft.Json.Settings");
        }

        /// <summary>
        /// Configures string to use for <see cref="Headers.ContentType"/> headers.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="ContentTypes.Json"/>.
        /// This setting is required when this serializer needs to co-exist with other json serializers.
        /// </remarks>
        /// <param name="config">The <see cref="SerializationExtensions{T}"/> instance.</param>
        /// <param name="contentTypeKey">The content type key to use.</param>
        public static void ContentTypeKey(this SerializationExtensions<NewtonsoftSerializer> config, string contentTypeKey)
        {
            Guard.AgainstNull(config, nameof(config));
            Guard.AgainstNullOrEmpty(contentTypeKey, nameof(contentTypeKey));
            config.GetSettings().Set("NServiceBus.Newtonsoft.Json.ContentTypeKey", contentTypeKey);
        }

        internal static string GetContentTypeKey(this ReadOnlySettings settings)
        {
            return settings.GetOrDefault<string>("NServiceBus.Newtonsoft.Json.ContentTypeKey");
        }
    }
}