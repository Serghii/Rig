using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Rig.Telegram;
using Rig.Telegram.Model;

namespace Rig
{
    public interface ICtrlSensorServise
    {
        ISensors GetSensors { get; }
    }
    public enum AlarmCommand
    {
        CpuTemperatureAlarm,
        CpuLoadAlarm,
        GpuTemperatureAlarm
    }
    public enum CurrentMinerCommand
    {
        stopMiner,
        LounchMiner,
        ChangeAndRunMiner
    }
    public interface IAlarmAction 
    {
        event Action<IAlarmSensor> AlarmAction;
    }

    public interface ISensorsSetings
    {
        IEnumerable<IAlarmSettings> GetAlarmSettings { get; }
        IEnumerable<IAlarmSettings> GetStopSettings { get; }
        Dictionary<SensorsType, bool> SensorActivity { get; }

    }
    public interface ICtrlAlarm : ISensorsSetings, IMiningStatus, ICtrlSensorServise
    {
        
        List<AlarmSensor> GetAlarmdata { get; }
        void CallAlarm(IAlarmSensor sensor);
        void SetMinigActivityStatus(bool setActivity);
    }

    public interface IMiningStatus
    {
        event Action MinerActivityAction;
        bool MinerStatus { get; set; }
    }
    public interface IMiningAction: IMiningStatus
    {
        IEnumerable<IMinerInfo> Miners { get; }
        IMinerInfo CurMiner { get;  }
    }

    public interface ITelegramCtrl : IMiningAction, IAlarmAction, IGetDifficulty, ICtrlSensorServise
    {
        void MinerCommand(CurrentMinerCommand c, IEventArgs e = null);
        IEnumerable<IToken> TelegramUser { get; }
        IEnumerable<IServer> ServerList { get; }
        void AlarmIgnoreCommand(string sensorsType);
        void SetAllAlarmActivity(bool setActive);
        void SetAllPingActivity(bool setActive);
        void SetPingActivityFor(string serverName, bool setActive);
        void UpdateData();
        void RestartButton();
        void CreateScreenShot();
    }

    public interface IMiningCtrl : IMiningAction, IAlarmAction
    {
        event Action<IEventArgs> ButtonChangeAndRunMinerAction;
        event Action<IEventArgs> ButtonLaunchMinerAction;
        
        IMinerInfo CurMiner { get; set; }
        void SendMsg(string msg);
    }

    public interface ICtrlSheet : IGsheetData, IMiningStatus, IAlarmAction
    {
        event Action UpdateDataAction;
        //TimeSpan AlarmDelay { get; set; }
        bool VersionIsLiquid(int curVersion, int newversion);
        int PingDelayMillisec { get; set; }
    }

    public interface IGetDifficulty
    {
        Func<Dictionary<string, double>> GetDiffucalty { get;}
    }

