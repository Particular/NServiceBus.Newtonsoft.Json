using System;
using NServiceBus;

class MyHandler : IHandleMessages<MyMessage>
{
    public void Handle(MyMessage message)
    {
        Console.Write("Hello from MyHandler " + message.DateSend);
    }
}