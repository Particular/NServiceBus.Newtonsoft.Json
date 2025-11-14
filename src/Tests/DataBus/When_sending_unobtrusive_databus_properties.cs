namespace NServiceBus.AcceptanceTests.DataBus;

using System;
using System.IO;
using System.Threading.Tasks;
using AcceptanceTesting;
using AcceptanceTesting.Customization;
using ClaimCheck;
using EndpointTemplates;
using MessageMutator;
using NUnit.Framework;

public class When_sending_unobtrusive_databus_properties : NServiceBusAcceptanceTest
{
    [Test]
    public async Task Should_receive_messages_with_largepayload_correctly()
    {
        var payloadToSend = new byte[PayloadSize];

        var context = await Scenario.Define<Context>()
            .WithEndpoint<Sender>(b => b.When(session => session.Send(new MyMessageWithLargePayload
            {
                Payload = payloadToSend
            })))
            .WithEndpoint<Receiver>()
            .Done(c => c.ReceivedPayload != null)
            .Run();

        Assert.That(context.ReceivedPayload, Is.EqualTo(payloadToSend), "The large payload should be marshalled correctly using the databus");
    }

    const int PayloadSize = 500;

    public class Context : ScenarioContext
    {
        public byte[] ReceivedPayload { get; set; }
    }

    public class Sender : EndpointConfigurationBuilder
    {
        public Sender() =>
            EndpointSetup<DefaultServer>(builder =>
            {
                builder.Conventions()
                    .DefiningCommandsAs(t => t.Namespace != null && t.FullName == typeof(MyMessageWithLargePayload).FullName)
                    .DefiningClaimCheckPropertiesAs(t => t.Name.Contains("Payload"));

                var basePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "databus", "sender");
                builder.UseClaimCheck<FileShareClaimCheck, SystemJsonClaimCheckSerializer>().BasePath(basePath);
                builder.UseSerialization<NewtonsoftJsonSerializer>();
                builder.ConfigureRouting().RouteToEndpoint(typeof(MyMessageWithLargePayload), typeof(Receiver));
            });
    }

    public class Receiver : EndpointConfigurationBuilder
    {
        public Receiver() =>
            EndpointSetup<DefaultServer>(builder =>
            {
                builder.Conventions()
                    .DefiningCommandsAs(t => t.Namespace != null && t.FullName == typeof(MyMessageWithLargePayload).FullName)
                    .DefiningClaimCheckPropertiesAs(t => t.Name.Contains("Payload"));

                var basePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "databus", "sender");
                builder.UseClaimCheck<FileShareClaimCheck, SystemJsonClaimCheckSerializer>().BasePath(basePath);
                builder.UseSerialization<NewtonsoftJsonSerializer>();
                builder.RegisterMessageMutator(new Mutator());
            });

        public class MyMessageHandler(Context testContext) : IHandleMessages<MyMessageWithLargePayload>
        {
            public Task Handle(MyMessageWithLargePayload messageWithLargePayload, IMessageHandlerContext context)
            {
                testContext.ReceivedPayload = messageWithLargePayload.Payload;

                return Task.CompletedTask;
            }
        }

        public class Mutator : IMutateIncomingTransportMessages
        {
            public Task MutateIncoming(MutateIncomingTransportMessageContext context)
            {
                if (context.Body.Length > PayloadSize)
                {
                    throw new Exception("The message body is too large, which means the DataBus was not used to transfer the payload.");
                }

                return Task.CompletedTask;
            }
        }
    }

    public class MyMessageWithLargePayload
    {
        public byte[] Payload { get; set; }
    }
}