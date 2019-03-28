using System.IO;
using System.Text;
using NServiceBus.MessageInterfaces.MessageMapper.Reflection;
using NServiceBus.Newtonsoft.Json;
using NUnit.Framework;

[TestFixture]
public class No_UTF8_BOM
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

            var result = stream.ToArray();
            var utf8bom = new UTF8Encoding(true).GetPreamble();

            for (var i = 0; i < utf8bom.Length; i++)
            {
                Assert.AreNotEqual(utf8bom[i], result[i]);
            }
        }
    }
    public class SimpleMessage
    {
        public string SomeProperty { get; set; }
    }

}