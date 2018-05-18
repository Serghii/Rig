using System;
using System.Collections.Generic;
using Rig.Telegram;

namespace Rig
{
    public interface IGsheetData
    {
        List<AlarmSettings> AlarmSettingsList { get; }
        List<Coin> CoinsList { get; }
        List<AlarmSettings> StopSettingsList { get; }
        List<ServerInfo> UserToKen { get; }
        List<ServerInfo> ServersSheetId { get; }
        List<MinerInfo> MinersList { get; }
        string BotId { get; set; }
        MyPage MyPage { get; set; }
        int PingDelayMillisec { get; set; }
    }

    public interface IDifficulty
    {
        IEnumerable<ICoin> GetCoins { get; }

    }
    public interface IAlarmData
    {
        IEnumerable<IAlarmSettings> GetAlarmSetings { get; }
        IEnumerable<IAlarmSettings> GetStopSettings { get; }
        List<AlarmSensor> Alarmdata { get;}
        ISensors GetSensors { get; }
    }

    public interface ISettings: IAlarmData, IGsheetData, IDifficulty
    {
        event Action MinerActivityChange;
        bool MinerStatus { get; set; }
        IEnumerable<IToken> userToKen { get; }
        IEnumerable<IServer> serversSheetId { get; }
        IEnumerable<IMinerInfo> miners { get; }
        IMinerInfo CurMiner { get; set; }
        string BotId { get; set; }
        Dictionary<SensorsType, bool> SensorActivity { get; set; }
        //int PingDelay { get; set; }
    }
    class SettingsData: ISettings
    {
        public event Action MinerActivityChange = () => { };
        private static List<AlarmSettings> _alarmSettingsList = new List<AlarmSettings>();
        private static List<AlarmSettings> _stopSettingsList = new List<AlarmSettings>();
        private static List<ServerInfo> _userToKen = new List<ServerInfo>();
        private static List<ServerInfo> _serversSheetId = new List<ServerInfo>();
        private static List<MinerInfo> _minersList = new List<MinerInfo>();
        private static List<AlarmSensor> alarmsensors = new List<AlarmSensor>();
        private static Sensors _sensors = new Sensors();
        private static List<Coin> _coinsList  = new List<Coin>();
        private static IMinerInfo curMiner = null;
        private static  bool minerStatus;
        private static string botId = String.Empty;
        private static MyPage myPage ;

        //private static TimeSpan alarmDelay = TimeSpan.FromMinutes(5);
        private static int pingDelayMillisec = 60000;

        private static Dictionary<SensorsType, bool> sensorActivity = new Dictionary<SensorsType, bool>
        {
            {SensorsType.CpuLoad, true},
            {SensorsType.CpuTemperature, true},
            {SensorsType.VideoTemperature, true},
            {SensorsType.GPUCore, true},
        };

        public string BotId
        {
            get => botId;
            set => botId = value;
        }

        public MyPage MyPage
        {
            get => myPage;
            set => myPage = value;
        }

        public bool MinerStatus
        {
            get { return minerStatus; }
            set
            {
                if (value != minerStatus)
                {
                    minerStatus = value;
                    MinerActivityChange();
                }
            }
        }

        public IMinerInfo CurMiner
        {
            get { return curMiner; }
            set
            {
                curMiner = value;
                RigEx.SafeLastMiner(curMiner.Name);
            }
        }

        public List<AlarmSettings> AlarmSettingsList
        {
            get { return _alarmSettingsList; }
            set { _alarmSettingsList = value; }
        }

        public List<Coin> CoinsList
        {
            get { return _coinsList; }
            set { _coinsList = value; }
        }

        public List<AlarmSettings> StopSettingsList
        {
            get { return _stopSettingsList; }
            set { _stopSettingsList = value; }
        }

        public List<ServerInfo> UserToKen
        {
            get { return _userToKen; }
            set { _userToKen = value; }
        }

        public List<ServerInfo> ServersSheetId
        {
            get { return _serversSheetId; }
            set { _serversSheetId = value; }
        }

        public List<MinerInfo> MinersList
        {
            get { return _minersList; }
            set { _minersList = value; }
        }
        
        public ISensors GetSensors => _sensors;

        public Dictionary<SensorsType, bool> SensorActivity
        {
            get => sensorActivity;
            set => sensorActivity = value;
        }

        public int PingDelayMillisec
        {
            get { return pingDelayMillisec; }
            set
            {
                pingDelayMillisec = value;
            }
        }
        public IEnumerable<ICoin> GetCoins => CoinsList.As<Coin, ICoin>();

        public IEnumerable<IAlarmSettings> GetAlarmSetings => _alarmSettingsList.As<AlarmSettings, IAlarmSettings>();

        public IEnumerable<IAlarmSettings> GetStopSettings => _stopSettingsList.As<AlarmSettings, IAlarmSettings>();

        List<AlarmSensor> IAlarmData.Alarmdata => alarmsensors;

        public IEnumerable<IToken> userToKen => _userToKen.As<ServerInfo, IToken>();

        public IEnumerable<IServer> serversSheetId => _serversSheetId.As<ServerInfo, IServer>();

        public IEnumerable<IMinerInfo> miners => _minersList.As<MinerInfo, IMinerInfo>();
    }
}
