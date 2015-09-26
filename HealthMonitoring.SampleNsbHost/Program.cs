﻿using System;
using System.Collections.Generic;
using HealthMonitoring.Monitors.Nsb3.Messages;
using NServiceBus;
using NServiceBus.Config;
using NServiceBus.Config.ConfigurationSource;
using NServiceBus.Installation.Environments;

namespace HealthMonitoring.SampleNsbHost
{
    class Program
    {
        static void Main(string[] args)
        {
            Configure configure = Configure.With(typeof(GetStatusRequest).Assembly, typeof(Handler).Assembly);
            configure.Log4Net();
            configure.DefineEndpointName("HealthMonitoring.SampleNsbHost");
            configure.DefaultBuilder();
            configure.MsmqTransport();
            configure.InMemorySagaPersister();
            configure.RunTimeoutManagerWithInMemoryPersistence();
            configure.InMemorySubscriptionStorage();
            using (IStartableBus startableBus = configure.UnicastBus().CreateBus())
            {
                IBus bus = startableBus
                    .Start(() => configure.ForInstallationOn<Windows>().Install());
                Console.WriteLine("Press any key to exit");
                Console.ReadKey();
            }

        }
    }

    public class Handler : IHandleMessages<GetStatusRequest>
    {
        private readonly IBus _bus;

        public Handler(IBus bus)
        {
            _bus = bus;
        }

        public void Handle(GetStatusRequest message)
        {
            Console.WriteLine("Received: {0}", message.RequestId);
            var details = new Dictionary<string, string> { { "Machine", "localhost" }, { "Version", "1.0.0.0" } };
            _bus.Reply(new GetStatusResponse { RequestId = message.RequestId, Details = details });
        }
    }

    class ConfigErrorQueue : IProvideConfiguration<MessageForwardingInCaseOfFaultConfig>
    {
        public MessageForwardingInCaseOfFaultConfig GetConfiguration()
        {
            return new MessageForwardingInCaseOfFaultConfig
            {
                ErrorQueue = "error"
            };
        }
    }
}