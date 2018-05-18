using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Rig
{
public class XmrHrate
    {
        private static readonly string XMRJsonUrl = "http://localhost:8080/api.json";

        public float GetHashRate
        {
            get
            {
                try
                {
                    using (var client = new WebClient())
                    {
                        var json = client.DownloadString(XMRJsonUrl);
                        Ping ping = JsonConvert.DeserializeObject<Ping>(json);
                        return ping.hashrate.total[2] ?? ping.hashrate.total[1] ?? ping.hashrate.total[0] ?? -1 ;
                    }
                }
                catch (Exception e)
                {
                    RigEx.WriteLineColors($"XMR get Hash rate Error: {e.Message}".AddTimeStamp(),ConsoleColor.DarkRed);
                    return -1;
                }
            }
        }

        class Ping
        {

            [JsonProperty("version")]
            public string version { get; set; }

            [JsonProperty("hashrate")]
            public Hashrate hashrate { get; set; }

            [JsonProperty("results")]
            public Results results { get; set; }

            [JsonProperty("connection")]
            public Connection connection { get; set; }
        }

         class Hashrate
        {

            [JsonProperty("threads")]
            public IList<IList<float?>> threads { get; set; }

            [JsonProperty("total")]
            public IList<float?> total { get; set; }

            [JsonProperty("highest")]
            public float? highest { get; set; }
        }

         class Results
        {

            [JsonProperty("diff_current")]
            public int? diff_current { get; set; }

            [JsonProperty("shares_good")]
            public int? shares_good { get; set; }

            [JsonProperty("shares_total")]
            public int? shares_total { get; set; }

            [JsonProperty("avg_time")]
            public float? avg_time { get; set; }

            [JsonProperty("hashes_total")]
            public int? hashes_total { get; set; }

            [JsonProperty("best")]
            public IList<int?> best { get; set; }

            [JsonProperty("error_log")]
            public IList<object> error_log { get; set; }
        }

         class Connection
        {

            [JsonProperty("pool")]
            public string pool { get; set; }

            [JsonProperty("uptime")]
            public int? uptime { get; set; }

            [JsonProperty("ping")]
            public int? ping { get; set; }

            [JsonProperty("error_log")]
            public IList<object> error_log { get; set; }
        }
    }
}
