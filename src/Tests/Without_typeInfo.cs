﻿using System.IO;
using NServiceBus.MessageInterfaces.MessageMapper.Reflection;
using NServiceBus.Newtonsoft.Json;
using NUnit.Framework;

[TestFixture]
public class Without_typeInfo
{

    [Test]
    [TestCase(Newtonsoft.Json.TypeNameHandling.Auto)]
    [TestCase(Newtonsoft.Json.TypeNameHandling.None)]
    public void Run(Newtonsoft.Json.TypeNameHandling typeNameHandling)
    {
        var messageMapper = new MessageMapper();
        var serializer = new JsonMessageSerializer(messageMapper, null, null, null, null, typeNameHandling);
        var message = new SimpleMessage();
        using (var stream = new MemoryStream())
        {
            serializer.Serialize(message, stream);

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