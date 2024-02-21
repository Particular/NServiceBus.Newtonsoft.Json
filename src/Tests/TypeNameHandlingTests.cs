using System.IO;
using NServiceBus.MessageInterfaces.MessageMapper.Reflection;
using NServiceBus.Newtonsoft.Json;
using NUnit.Framework;

[TestFixture]
public class TypeNameHandlingTests
{
    string typeName = $"{typeof(TestMessage).FullName}, {typeof(TestMessage).Assembly.GetName().Name}";

    [Test]
    public void Should_not_recognise_type_with_none()
    {
        var text = $@"
{{
    $type: '{typeName}',
    SomeProperty: 'Value1'
}}";

        using (var stream = new MemoryStream())
        {
            WriteToStream(stream, text);

            var messageMapper = new MessageMapper();
            var serializer = new JsonMessageSerializer(messageMapper, null, null, null, null);
            var result = serializer.Deserialize(stream.ToArray(), []);
            Assert.IsNotInstanceOf<TestMessage>(result[0]);
        }
    }


    [Test]
    public void Should_recognise_type_with_auto()
    {
        var text = $@"
{{
    $type: '{typeName}',
    SomeProperty: 'Value1'
}}";

        using (var stream = new MemoryStream())
        {
            WriteToStream(stream, text);

            var messageMapper = new MessageMapper();
            var settings = new Newtonsoft.Json.JsonSerializerSettings() { TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Auto };
            var serializer = new JsonMessageSerializer(messageMapper, null, null, settings, null);
            var result = serializer.Deserialize(stream.ToArray(), []);
            Assert.That(result[0], Is.TypeOf(typeof(TestMessage)));
            Assert.IsInstanceOf<TestMessage>(result[0]);
        }
    }

    static void WriteToStream(MemoryStream stream, string text)
    {
        var streamWriter = new StreamWriter(stream);
        streamWriter.Write(text);
        streamWriter.Flush();
        stream.Position = 0;
    }

    public class TestMessage
    {
        public string SomeProperty { get; set; }
    }
}