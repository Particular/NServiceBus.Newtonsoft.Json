using System.IO;
using NServiceBus.MessageInterfaces.MessageMapper.Reflection;
using NServiceBus.Newtonsoft.Json;
using NUnit.Framework;

[TestFixture]
public class Without_typeInfo
{

    [Test]
    public void Run()
    {
        var messageMapper = new MessageMapper();
        var serializer = new JsonMessageSerializer(messageMapper, null, null, null, null);
        var message = new SimpleMessage();
        using (var stream = new MemoryStream())
        {
            serializer.Serialize(message, stream);

            stream.Position = 0;
            var result = new StreamReader(stream).ReadToEnd();

            Assert.That(result, Does.Not.Contain("$type"), result);
        }
    }
    public class SimpleMessage
    {
        public string SomeProperty { get; set; }
    }

}