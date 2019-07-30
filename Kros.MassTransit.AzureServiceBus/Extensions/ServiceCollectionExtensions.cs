using Kros.MassTransit.AzureServiceBus;
using MassTransit;
using System;
using System.Reflection;
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
            services.AddMassTransit(cfg =>
            {
                var assembly = Assembly.GetCallingAssembly();
                cfg.AddConsumersFromNamespaceContaining(assembly.GetType(), type => type is IConsumer);

                var builder = new MassTransitForAzureBuilder(connectionString, tokenTimeToLive);
                busCfg?.Invoke(builder);

                cfg.AddBus(provider => Task.Run(async () => await builder.Build()).Result);
            });

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
