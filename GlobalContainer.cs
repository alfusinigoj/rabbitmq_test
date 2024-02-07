namespace RabbitMQ.Spike.Test;

public static class Global
{
    public static Dictionary<Type, int> MessageProcessed { get; set; } = new Dictionary<Type, int>();
}