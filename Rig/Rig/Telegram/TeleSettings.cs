using System;
using System.Collections.Generic;
using System.Linq;

namespace Rig.Telegram
{
    [Flags]
    public enum TCmdType
    {
        none = 0,
        miner = 1,
        screen = 2,
        restartMiner = 3,
        restartconfirm = 4,
        stopPing = 5,
        stopAllPing = 6,
        startAllPings = 7,
        showRates =8,
        startMining = 9,
        stopMining = 10,
        changeMiner = 11,
        AlarmIgnor = 12,
        StopAlarm = 14,
        UpdateGSheet= 15,
        turnOnAll = 16,
        mainKeybord = 17,
        runAll = 18,
        pcmenu = 19,
        lounchMiner = 21,
        pcRestart = 22,
        pcShutdown = 23,
        pc = 24,
        temperature
    }
    public static class Icons
    {
        public static readonly string nalik = "💰";
        public static readonly string bitok = "🅱️";
        public static readonly string kyrs = "💲";
        public static readonly string cpuL= "⚡️";
        public static readonly string cpuT= "CPU    🌡";
        public static readonly string gpuT= "Video 🌡";
        public static readonly string celsius = "℃";
        public static readonly string persent = "%";
        public static readonly string miner = "🐜";
        public static readonly string fire = "🔥";
        public static readonly string high = "🔺";
        public static readonly string low = "🔽";
        public static readonly string ring = "🔊";
        public static readonly string clock = "⏰";
        public static readonly string RedPoint = "🔴";
        public static readonly string GreenOk = "✅";
        public static readonly string temperature = "🌡";

        public static string ValueIcon(this SensorsType type)
        {
            switch (type)
            {
                case SensorsType.CpuLoad:
                    return persent;
                case SensorsType.CpuTemperature:
                    return celsius;
                case SensorsType.VideoTemperature:
                    return celsius;
                case SensorsType.GPUCore:
                case SensorsType.none:
                default:
                    return String.Empty;
            }
        }
        public static string Icon(this SensorsType type)
        {
            switch (type)
            {
                case SensorsType.CpuLoad:
                    return cpuL;
                case SensorsType.CpuTemperature:
                    return cpuT;
                case SensorsType.VideoTemperature:
                    return gpuT;
                case SensorsType.GPUCore:
                    return clock;
                case SensorsType.none:
                default:
                    return String.Empty;
            }
        }

        public static string Icon(this AlarmType at) 
        {
            switch (at)
            {
                case AlarmType.Low:
                    return low;
                case AlarmType.High:
                    return high;
                default:
                    return String.Empty;
            }
        }
    }

    public static class TeleSettings
    {
        public static readonly string MyName = Environment.MachineName.ToLower();
        public static readonly string f01 = "481671345:AAEsZuCj4FuTQ-L4xdlj2YC6U37CKwMZZ70";
        public static readonly string f02 = "579217381:AAHSle--RPAETP6OhbhIwk2ATvKYWloIkUk";
        public static readonly string S03_bot = "463712557:AAEu7Z1AYGvm78Oq2rosRvAbT_TQJHS9rVI";
        public static readonly string S01_bot = "383624735:AAFu9TjuVqwqL1xH5Rgoa6bSxKySZjOW6Ss";
        public static readonly string S02_bot = "505468158:AAEQBd5guJNjwTelm1h5NnQDEoPaY35EoCY";

        //public static readonly string stopAllPing = "⭕️ Stop All Ping ";
        public static readonly string starAllPing = $"{Icons.GreenOk} Start All Ping";
        public static readonly string stopsAllPing = $"{Icons.RedPoint} Stop All Ping";
        public static readonly string stopPing = "📵 stop ping";
        public static readonly string key = "key";
        public static readonly string k = "k";

        public static readonly string miner = "miner";
        public static readonly string rates = "💹 rates ";
        public static readonly string pcrestart = "🔄 pc restart";
        public static readonly string pcshutdown = "👀 pc shutdown ";
        public static readonly string restartMiner = "🔄 restart miner ";
        public static readonly string screen = "🖥 screen ";
        public static readonly string infopc = "🖥 pc";
        public static readonly string startMining = "✅ start miner";
        public static readonly string stopMining = $"{Icons.RedPoint} stop miner";
        public static readonly string ignoreAlarm = "🔇 ignore";
        public static readonly string update = "📝 update data";
        public static readonly string runAll = "🚀 run All";
        public static readonly string stopAlarm = "🔕 stop alarm";
        public static readonly string changeMiner = "change miner";
        public static readonly string temperature = "show 🌡";


        private static Dictionary<TCmdType, string> commandToString = new Dictionary<TCmdType, string>()
        {
            {TCmdType.miner, TeleSettings.miner}, {TCmdType.screen, TeleSettings.screen}, {TCmdType.restartMiner, TeleSettings.restartMiner},
            { TCmdType.restartconfirm, ""},
            {TCmdType.stopAllPing, TeleSettings.stopsAllPing}, {TCmdType.startAllPings, TeleSettings.starAllPing},
            { TCmdType.showRates, TeleSettings.rates}, {TCmdType.startMining, TeleSettings.startMining},
            { TCmdType.stopMining, TeleSettings.stopMining}, {TCmdType.changeMiner, ""},
            { TCmdType.AlarmIgnor, TeleSettings.ignoreAlarm}, {TCmdType.StopAlarm, TeleSettings.stopAlarm},
            { TCmdType.UpdateGSheet, TeleSettings.update}, {TCmdType.turnOnAll, TeleSettings.runAll},
            { TCmdType.temperature,TeleSettings.temperature}
        };

        public static string ToCommandString(this TCmdType c)
        {
            return commandToString.ContainsKey(c) ? commandToString[c] : String.Empty;
        }

        public static TCmdType StringToCommand(this string c)
        {
            return commandToString.FirstOrDefault(i => i.Value.Equals(c)).Key;
        }
    }
}
