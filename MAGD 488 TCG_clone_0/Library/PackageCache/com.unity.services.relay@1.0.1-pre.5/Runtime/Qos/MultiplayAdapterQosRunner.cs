using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Jobs;
using Unity.Networking.QoS;
using Unity.Services.Relay.Qos.Models;
using UnityEngine;

namespace Unity.Services.Relay.Qos
{
    // MultiplayAdapterQosRunner compiles only when the ucg QoS package is installed.
    internal class MultiplayAdapterQosRunner : IQosRunner
    {
        // Helper method to covert a Discovery service QoS server model to the UCG QoS package server model.
        // the main difference is the 'address' and 'port' fields vs a 'endpoint' field.
        // If the 'endpoint' field cannot be parsed, this will return a null struct.
        private static QosServer? ToUcgFormat(QoSServer server)
        {
            if (!Uri.TryCreate($"udp://{server.Endpoints[0]}", UriKind.Absolute, out var uri))
            {
                Debug.Log("Could not create address from endpoint");
                return null;
            }

            return new QosServer
            {
                locationid = 0,
                regionid = server.Region,
                ipv4 = uri.Host,
                ipv6 = null,
                port = Convert.ToUInt16(uri.Port),
                BackoffUntilUtc = default
            };
        }
        private static List<QosResult> ParseResults(IEnumerable<Unity.Networking.QoS.QosResult> ucgResults, IEnumerable<QoSServer> servers)
        {
            var results = new List<QosResult>();
            using var enr = servers.GetEnumerator();
            foreach (Unity.Networking.QoS.QosResult r in ucgResults)
            {
                enr.MoveNext();
                results.Add(new QosResult
                {
                    Region = enr.Current.Region, // note: the results from ucg do not contain the region, so we assume they're in the same order as the servers list to populate the field.
                    AverageLatencyMs = (int) r.AverageLatencyMs,
                    PacketLossPercent = r.PacketLoss
                });
            }

            return results;
        }

        // MeasureQos will return an empty list if an error occurs
        async Task<IList<QosResult>> IQosRunner.MeasureQosAsync(IList<QoSServer> servers)
        {
            var convertedServers = servers.Select(ToUcgFormat)
                .Where(s => s != null)
                .Select(s => s.Value)
                .ToList();

            var title = "QoS request from Relay";
            var job = new QosJob(convertedServers, title);
            
            var handle = job.Schedule();
            while (!handle.IsCompleted)
            {
                await Task.Yield();
            }
            handle.Complete();

            var results = new List<QosResult>();
            if (servers.Count() == job.QosResults.Count())
            { 
                results = ParseResults(job.QosResults, servers);
            }

            job.QosResults.Dispose();
            job.Dispose();
            return results;
        }
    }
}
