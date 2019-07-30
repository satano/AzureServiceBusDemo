using GreenPipes;
using Kros.MassTransit.AzureServiceBus;
using MassTransit;
using MassTransit.Pipeline;
using MassTransit.Transports;
using System;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extensions for registering services to the DI container.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds MassTransit fluent configurator for Azure service bus.
        /// </summary>
        /// <param name="services">DI container.</param>
        /// <param name="connectionString">Connection string to Azure service bus.</param>
        /// <param name="tokenTimeToLive">TTL for Azure service bus token.</param>
        /// <param name="busCfg">Service bus configurator.</param>
        /// <returns>MassTransit fluent configuration for Azure service bus.</returns>
        public static IServiceCollection AddMassTransitForAzure(
            this IServiceCollection services,
            string connectionString,
            TimeSpan tokenTimeToLive,
            Action<IMassTransitForAzureBuilder> busCfg = null)
        {
            var builder = new MassTransitForAzureBuilder(connectionString, tokenTimeToLive);
            busCfg?.Invoke(builder);

            IBusControl bus = Task.Run(async () => await builder.Build()).Result;
            services.AddSingleton(bus);
            services.AddSingleton<IBus>(bus);
            services.AddSingleton<IPublishEndpoint>(bus);
            services.AddSingleton<IPublishObserverConnector>(bus);
            services.AddSingleton<ISendEndpointProvider>(bus);
            services.AddSingleton<ISendObserverConnector>(bus);
            services.AddSingleton<IConsumePipeConnector>(bus);
            services.AddSingleton<IRequestPipeConnector>(bus);
            services.AddSingleton<IConsumeMessageObserverConnector>(bus);
            services.AddSingleton<IConsumeObserverConnector>(bus);
            services.AddSingleton<IReceiveObserverConnector>(bus);
            services.AddSingleton<IReceiveEndpointObserverConnector>(bus);
            services.AddSingleton<IProbeSite>(bus);

            return services;
        }

        /// <summary>
        /// Adds MassTransit fluent configurator for Azure service bus.
        /// </summary>
        /// <param name="services">DI container.</param>
        /// <param name="connectionString">Connection string to Azure service bus.</param>
        /// <param name="busCfg">Service bus configurator.</param>
        /// <returns>MassTransit fluent configuration for Azure service bus.</returns>
        public static IServiceCollection AddMassTransitForAzure(
            this IServiceCollection services,
            string connectionString,
            Action<IMassTransitForAzureBuilder> busCfg = null)
            => services.AddMassTransitForAzure(connectionString, MassTransitForAzureBuilder.DefaultTokenTimeToLive, busCfg);
    }
}
