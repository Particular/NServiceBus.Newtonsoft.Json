using System;
using NServiceBus;

class MyMessage:IMessage
{
    public DateTime DateSend { get; set; }
}
