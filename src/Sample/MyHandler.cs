using System;
using System.Threading.Tasks;
using NServiceBus;

class MyHandler : IHandleMessages<MyMessage>
{
    public Task Handle(MyMessage message, IMessageHandlerContext context)
    {
        Console.Write("Hello from MyHandler " + message.DateSend);
        return Task.FromResult(0);
    }
}