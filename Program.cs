using Microsoft.Extensions.DependencyInjection.Extensions;
using PivotalServices.RabbitMQ.Messaging;
using RabbitMQ.Client;

namespace RabbitMQ.Spike.Test;

public static class Program
{
    public static void Main(string[] args)
    {
        using var host = Host.CreateDefaultBuilder(args)
        .ConfigureServices((hostContext, services) =>
        {
            services.AddRabbitMQ(hostContext.Configuration, cfg =>
            {
                cfg.AddProducer<Message>(bindingExchangeName: "first-exchange",
                    queueName: "publisher-queue",
                    addDeadLetterQueue: false, (queueConfiguration) =>
                    {
                        queueConfiguration.BindingExchangeType = ExchangeType.Topic;
                        queueConfiguration.AdditionalArguments.Add("x-queue-type", "quorum");
                    });

                cfg.AddConsumer<Queue1Message>(bindingExchangeName: "second-exchange",
                    queueName: "queue-one-all-test",
                    addDeadLetterQueue: false, (queueConfiguration) =>
                    {
                        queueConfiguration.BindingExchangeType = ExchangeType.Topic;
                        queueConfiguration.RoutingKeysCsv = "my.*.test";
                        queueConfiguration.AdditionalArguments.Add("x-queue-type", "quorum");
                    });

                cfg.AddConsumer<Queue2Message>(bindingExchangeName: "second-exchange",
                    queueName: "queue-two-all-quorum",
                    addDeadLetterQueue: false, (queueConfiguration) =>
                    {
                        queueConfiguration.BindingExchangeType = ExchangeType.Topic;
                        queueConfiguration.RoutingKeysCsv = "my.quorum.*";
                        queueConfiguration.AdditionalArguments.Add("x-queue-type", "quorum");
                    });
            });

            services.TryAddEnumerable(ServiceDescriptor.Singleton<IHostedService, MessageProcessor>());
            services.TryAddEnumerable(ServiceDescriptor.Singleton<IHostedService, MessagePublisher>());

        }).Build();

        host.Start();
        host.WaitForShutdown();

        foreach (var messageProcessed in Global.MessageProcessed)
        {
            Console.WriteLine($"MessageProcessed: {messageProcessed.Key.Name} - {messageProcessed.Value}");
        }
    }
}