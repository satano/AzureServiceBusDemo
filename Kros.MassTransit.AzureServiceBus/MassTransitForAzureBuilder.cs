using Kros.MassTransit.AzureServiceBus.Endpoints;
using Kros.Utils;
using MassTransit;
using MassTransit.Azure.ServiceBus.Core;
using MassTransit.ConsumeConfigurators;
using Microsoft.Azure.ServiceBus;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kros.MassTransit.AzureServiceBus
{
    /// <summary>
    /// Builder for Azure service bus via MassTransit.
    /// </summary>
    public class MassTransitForAzureBuilder : IMassTransitForAzureBuilder, IBusConsumerBuilder
    {
        #region Attributes

        /// <summary>
        /// Connection string to Azure service bus.
        /// </summary>
        private string _connectionString;
        /// <summary>
        /// TTL for Azure service bus token.
        /// </summary>
        private TimeSpan _tokenTimeToLive;

        /// <summary>
        /// Delegate to configure service bus.
        /// </summary>
        private Action<IServiceBusBusFactoryConfigurator, IServiceBusHost> _busConfigurator;
        /// <summary>
        /// List of service bus endpoints.
        /// </summary>
        private List<Endpoint> _endpoints;
        /// <summary>
        /// Currently configured endpoint.
        /// </summary>
        private Endpoint _currentEndpoint;

        #endregion

        #region Properties

        /// <summary>
        /// Default TTL for Azure service bus token.
        /// </summary>
        public static TimeSpan DefaultTokenTimeToLive { get; } = TimeSpan.FromMinutes(1);

        #endregion

        #region Constructors

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="connectionString">Connection string to Azure service bus.</param>
        public MassTransitForAzureBuilder(string connectionString) : this(connectionString, DefaultTokenTimeToLive)
        { }

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="connectionString">Connection string to Azure service bus.</param>
        /// <param name="tokenTimeToLive">TTL for Azure service bus token.</param>
        public MassTransitForAzureBuilder(string connectionString, TimeSpan tokenTimeToLive)
        {
            _connectionString = Check.NotNullOrWhiteSpace(connectionString, nameof(connectionString));
            _tokenTimeToLive = Check.GreaterThan(tokenTimeToLive, TimeSpan.Zero, nameof(tokenTimeToLive));
            _endpoints = new List<Endpoint>();
        }

        #endregion

        #region Config

        /// <inheritdoc />
        public IMassTransitForAzureBuilder ConfigureServiceBusFactory(
            Action<IServiceBusBusFactoryConfigurator, IServiceBusHost> configurator = null)
        {
            _busConfigurator = configurator;
            return this;
        }

        #endregion

        #region Endpoints

        /// <inheritdoc />
        public IBusConsumerBuilder ConfigureQueue(string queueName, Action<IServiceBusReceiveEndpointConfigurator> configurator)
        {
            _currentEndpoint = new ReceiveEndpoint(queueName, configurator);
            _endpoints.Add(_currentEndpoint);

            return this;
        }

        /// <inheritdoc />
        public IBusConsumerBuilder ConfigureSubscription<T>(
            string subscriptionName,
            Action<IServiceBusSubscriptionEndpointConfigurator> configurator) where T : class
        {
            _currentEndpoint = new SubscriptionEndpoint<T>(subscriptionName, configurator);
            _endpoints.Add(_currentEndpoint);

            return this;
        }

        /// <inheritdoc />
        public IBusConsumerBuilder ConfigureQueue(string queueName)
            => ConfigureQueue(queueName, config => { });

        /// <inheritdoc />
        public IBusConsumerBuilder ConfigureSubscription<T>(string subscriptionName) where T : class
            => ConfigureSubscription<T>(subscriptionName, config => { });

        #endregion

        #region Consumers

        /// <inheritdoc />
        public IBusConsumerBuilder AddConsumer<TConsumer>(
            IServiceProvider provider,
            Action<IConsumerConfigurator<TConsumer>> configure = null) where TConsumer : class, IConsumer
        {
            _currentEndpoint.AddConsumerWithDependencies(provider, configure);
            return this;
        }

        /// <inheritdoc />
        public IBusConsumerBuilder AddConsumer<TConsumer>(Action<IConsumerConfigurator<TConsumer>> configure = null)
            where TConsumer : class, IConsumer, new()
        {
            _currentEndpoint.AddConsumer(configure);
            return this;
        }

        /// <inheritdoc />
        public IBusConsumerBuilder AddConsumer<T>(MessageHandler<T> handler) where T : class
        {
            _currentEndpoint.AddConsumer(handler);
            return this;
        }

        #endregion

        #region Build

        /// <inheritdoc />
        public async Task<IBusControl> Build()
        {
            IBusControl bus = Bus.Factory.CreateUsingAzureServiceBus(busCfg =>
            {
                IServiceBusHost host = CreateServiceHost(busCfg);

                ConfigureServiceBus(busCfg, host);
                AddEndpoints(busCfg, host);
            });

            await bus.StartAsync();

            return bus;
        }

        /// <summary>
        /// Creates service bus host.
        /// </summary>
        /// <param name="busCfg">Service bus configuration.</param>
        /// <returns>Service bus host.</returns>
        private IServiceBusHost CreateServiceHost(IServiceBusBusFactoryConfigurator busCfg)
        {
            var cstrBuilder = new ServiceBusConnectionStringBuilder(_connectionString);

            return busCfg.Host(_connectionString, hostCfg =>
            {
                hostCfg.SharedAccessSignature(sasCfg =>
                {
                    sasCfg.KeyName = cstrBuilder.SasKeyName;
                    sasCfg.SharedAccessKey = cstrBuilder.SasKey;
                    sasCfg.TokenTimeToLive = _tokenTimeToLive;
                });
            });
        }

        /// <summary>
        /// Configures service bus.
        /// </summary>
        /// <param name="busCfg">Service bus configuration.</param>
        /// <param name="host">Service bus host.</param>
        private void ConfigureServiceBus(IServiceBusBusFactoryConfigurator busCfg, IServiceBusHost host)
        {
            busCfg.UseJsonSerializer();
            busCfg.DefaultMessageTimeToLive = ConfigDefaults.MessageTimeToLive;
            busCfg.EnableDeadLetteringOnMessageExpiration = ConfigDefaults.EnableDeadLetteringOnMessageExpiration;
            busCfg.LockDuration = ConfigDefaults.LockDuration;
            busCfg.AutoDeleteOnIdle = ConfigDefaults.AutoDeleteOnIdle;
            busCfg.MaxDeliveryCount = ConfigDefaults.MaxDeliveryCount;
            busCfg.EnableDuplicateDetection(ConfigDefaults.DuplicateDetectionWindow);

            _busConfigurator?.Invoke(busCfg, host);
        }

        /// <summary>
        /// Adds endpoints to service bus.
        /// </summary>
        /// <param name="busCfg">Service bus configuration.</param>
        /// <param name="host">Service bus host.</param>
        private void AddEndpoints(IServiceBusBusFactoryConfigurator busCfg, IServiceBusHost host)
        {
            foreach (Endpoint endpoint in _endpoints)
            {
                endpoint.SetEndpoint(busCfg, host);
            }
        }

        #endregion
    }
}
