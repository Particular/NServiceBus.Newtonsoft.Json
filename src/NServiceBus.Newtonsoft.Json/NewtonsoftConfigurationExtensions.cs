﻿using System;
using System.IO;
using Newtonsoft.Json;
using NServiceBus.Configuration.AdvanceExtensibility;
using NServiceBus.Serialization;
using NServiceBus.Newtonsoft.Json;

namespace NServiceBus
{
    /// <summary>
    /// Extensions for <see cref="EndpointConfiguration"/> to manipulate how messages are serialized via Json.net.
    /// </summary>
    public static class NewtonsoftConfigurationExtensions
    {
        /// <summary>
        /// Configures the <see cref="JsonReader"/> creator of JSON stream.
        /// </summary>
        /// <param name="config">The configuration object</param>
        /// <param name="readerCreator">A delegate that creates a <see cref="JsonReader"/> for a <see cref="Stream"/>.</param>
        public static void ReaderCreator(this SerializationExtensions<NewtonsoftSerializer> config, Func<Stream, JsonReader> readerCreator)
        {
            Guard.AgainstNull(config, "config");
            Guard.AgainstNull(readerCreator, "readerCreator");
            config.GetSettings().Set("NServiceBus.Newtonsoft.Json.ReaderCreator", readerCreator);
        }

        /// <summary>
        /// Configures the <see cref="JsonWriter"/> creator of JSON stream.
        /// </summary>
        /// <param name="config">The configuration object.</param>
        /// <param name="writerCreator">A delegate that creates a <see cref="JsonWriter"/> for a <see cref="Stream"/>.</param>
        public static void WriterCreator(this SerializationExtensions<NewtonsoftSerializer> config, Func<Stream, JsonWriter> writerCreator)
        {
            Guard.AgainstNull(config, "config");
            Guard.AgainstNull(writerCreator, "writerCreator");
            config.GetSettings().Set("NServiceBus.Newtonsoft.Json.WriterCreator", writerCreator);
        }

        /// <summary>
        /// Configures the <see cref="JsonSerializerSettings"/> to use.
        /// </summary>
        /// <param name="config">The configuration object.</param>
        /// <param name="settings">The <see cref="JsonSerializerSettings"/> to use.</param>
        public static void Settings(this SerializationExtensions<NewtonsoftSerializer> config, JsonSerializerSettings settings)
        {
            Guard.AgainstNull(config, "config");
            Guard.AgainstNull(settings, "settings");
            config.GetSettings().Set("NServiceBus.Newtonsoft.Json.Settings", settings);
        }
    }
}