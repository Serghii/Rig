using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Rig
{
public class XmrHrate
    {
        private static readonly string BTCPriceUrl = "https://api.coindesk.com/v1/bpi/currentprice.json";
        private static string balance ;//= "https://api.nicehash.com/api?method=balance&id=310282&key=a60ecb26-d1e0-575d-99ef-12fbf76ac62c";

        private static readonly string XMRJsonUrl = "http://localhost:8080/api.json";
        private static readonly string XMRStakJsonUrl = "http://localhost:4001/api.json";
        private static readonly string ethminer0 = "http://localhost:4002/api.json";
        private static readonly string Zec0 = "http://localhost:4003/api.json";
        private static readonly string Zec1 = "http://localhost:4004/api.json";
        private static readonly string Zec2 = "http://localhost:4005/api.json";
        private static readonly string Equihash0 = "http://localhost:4006/api.json";
        private static readonly string Equihash1 = "http://localhost:4058/api.json";

        private static readonly string[] ports =
            {XMRJsonUrl, XMRStakJsonUrl, ethminer0, Zec0, Zec1, Zec2, Equihash0, Equihash1};
       
        public float GetBalance
        {
            get
            {
                try
                {
                    using (var client = new WebClient())
                    {
                        string json = string.Empty;
                        balance = balance ?? GetBalanceUrl();
                        if (string.IsNullOrEmpty(balance))
                        {
                            throw null;
                        }
                        try
                        {
                            json = client.DownloadString(balance);
                        }
                        catch (Exception e)
                        {
                        }

                        var ping = JsonConvert.DeserializeObject<Balance>(json);
                        float result;
                        return float.TryParse(ping.result.balance_confirmed, NumberStyles.Any, CultureInfo.InvariantCulture, out result) ? result : -1;
                    }
                }
                catch (Exception e)
                {
                    RigEx.WriteLineColors($"Balance Error:{e.Message}".AddTimeStamp(), ConsoleColor.DarkGray);
                    return -1;
                }
            }
        }

        public float GetBTCPrice
        {
            get
            {
                try
                {
                    using (var client = new WebClient())
                    {
                        string json = string.Empty;
                            try
                            {
                                json = client.DownloadString(BTCPriceUrl);
                            }
                            catch (Exception e) { }

                        var btc = JsonConvert.DeserializeObject<BTCPrice>(json);
                        return btc.bpi.USD.rate_float;
                    }
                }
                catch (Exception e)
                {
                    RigEx.WriteLineColors($"GetBTCPrice Error:{e.Message}".AddTimeStamp(), ConsoleColor.DarkGray);
                    return -1;
                }
            }
        }
        public float GetHashRateAll
        {
            get
            {
                try
                {
                    using (var client = new WebClient())
                    {
                        string json = string.Empty;
                        for (int i = 0; i < 5; i++)
                        for (int j = 0; j < 9; j++)
                        {
                            try
                            {
                                json = client.DownloadString($"http://localhost:40{i}{j}/api.json");
                            }
                            catch (Exception e)
                            {}

                            if (!string.IsNullOrEmpty(json))
                                break;
                        }
                        foreach (string port in ports)
                        {
                            try
                            {
                                json = client.DownloadString(port);
                            }
                            catch (Exception e){}
                            
                            if (!string.IsNullOrEmpty(json))
                                break;
                        }
                        
                        Ping ping = JsonConvert.DeserializeObject<Ping>(json);
                        return ping.hashrate.total[2] ?? ping.hashrate.total[1] ?? ping.hashrate.total[0] ?? -1 ;
                    }
                }
                catch (Exception e)
                {
                    RigEx.WriteLineColors($"XMR {e.Message}".AddTimeStamp(),ConsoleColor.DarkGray);
                    return -1;
                }
            }
        }
        
        public float GetHashRate
        {
            get
            {
                try
                {
                    using (var client = new WebClient())
                    {
                        string json = string.Empty;
                        json = client.DownloadString(XMRJsonUrl);
                        Ping ping = JsonConvert.DeserializeObject<Ping>(json);
                        return ping.hashrate.total[2] ?? ping.hashrate.total[1] ?? ping.hashrate.total[0] ?? -1;
                    }
                }
                catch (Exception e)
                {
                    RigEx.WriteLineColors($"XMR {e.Message}".AddTimeStamp(), ConsoleColor.DarkGray);
                    return -1;
                }
            }
        }
        private string GetBalanceUrl()
        {
            RigEx.NiceHashKey niceKey = RigEx.GetNiceHashKey();
            return niceKey == null ? String.Empty : $"https://api.nicehash.com/api?method=balance&id={niceKey.id}&key={niceKey.keyRead}";
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
        #region nicehash btc wallet
        public class Result
        {
            public string balance_pending { get; set; }
            public string balance_confirmed { get; set; }
        }

        public class Balance
        {
            public Result result { get; set; }
            public string method { get; set; }
        }
        #endregion
        #region btc Price
        public class Time
        {
            public string updated { get; set; }
            public DateTime updatedISO { get; set; }
            public string updateduk { get; set; }
        }

        public class USD
        {
            public string code { get; set; }
            public string symbol { get; set; }
            public string rate { get; set; }
            public string description { get; set; }
            public float rate_float { get; set; }
        }

        public class GBP
        {
            public string code { get; set; }
            public string symbol { get; set; }
            public string rate { get; set; }
            public string description { get; set; }
            public float rate_float { get; set; }
        }

        public class EUR
        {
            public string code { get; set; }
            public string symbol { get; set; }
            public string rate { get; set; }
            public string description { get; set; }
            public float rate_float { get; set; }
        }

        public class Bpi
        {
            public USD USD { get; set; }
            public GBP GBP { get; set; }
            public EUR EUR { get; set; }
        }

        public class BTCPrice
        {
            public Time time { get; set; }
            public string disclaimer { get; set; }
            public string chartName { get; set; }
            public Bpi bpi { get; set; }
        }
        #endregion
    }
}
