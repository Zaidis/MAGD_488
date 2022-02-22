using System.Runtime.CompilerServices;
#if USE_QOS
using Unity.Services.Relay.Qos;
#endif

[assembly: InternalsVisibleTo("Unity.Services.Relay.Tests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace Unity.Services.Relay
{
    /// <summary>
    /// The entry class of the Relay Allocations Service enables clients to connect to relay servers. Once connected, they are able to communicate with each other, via the relay servers, using the bespoke relay binary protocol.
    /// </summary>
    public static class Relay
    {
        private static IRelayServiceSDK service;
        
        private static readonly Configuration allocationsApiConfiguration;
#if USE_QOS
        private static readonly Qos.Configuration qosDiscoveryApiConfiguration;
#endif

        static Relay()
        {
#if AUTHENTICATION_TESTING_STAGING_UAS
            allocationsApiConfiguration = new Configuration("https://relay-allocations-stg.services.api.unity.com", 10, 4, null);
#if USE_QOS
            qosDiscoveryApiConfiguration = new Qos.Configuration("https://qos-discovery-stg.services.api.unity.com", 10, 4, null);
#endif // USE_QOS
#else // AUTHENTICATION_TESTING_STAGING_UAS
            allocationsApiConfiguration = new Configuration("https://relay-allocations.services.api.unity.com", 10, 4, null);
#if USE_QOS
            qosDiscoveryApiConfiguration = new Qos.Configuration("https://qos-discovery.services.api.unity.com", 10, 4, null);
#endif // USE_QOS
#endif // AUTHENTICATION_TESTING_STAGING_UAS
        }

        /// <summary>
        /// A static instance of the Relay Allocation Client.
        /// </summary>
        public static IRelayServiceSDK Instance
        {
            get
            {
                if (service != null) return service;

#if USE_QOS
                IQosService qosService = new RelayQosService(QosDiscoveryService.Instance.QosDiscoveryApi, qosDiscoveryApiConfiguration, new MultiplayAdapterQosRunner());
#else
                IQosService qosService = null;
#endif
                service = new WrappedRelayService(RelayService.Instance.AllocationsApi, allocationsApiConfiguration,
                    qosService);

                return service;
            }
        }
    }
}
