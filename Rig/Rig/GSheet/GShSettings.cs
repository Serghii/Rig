using System;
using System.IO;
public static class GShSettings
    {
        public static readonly string RangeAll = "bios!A1:Z100";
        public static readonly string SpreadsheetId = GetSpreadsheetId();
        public static readonly string UserTokenLine = "token";
        public static readonly string sheetId = "sheetid";
        public static readonly string minerLine = "miner";
        public static readonly string alarmLine = "alarm";
        public static readonly string stopMiner = "stop";
        public static readonly string ping = "a";
        public static readonly string lastlineId = "c";
        public static readonly string ServerStatus = "b";
        public static readonly string ClientSecret = "client_secret_.json";
        public static readonly string AppName = "forrig";
        public static readonly string PingNotify = "pingnotify";
        public static readonly string CoinName = "coinname";
        public static readonly string botId = "botid";

        public static readonly string PCname = Environment.MachineName.ToLower();

        private static string GetSpreadsheetId()
        {
            string path = Path.GetFullPath(Path.Combine(Path.Combine(System.Reflection.Assembly.GetEntryAssembly().Location, @"..\..\"), "sheetId.txt"));
            if (!File.Exists(path))
            {
                RigEx.WriteLineColors($"Cannot faund file {path} ",ConsoleColor.Red);
            return String.Empty;
            }
          return File.ReadAllText(path);
        }
    }
