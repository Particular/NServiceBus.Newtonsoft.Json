using System.IO;
using NServiceBus.MessageInterfaces.MessageMapper.Reflection;
using NServiceBus.Newtonsoft.Json;
using NUnit.Framework;

[TestFixture]
public class With_interface_without_wrapping
{
    [Test]
    [TestCase(Newtonsoft.Json.TypeNameHandling.Auto)]
    [TestCase(Newtonsoft.Json.TypeNameHandling.None)]
    public void Run(Newtonsoft.Json.TypeNameHandling typeNameHandling)
    {
        var messageMapper = new MessageMapper();
        var serializer = new JsonMessageSerializer(messageMapper, null, null, null, null, typeNameHandling);
        var message = new SuperMessage { SomeProperty = "John" };
        using (var stream = new MemoryStream())
        {
            serializer.Serialize(message, stream);

            stream.Position = 0;

            var result = (SuperMessage)serializer.Deserialize(stream, new[] { typeof(SuperMessage), typeof(IMyEvent) })[0];

            Assert.AreEqual("John", result.SomeProperty);
        }
    }

    public class SuperMessage : IMyEvent
    {
        public string SomeProperty { get; set; }
    }
    public interface IMyEvent
    {
    }
}