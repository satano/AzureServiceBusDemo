using Kros.Utils;
using MassTransit.Azure.ServiceBus.Core;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Kros.MassTransit.Demo
{
    public static class ServiceCollectionExtensions
    {
        public static void Configure(IServiceCollection services)
        {
            services.AddMassTransitForAzure("connection string", TimeSpan.FromSeconds(10))
                .ConfigureServiceBusFactory((busCfg, host) => { busCfg.LockDuration = TimeSpan.FromSeconds(30); })
                //.ConfigureQueue("queueName1", IServiceBusReceiveEndpointConfigurator => { })
                //    .AddConsumer<TConsumer>()
                //    .AddConsumer<TMessageType>(ConsumeContext => { })
                //.ConfigureQueue("queueName2", endpointCfg => { })
                //    .AddConsumer<TConsumer>()
                //    .AddConsumer<TMessageType>(ConsumeContext => { })
                ;
        }

        public static IMassTransitForAzureBuilder AddMassTransitForAzure(
            this IServiceCollection services,
            string connectionString,
            TimeSpan tokenTimeToLive)
        {
            IMassTransitForAzureBuilder builder = new MassTransitForAzureBuilder(connectionString, tokenTimeToLive);
            return builder;
        }
    }

    public interface IMassTransitForAzureBuilder
    {
        IServiceCollection Build();
        IMassTransitForAzureBuilder ConfigureServiceBusFactory(Action<IServiceBusBusFactoryConfigurator, IServiceBusHost> configurator = null);
    }

    public class MassTransitForAzureBuilder : IMassTransitForAzureBuilder
    {
        public static TimeSpan DefaultTokenTimeToLive { get; } = TimeSpan.FromMinutes(1);

        private readonly string _connectionString;
        private readonly TimeSpan _tokenTimeToLive;

        public MassTransitForAzureBuilder(string connectionString)
            : this(connectionString, DefaultTokenTimeToLive)
        {
        }

        public MassTransitForAzureBuilder(string connectionString, TimeSpan tokenTimeToLive)
        {
            _connectionString = Check.NotNullOrWhiteSpace(connectionString, nameof(connectionString));
            _tokenTimeToLive = Check.GreaterThan(tokenTimeToLive, TimeSpan.Zero, nameof(tokenTimeToLive));
        }

        public IServiceCollection Build() => throw new NotImplementedException();
        public IMassTransitForAzureBuilder ConfigureServiceBusFactory(Action<IServiceBusBusFactoryConfigurator, IServiceBusHost> configurator = null) => throw new NotImplementedException();
    }
}
