﻿using System.IO;
using NServiceBus;
using NServiceBus.MessageInterfaces.MessageMapper.Reflection;
using NServiceBus.Newtonsoft.Json;
using NUnit.Framework;
using Particular.Approvals;
// Databus is obsolete and this test needs to be refactored to use the new data bus API or removed. For now
// we are suppressing the obsoletion warning it to unblock the build.
#pragma warning disable CS0618 // Type or member is obsolete
[TestFixture]
class When_serializing_a_message_with_databus_property
{
    [Test]
    public void Should_only_write_key_and_hasValue()
    {
        var messageMapper = new MessageMapper();
        var serializer = new JsonMessageSerializer(messageMapper, null, null, null, null);
        var message = new Message { Property = new DataBusProperty<byte[]>([1, 2, 3, 4]) { Key = "key" } };

        using var stream = new MemoryStream();
        serializer.Serialize(message, stream);

        stream.Position = 0;
        var result = new StreamReader(stream).ReadToEnd();

        Approver.Verify(result);
    }

    class Message
    {
        public DataBusProperty<byte[]> Property { get; set; }
    }
}
#pragma warning restore CS0618
