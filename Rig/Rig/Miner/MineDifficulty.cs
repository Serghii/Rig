using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;

namespace Rig
{
    using MineBlob = Dictionary<string, Dictionary<string, CoinBlob>>;

     class CoinBlob
    {
        [JsonProperty(PropertyName = "id")]
        public int Id;
        [JsonProperty(PropertyName = "tag")]
        public string Tag;
        [JsonProperty(PropertyName = "algorithm")]
        public string Algorithm;
        [JsonProperty(PropertyName = "block_time")]
        public double BlockTime;
        [JsonProperty(PropertyName = "block_reward")]
        public double BlockReward;
        [JsonProperty(PropertyName = "block_reward24")]
        public double BlockReward24;
        [JsonProperty(PropertyName = "last_block")]
        public long LastBlock;
        [JsonProperty(PropertyName = "infopc")]
        public double Difficulty;
        [JsonProperty(PropertyName = "difficulty24")]
        public double Difficulty24;
        [JsonProperty(PropertyName = "nethash")]
        public long NetHash;
        [JsonProperty(PropertyName = "exchange_rate")]
        public double ExchangeRate;
        [JsonProperty(PropertyName = "exchange_rate24")]
        public double ExchangeRate24;
        [JsonProperty(PropertyName = "exchange_rate_vol")]
        public double ExchangeRateVolume;
        [JsonProperty(PropertyName = "exchange_rate_curr")]
        public string ExchangeRateCurrency;
        [JsonProperty(PropertyName = "market_cap")]
        public string MarketCapUsd;
        [JsonProperty(PropertyName = "estimated_rewards")]
        public string EstimatedRewards;
        [JsonProperty(PropertyName = "estimated_rewards24")]
        public string EstimatedRewards24;
        [JsonProperty(PropertyName = "btc_revenue")]
        public string BtcRevenue;
        [JsonProperty(PropertyName = "btc_revenue24")]
        public string BtcRevenue24;
        [JsonProperty(PropertyName = "profitability")]
        public double Profitability;
        [JsonProperty(PropertyName = "profitability24")]
        public double Profitability24;
        [JsonProperty(PropertyName = "lagging")]
        public bool IsLagging;
        [JsonProperty(PropertyName = "timestamp")]
        public long TimeStamp;
    }
    public class MineDifficulty
    {
        private static readonly string JsonUrlWhattomine = "http://whattomine.com/coins.json";
        private static MineBlob blob;
        private IDifficultyCtrl ctrl;
        public  MineDifficulty(IDifficultyCtrl controller)
        {
            ctrl = controller;
            ctrl.GetDiffucalty += this.GetDifficulty;
        }

        public double GetRevard(string CoinName, double hashRate = 1000)
        {
            using (var client = new WebClient())
            {
                var json = client.DownloadString(JsonUrlWhattomine);
                blob = JsonConvert.DeserializeObject<MineBlob>(json);
            }
            var blockReward = blob["coins"][CoinName].BlockReward;
            var blocksPerDay = blob["coins"][CoinName].BlockTime;// * 24;//block count in day,
            var netHash = blob["coins"][CoinName].NetHash;
            var proff = blob["coins"][CoinName].Profitability;

            string result = $"Rew: {blockReward} time: {blocksPerDay} nH: {netHash} = {86400 /blocksPerDay * blockReward / netHash * hashRate}";
            RigEx.WriteLineColors($"{CoinName}: \trew {blockReward} \tt:{blocksPerDay} \tnh {netHash} \t= {86400 / blocksPerDay * blockReward / netHash * hashRate} / {proff}", ConsoleColor.Cyan);
            return 86400 / blocksPerDay * blockReward / netHash * hashRate;
        }
        public Dictionary<string, double> GetDifficulty()
        {
            using (var client = new WebClient())
            {
                var json = client.DownloadString(JsonUrlWhattomine);
                blob = JsonConvert.DeserializeObject<MineBlob>(json);
            }
            Dictionary<string, double> difficultyDictionary = new Dictionary<string, double>();
            foreach (ICoin coin in ctrl.GetCoins)
            {
                if (!blob["coins"].ContainsKey(coin.Name))
                {
                    RigEx.WriteLineColors($"{coin.Name} not found in WotToMine.com",ConsoleColor.DarkRed);
                    continue; 
                }
                var blockReward = blob["coins"][coin.Name].BlockReward;
                var blocksPerDay = blob["coins"][coin.Name].BlockTime;// * 24;//block count in day,
                var netHash = blob["coins"][coin.Name].NetHash;
                var proff = blob["coins"][coin.Name].Profitability;

                string result = $"Rew: {blockReward} time: {blocksPerDay} nH: {netHash} = {86400 / blocksPerDay * blockReward / netHash * coin.Hashrate}";
                RigEx.WriteLineColors($"{coin.Name}: \trew {blockReward} \tt:{blocksPerDay} \tnh {netHash} \t= {86400 / blocksPerDay * blockReward / netHash * coin.Hashrate} / {proff}", ConsoleColor.Cyan);
                double reward = 86400 / blocksPerDay * blockReward / netHash * coin.Hashrate;
                difficultyDictionary.Add(coin.Name,reward);
            }
            return difficultyDictionary;
        }
    }
}
