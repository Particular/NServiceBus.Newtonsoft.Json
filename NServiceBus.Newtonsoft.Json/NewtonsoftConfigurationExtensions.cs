using System;
using System.IO;
using Newtonsoft.Json;
using NServiceBus.Configuration.AdvanceExtensibility;
using NServiceBus.Newtonsoft.Json;
using NServiceBus.Serialization;

namespace NServiceBus
{
    public static class NewtonsoftConfigurationExtensions
    {
        /// <summary>
        /// Configures the <see cref="JsonReader"/> creator of JSON stream.
        /// </summary>
        /// <param name="config">The configuration object</param>
        /// <param name="readerCreator">A delegate that creates a <see cref="JsonReader"/> for a <see cref="Stream"/>.</param>
        /// <example >
        /// var streamReader = new StreamReader(stream, Encoding.UTF8);
        /// return new JsonTextReader(streamReader);
        /// </example>
        public static void ReaderCreator(this SerializationExtentions<JsonSerializer> config, Func<Stream, JsonReader> readerCreator)
        {
            if (readerCreator == null)
            {
                throw new ArgumentNullException("readerCreator");
            }
            config.GetSettings()
                .SetProperty<JsonMessageSerializer>(s => s.ReaderCreator, readerCreator);
        }

        /// <summary>
        /// Configures the <see cref="JsonWriter"/> creator of JSON stream.
        /// </summary>
        /// <param name="config">The configuration object.</param>
        /// <param name="writerCreator">A delegate that creates a <see cref="JsonWriter"/> for a <see cref="Stream"/>.</param>
        /// <example >
        /// var streamWriter = new StreamWriter(stream, Encoding.UTF8);
        /// return new JsonTextWriter(streamWriter)
        /// {
        ///     Formatting = Formatting.None
        /// };
        /// </example>
        public static void WriterCreator(this SerializationExtentions<JsonSerializer> config, Func<Stream, JsonWriter> writerCreator)
        {
            if (writerCreator == null)
            {
                throw new ArgumentNullException("writerCreator");
            }
            config.GetSettings()
                .SetProperty<JsonMessageSerializer>(s => s.WriterCreator, writerCreator);
        }

        /// <summary>
        /// Configures the <see cref="JsonSerializerSettings"/> to use.
        /// </summary>
        /// <param name="config">The configuration object.</param>
        /// <param name="settings">The <see cref="JsonSerializerSettings"/> to use.</param>
        /// <example >
        /// new JsonSerializerSettings
        ///               {
        ///                   TypeNameHandling = TypeNameHandling.Auto,
        ///                   Converters =
        ///                   {
        ///                       new IsoDateTimeConverter
        ///                       {
        ///                           DateTimeStyles = DateTimeStyles.RoundtripKind
        ///                       }
        ///                   }
        ///               }
        /// </example>
        public static void Settings(this SerializationExtentions<JsonSerializer> config, JsonSerializerSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }
            config.GetSettings()
                .SetProperty<JsonMessageSerializer>(s => s.Settings, settings);
        }
    }
}