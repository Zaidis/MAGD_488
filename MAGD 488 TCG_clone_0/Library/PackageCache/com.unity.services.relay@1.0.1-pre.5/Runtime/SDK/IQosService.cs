using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Relay.Models;

namespace Unity.Services.Relay
{
    internal interface IQosService
    {
        /// <summary>
        /// Sorts the given regions for best possible QoS.
        /// </summary>
        /// <param name="regions">The regions to sort.</param>
        /// <returns>Sorted list of regions, ordered for best possible QoS</returns>
        Task<List<Region>> OrderRegionsByQoSAsync(List<Region> regions);
    }
}
