using System.IO;
using System.Text;
using NServiceBus.MessageInterfaces.MessageMapper.Reflection;
using NServiceBus.Newtonsoft.Json;
using NUnit.Framework;

[TestFixture]
public class With_UTF8_BOM
{

    [Test]
    public void Run()
    {
        var messageMapper = new MessageMapper();
        var serializer = new JsonMessageSerializer(messageMapper, null, null, null, null);

        var serialized = new UTF8Encoding(true).GetBytes($"{{\"{nameof(SimpleMessage.SomeProperty)}\":\"John\"}}");

        using (var stream = new MemoryStream(serialized))
        {
            stream.Position = 0;

            var result = (SimpleMessage)serializer.Deserialize(stream, new[] { typeof(SimpleMessage) })[0];

            Assert.AreEqual("John", result.SomeProperty);
        }
    }
    public class SimpleMessage
    {
        public string SomeProperty { get; set; }
    }

}