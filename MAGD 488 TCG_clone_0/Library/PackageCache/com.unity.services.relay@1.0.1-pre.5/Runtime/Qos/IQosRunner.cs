using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Relay.Qos.Models;

namespace Unity.Services.Relay.Qos
{
    internal struct QosResult
    {
        public string Region;
        public int AverageLatencyMs;
        /// <summary>Percentage of packet loss from 0.0f - 1.0f (0 - 100%).</summary>
        public float PacketLossPercent;
    }

    internal interface IQosRunner
    {
        Task<IList<QosResult>> MeasureQosAsync(IList<QoSServer> servers);
    }
}
