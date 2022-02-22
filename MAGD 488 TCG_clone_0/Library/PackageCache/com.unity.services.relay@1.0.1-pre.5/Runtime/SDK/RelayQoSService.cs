#if USE_QOS
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Relay.Models;
using Unity.Services.Relay.Qos;
using Unity.Services.Relay.Qos.Apis.QosDiscovery;
using Unity.Services.Relay.Qos.QosDiscovery;

namespace Unity.Services.Relay
{
    internal class RelayQosService : IQosService
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IQosDiscoveryApiClient _qosDiscoveryApiClient;
        private readonly Qos.Configuration _configuration;
        private readonly IQosRunner _qosRunner;
        private const string Service = "relay";

        internal RelayQosService(IQosDiscoveryApiClient qosDiscoveryApiClient, 
                                 Unity.Services.Relay.Qos.Configuration configuration, 
                                 IQosRunner runner,
                                 IAuthenticationService authenticationService = null)
        {
            _qosDiscoveryApiClient = qosDiscoveryApiClient;
            _configuration = configuration;
            _qosRunner = runner;
            _authenticationService = authenticationService ?? AuthenticationService.Instance;
        }

        /// <inheritdoc/>
        /// Relay implementation of OrderRegionsByQoS, were QoS results are sorted by packet latency and _then_ by
        /// packet loss. E.g. Regions for the following pairs of (Latency,PL): (1,0) (1,1) (1,3) (2,0) (2,1) they will
        /// be sorted in the following manner:
        /// Lat	P/L
        /// 1	0 
        /// 1	1   
        /// 1	3
        /// 2	0
        /// 2	1
        ///
        /// Notice that the third entry (1,3) is put before the (2,1) because it has less latency, even if it has more
        /// packet loss.
        /// In case where no QoS servers can be found, no QoS is performed and an empty list is returned.
        async Task<List<Region>> IQosService.OrderRegionsByQoSAsync(List<Region> regions)
        {
            EnsureSignedIn();
            if (!regions.Any())
            {
                return regions;
            }
            var regionIds = regions.Select(r => r.Id).ToList();
            var httpResp = await _qosDiscoveryApiClient.GetServersAsync(new GetServersRequest(regionIds, Service), _configuration);
            var servers = httpResp.Result.Data.Servers;
            if (!servers.Any())
            {
                return new List<Region>();
            }

            var qosResults = await _qosRunner.MeasureQosAsync(servers);
            return qosResults
                .OrderBy(q => q.AverageLatencyMs)
                .ThenBy(q => q.PacketLossPercent)
                .Select(q => regions.Find(region => region.Id == q.Region))
                .ToList();
        }

        private void EnsureSignedIn()
        {
            if (!_authenticationService.IsSignedIn)
            {
                throw new RelayServiceException(RelayExceptionReason.Unauthorized, "You are not signed in to the Authentication Service. Please sign in.");
            }
        }
    }
}
#endif
