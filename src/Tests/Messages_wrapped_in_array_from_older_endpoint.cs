using System.IO;
using NServiceBus.MessageInterfaces.MessageMapper.Reflection;
using NServiceBus.Newtonsoft.Json;
using NUnit.Framework;

[TestFixture]
public class Messages_wrapped_in_array_from_older_endpoint
{
    public class SimpleMessage1
    {
        public string PropertyOnMessage1 { get; set; }
    }

    public class SimpleMessage2
    {
        public string PropertyOnMessage2 { get; set; }
    }


    [Test]
    public void Run()
    {
        var messageMapper = new MessageMapper();
        var serializer = new JsonMessageSerializer(messageMapper, null, null, null);
        var jsonWithMultipleMessages = $@"
[
{{
$type: '{nameof(SimpleMessage1)}, NServiceBus.Core.Tests',
PropertyOnMessage1: 'Message1'
}},
{{
$type: '{nameof(SimpleMessage2)}, NServiceBus.Core.Tests',
PropertyOnMessage2: 'Message2'
}}
]";
        using (var stream = new MemoryStream())
        {
            var streamWriter = new StreamWriter(stream);
            streamWriter.Write(jsonWithMultipleMessages);
            streamWriter.Flush();
            stream.Position = 0;
            var result = serializer.Deserialize(stream, new[]
            {
                typeof(SimpleMessage2),
                typeof(SimpleMessage1)
            });

            Assert.AreEqual(2, result.Length);
            Assert.AreEqual("Message1", ((SimpleMessage1) result[0]).PropertyOnMessage1);
            Assert.AreEqual("Message2", ((SimpleMessage2) result[1]).PropertyOnMessage2);
        }
    }
}