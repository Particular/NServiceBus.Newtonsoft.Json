using System;
using System.Collections.Generic;
using System.IO;
using ApprovalTests;
using NServiceBus.MessageInterfaces.MessageMapper.Reflection;
using NServiceBus.Newtonsoft.Json;
using NUnit.Framework;

[TestFixture]
public class ArrayTests
{

    [Test]
    public void Should_throw_for_array()
    {
        var xml = @"[{
$type: ""IA, NServiceBus.Core.Tests"",
Data: ""rhNAGU4dr/Qjz6ocAsOs3wk3ZmxHMOg="",
S: ""kalle"",
I: 42,
B: {
}
}, {
$type: ""IA, NServiceBus.Core.Tests"",
Data: ""AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA="",
S: ""kalle"",
I: 42,
B: {
}
}]";

        using (var stream = new MemoryStream())
        {
            var streamWriter = new StreamWriter(stream);
            streamWriter.Write(xml);
            streamWriter.Flush();
            stream.Position = 0;

            var messageMapper = new MessageMapper();
            var serializer = new JsonMessageSerializer(messageMapper, null, null, null, null);
            var exception = Assert.Throws<Exception>(() => serializer.Deserialize(stream, new List<Type>()));
            Approvals.Verify(exception.Message);
        }

    }


}