    public interface IDifficultyCtrl: IGetDifficulty
    {
        Func<Dictionary<string, double>> GetDiffucalty { get; set; }
        IEnumerable<ICoin> GetCoins { get;}
    }
    public class Controller : ITelegramCtrl, IMiningCtrl, ICtrlAlarm, ICtrlSheet, IDifficultyCtrl, ICtrlSensorServise
    {
        public static readonly string ScreenPath =
            string.Format(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Screenshot.png");
        public event Action<IAlarmSensor> AlarmAction = sensor => { };
        public event Action<IEventArgs> ButtonChangeAndRunMinerAction = name => {};
        public event Action<IEventArgs> ButtonLaunchMinerAction = name => {};
        public event Action<IEventArgs> ButtonStopMinerAction = args => {};

        public Dictionary<CurrentMinerCommand, Action<IEventArgs>> MinerCommandDictionary = new Dictionary<CurrentMinerCommand, Action<IEventArgs>>();
        private ISettings data;
        public event Action MinerActivityAction = () => { };
        public event Action UpdateDataAction = () => {};
        
        public Func<Dictionary<string, double>> getDiffucalty;

        public bool VersionIsLiquid(int curVersion, int newversion)
        {
            if (newversion > curVersion)
            {
                RigEx.WriteLineColors($"start Update Version curent: {curVersion} to last version: {newversion}", ConsoleColor.Green);
                RunLauncher();
                return false;
            }
            return true;
        }

        public int PingDelayMillisec
        {
            get => data.PingDelayMillisec;
            set => data.PingDelayMillisec = value;
        }

        public Controller(ISettings data)
        {
            this.data = data;
            InitCommandDictionary();
            data.MinerActivityChange += ()=>{MinerActivityAction();};
            ButtonLaunchMinerAction += OnButtonLaunchMinerAction;
            ButtonStopMinerAction += OnButtonStopMinerAction;
            OnButtonLaunchMinerAction(null);
        }
        public void CreateScreenShot()
        {
            if (data.GetSensors.GetSensor.All(i => i.HwType != HWType.gpu) )
            {
               RigEx.WriteLineColors("not found any video card for screenshot".AddTimeStamp(),ConsoleColor.DarkRed);
                try
                {
                    Bot.Client.SendTextMessageAsync(data.userToKen.First().Id, "not found any video card for screenshot");
                }
                catch (Exception e)
                {
                   RigEx.WriteLineColors($"Telegram send message  Error: {e.Message}".AddTimeStamp(),ConsoleColor.DarkRed);
                }
               
               return;
            }
            try
            {
                Bitmap bmpScreenCapture = new Bitmap(Screen.PrimaryScreen.Bounds.Width,
                    Screen.PrimaryScreen.Bounds.Height);
                using (Graphics g = Graphics.FromImage(bmpScreenCapture))
                {
                    g.CopyFromScreen(Screen.PrimaryScreen.Bounds.X,
                        Screen.PrimaryScreen.Bounds.Y,
                        0, 0,
                        bmpScreenCapture.Size,
                        CopyPixelOperation.SourceCopy);
                }
                bmpScreenCapture.Save(ScreenPath);
            }
            catch (Exception e)
            {
                RigEx.WriteLineColors($"Create Screen Shot Error:{e.Message}".AddTimeStamp(), ConsoleColor.DarkRed);
                Console.WriteLine(e.Message);

            }
        }
        private void OnButtonStopMinerAction(IEventArgs eventArgs)
        {
            data.MinerStatus = false;
        }

        private void OnButtonLaunchMinerAction(IEventArgs eventArgs)
        {
            data.MinerStatus = true;
        }
        public IEnumerable<IToken> TelegramUser => data.userToKen;

        public IEnumerable<IServer> ServerList => data.serversSheetId;


        public void SetAllAlarmActivity(bool setActive)
        {
            RigEx.WriteLineColors($"Command Set All Alarm Activity {setActive}".AddTimeStamp(), ConsoleColor.Yellow);
            for (int i = 0; i < data.SensorActivity.Count; i++)
            {
                var key = data.SensorActivity.ElementAt(i).Key;
                data.SensorActivity[key] = setActive;
            }
        }

        public void AlarmIgnoreCommand(string sensorType)
        {
            SensorsType type ;
            if (!Enum.TryParse(sensorType, out type))
            {
                RigEx.WriteLineColors($"AlarmCommand Cannot convert SensorsType {sensorType}", ConsoleColor.Red);
                return;
            }

            try
            {
                if (data.SensorActivity.ContainsKey(type))
                {
                    data.SensorActivity[type] = false;
                }
                else
                {
                    RigEx.WriteLineColors($"cannot find sensot for{type}".AddTimeStamp(),ConsoleColor.DarkRed);
                }
            }
            catch (Exception)
            {
                RigEx.WriteLineColors($"AlarmCommand cannot found any sensor whith this type: {type} ",ConsoleColor.DarkMagenta);
            }
        }

        public void SetAllPingActivity(bool setActive)
        {
            RigEx.WriteLineColors($"Command change All Ping Activity to {setActive }".AddTimeStamp(), ConsoleColor.Yellow);
            foreach (IServer server in ServerList)
                server.isActive = setActive;
        }

        public void SetPingActivityFor(string serverName, bool setActive)
        {
            RigEx.WriteLineColors($"Command SetPingActivityFor {serverName} to {setActive }".AddTimeStamp(), ConsoleColor.Yellow);

            if (string.IsNullOrEmpty(serverName))
            {
                RigEx.WriteLineColors($"SetPingActivityFor : server name is null -=> value{setActive}", ConsoleColor.DarkRed);
                return;
            }
            foreach (IServer server in ServerList)
            {
                if (server.Name == serverName)
                {
                    server.isActive = setActive;
                    break;
                }
            }
        }

        public void UpdateData()
        {
            UpdateDataAction();
        }

        public void RestartButton()
        {
            RigEx.WriteLineColors("QUIT".AddTimeStamp(),ConsoleColor.Yellow);
            RunLauncher();
            RigEx.QuitApp(500);
        }

        public ISensors GetSensors => data.GetSensors;
        public IEnumerable<IAlarmSettings> GetAlarmSettings => data.GetAlarmSetings;

        public IEnumerable<IAlarmSettings> GetStopSettings => data.GetStopSettings;
        public Dictionary<SensorsType, bool> SensorActivity => data.SensorActivity;


        public Func<Dictionary<string, double>> GetDiffucalty
        {
            get { return getDiffucalty; }
            set { getDiffucalty = value; }
        }

        public IEnumerable<ICoin> GetCoins => data.GetCoins;

        public List<AlarmSensor> GetAlarmdata => data.Alarmdata;


        public void CallAlarm(IAlarmSensor sensor)
        {
            AlarmAction(sensor);
        }

        public void SetMinigActivityStatus(bool setActivity)
        {
            data.MinerStatus = setActivity;
        }

        private void InitCommandDictionary()
        {
            MinerCommandDictionary.Add(CurrentMinerCommand.LounchMiner, s=>ButtonLaunchMinerAction(s));
            MinerCommandDictionary.Add(CurrentMinerCommand.stopMiner, s=>ButtonStopMinerAction(s));
            MinerCommandDictionary.Add(CurrentMinerCommand.ChangeAndRunMiner, s=>ButtonChangeAndRunMinerAction(s));
        }

        public void MinerCommand(CurrentMinerCommand c, IEventArgs e)
        {
            if (MinerCommandDictionary.ContainsKey(c))
                MinerCommandDictionary[c](e);
            else
                RigEx.WriteLineColors($"Warning Ctrl Cannot find command {c}".AddTimeStamp(),ConsoleColor.DarkRed);
        }

        public IEnumerable<IMinerInfo> Miners => data.miners;


        public IMinerInfo CurMiner
        {
            get { return data.CurMiner; }
            set { data.CurMiner = value; }
        }

        public void SendMsg(string msg)
        {
            try
            {
                foreach (var token in data.userToKen)
                     Bot.Client?.SendTextMessageAsync(token.Id, $"{msg}");
            }
            catch (Exception e)
            {
                RigEx.WriteLineColors($"Controller: Telegram Send:{msg}\nError: {e.Message}".AddTimeStamp(), ConsoleColor.DarkRed);
            }
        }

        public List<Coin> CoinsList => data.CoinsList;
        public List<AlarmSettings> AlarmSettingsList => data.AlarmSettingsList;

        public List<AlarmSettings> StopSettingsList => data.StopSettingsList;

        public List<ServerInfo> UserToKen => data.UserToKen;

        public List<ServerInfo> ServersSheetId => data.ServersSheetId;

        public List<MinerInfo> MinersList => data.MinersList;

        public string BotId
        {
            get => data.BotId;
            set
            {
                data.BotId = value;
                Bot.Get(BotId);
            }
        }

        public MyPage MyPage
        {
            get => data.MyPage;
            set => data.MyPage = value;
        }

        public bool MinerStatus
        {
            get => data.MinerStatus;
            set => data.MinerStatus = value;
        }

        private void RunLauncher()
        {
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.CreateNoWindow = true;
                startInfo.UseShellExecute = true;
                startInfo.FileName = RigEx.Path + @"RigLauncher.exe";
                startInfo.WindowStyle = ProcessWindowStyle.Normal;

                RigEx.WriteLineColors($"Run RigLauncher: {startInfo.FileName}".AddTimeStamp(), ConsoleColor.DarkCyan);
                Process.Start(startInfo);
            }
            catch (Exception e)
            {
                RigEx.WriteLineColors($"Run Launcher Error: {e.Message}", ConsoleColor.Red);
            }
        }
    }
}
