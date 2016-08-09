using System.IO;
using NServiceBus.MessageInterfaces.MessageMapper.Reflection;
using NServiceBus.Newtonsoft.Json;
using NUnit.Framework;

[TestFixture]
public class Message_without_typeInfo
{

    [Test]
    public void Run()
    {
        var messageMapper = new MessageMapper();
        var serializer = new JsonMessageSerializer(messageMapper, null, null, null, null);
        using (var stream = new MemoryStream())
        {
            serializer.Serialize(new SimpleMessage(), stream);

            stream.Position = 0;
            var result = new StreamReader(stream).ReadToEnd();

            Assert.That(!result.Contains("$type"), result);
        }
    }
    public class SimpleMessage
    {
        public string SomeProperty { get; set; }
    }

}