﻿namespace NServiceBus
{
    using System;
    using NServiceBus.MessageInterfaces;
    using NServiceBus.Newtonsoft.Json;
    using NServiceBus.Serialization;
    using NServiceBus.Settings;

    /// <summary>
    /// Enables Newtonsoft Json serialization.
    /// </summary>
    public class NewtonsoftJsonSerializer : SerializationDefinition
    {
        /// <summary>
        /// Provides a factory method for building a message serializer.
        /// </summary>
        public override Func<IMessageMapper, IMessageSerializer> Configure(IReadOnlySettings settings)
        {
            return mapper =>
            {
                var readerCreator = settings.GetReaderCreator();
                var writerCreator = settings.GetWriterCreator();
                var serializerSettings = settings.GetSettings();
                var contentTypeKey = settings.GetContentTypeKey();
                return new JsonMessageSerializer(mapper, readerCreator, writerCreator, serializerSettings, contentTypeKey);
            };
        }

    }
}