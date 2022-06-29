#pragma warning disable 1591

namespace NServiceBus
{
    using System;
    using System.IO;
    using global::Newtonsoft.Json;
    using NServiceBus.Serialization;
    using NServiceBus.MessageInterfaces;
    using NServiceBus.Settings;

    [ObsoleteEx(
    TreatAsErrorFromVersion = "3.0",
    RemoveInVersion = "4.0",
    ReplacementTypeOrMember = nameof(NewtonsoftJsonSerializer))]
    public class NewtonsoftSerializer : SerializationDefinition
    {
        public override Func<IMessageMapper, IMessageSerializer> Configure(IReadOnlySettings settings)
        {
            throw new NotImplementedException();
        }
    }

    public static partial class NewtonsoftConfigurationExtensions
    {
        [ObsoleteEx(
        Message = "Use the equivalent method on NewtonsoftJsonSerializer instead.",
        TreatAsErrorFromVersion = "3.0",
        RemoveInVersion = "4.0")]
        public static void ReaderCreator(this SerializationExtensions<NewtonsoftSerializer> config, Func<Stream, JsonReader> readerCreator)
        {
            throw new NotImplementedException();
        }

        [ObsoleteEx(
        Message = "Use the equivalent method on NewtonsoftJsonSerializer instead.",
        TreatAsErrorFromVersion = "3.0",
        RemoveInVersion = "4.0")]
        public static void WriterCreator(this SerializationExtensions<NewtonsoftSerializer> config, Func<Stream, JsonWriter> writerCreator)
        {
            throw new NotImplementedException();
        }

        [ObsoleteEx(
        Message = "Use the equivalent method on NewtonsoftJsonSerializer instead.",
        TreatAsErrorFromVersion = "3.0",
        RemoveInVersion = "4.0")]
        public static void Settings(this SerializationExtensions<NewtonsoftSerializer> config, JsonSerializerSettings settings)
        {
            throw new NotImplementedException();
        }

        [ObsoleteEx(
        Message = "Use the equivalent method on NewtonsoftJsonSerializer instead.",
        TreatAsErrorFromVersion = "3.0",
        RemoveInVersion = "4.0")]
        public static void ContentTypeKey(this SerializationExtensions<NewtonsoftSerializer> config, string contentTypeKey)
        {
            throw new NotImplementedException();
        }
    }
}