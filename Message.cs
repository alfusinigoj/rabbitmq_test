
namespace RabbitMQ.Spike.Test;

public class Message
{
    public string? SomeText { get; set; }
}

public class Queue1Message : Message
{
}

public class Queue2Message : Message
{
}