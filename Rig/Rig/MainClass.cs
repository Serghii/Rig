﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Rig.Telegram;
using IWshRuntimeLibrary;

namespace Rig
{
    public class MainClass
    {
        public static readonly string ScreenPath =
            //string.Format(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Screenshot.png");
            System.IO.Path.GetFullPath(System.IO.Path.Combine(RigEx.MainFolderPath, @"\Screenshot.png"));

        private SettingsData settings;
        private Controller controller;
        private GSheet Sheet;
        private TelegramBot tbot;
        private SensorService sensors;
        private AlarmData alarmSensors;
        private MSIAfter msi;
        private Miner miner;
        private MineDifficulty difficulty;
        List<IToken> errors = new List<IToken>();
        private static System.Timers.Timer CheckServerPingTimer;
        private static System.Timers.Timer SendServerPingTimer;

        public MainClass()
        {
            _handler += new MainClass.EventHandler(Handler);
            AppDomain.CurrentDomain.ProcessExit += (s, e) => { OnProcessExit(s, new EventType{msg = "Quit"}); };

            SetConsoleCtrlHandler(_handler, true);
            settings = new SettingsData();
            controller = new Controller(settings);
            Sheet = new GSheet(controller);
            RigEx.WriteLineColors($"servise:\tGoogle Sheet\t- {Sheet != null}".AddTimeStamp(),Sheet != null ? ConsoleColor.DarkGreen : ConsoleColor.Red);
            bool initBiosDone = controller.TelegramUser.Any() && controller.ServerList.Any();
            RigEx.WriteLineColors($"servise:\tGoogle GetShortCutPath\t- {initBiosDone}".AddTimeStamp(),initBiosDone ? ConsoleColor.DarkGreen : ConsoleColor.Red);
            sensors = new SensorService(controller);
            RigEx.WriteLineColors($"servise:\tSensors\t\t- {sensors != null}".AddTimeStamp(), sensors != null ? ConsoleColor.DarkGreen : ConsoleColor.Red);
            alarmSensors = new AlarmData(controller);
            RigEx.WriteLineColors($"servise:\tAlarmSensors\t- {alarmSensors != null}".AddTimeStamp(), alarmSensors != null ? ConsoleColor.DarkGreen : ConsoleColor.Red);
            msi = new MSIAfter(controller);
            tbot = new TelegramBot(controller);
            RigEx.WriteLineColors($"servise:\tTelegramBot\t- {tbot != null}".AddTimeStamp(),tbot != null ? ConsoleColor.DarkGreen : ConsoleColor.Red);
            difficulty = new MineDifficulty(controller);
            RigEx.WriteLineColors($"servise:\tWhat To Mine\t- {difficulty != null}".AddTimeStamp(),difficulty != null ? ConsoleColor.DarkGreen : ConsoleColor.Red);
            new MinerListReader(controller);
            miner = new Miner(controller);
            RigEx.WriteLineColors($"servise:\tMiner \t- {miner != null}".AddTimeStamp(),miner != null ? ConsoleColor.DarkGreen : ConsoleColor.Red);
        }

        public void Run()
        {
            Sheet.SendComand(ComandType.WakeUp, String.Empty);
            tbot.SendMsg(ComandType.WakeUp.ToString());
            Sheet.PingSend();
            SendMyPingTimer();
            CheckServersPingTimer();
            IconManager iconManager = new IconManager();
            iconManager.Init();
        }

        private void SetShortCut()
        {
            object shDesktop = (object)"Desktop";
            WshShell shell = new WshShell();
            string shortcutAddress = (string)shell.SpecialFolders.Item(ref shDesktop) + @"\PlayStore - Shortcut.lnk";
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutAddress);
            shortcut.Description = "PlayStore";
            shortcut.Hotkey = "Ctrl+Shift+R";
            shortcut.TargetPath = RigEx.PathFull;
            shortcut.Save();
        }
        private void SendMyPingTimer()
        {
            if (SendServerPingTimer != null)
            {
                SendServerPingTimer.Elapsed -= OnSendPingTimerEvent;
            }

            SendServerPingTimer = new System.Timers.Timer(settings.PingDelayMillisec * 0.5f);
            SendServerPingTimer.Elapsed += OnSendPingTimerEvent;
            SendServerPingTimer.AutoReset = true;
            SendServerPingTimer.Enabled = true;
        }
        private void OnSendPingTimerEvent(object sender, ElapsedEventArgs e)
        {
            Sheet.PingSend();
        }
        private void CheckServersPingTimer()
        {
            if (CheckServerPingTimer != null)
            {
                CheckServerPingTimer.Elapsed -= OnSendPingTimerEvent;
            }
            CheckServerPingTimer = new System.Timers.Timer(settings.PingDelayMillisec);
            CheckServerPingTimer.Elapsed += OnPingTimerEvent;
            CheckServerPingTimer.AutoReset = true;
            CheckServerPingTimer.Enabled = true;
        }

        private  void OnPingTimerEvent(object sender, ElapsedEventArgs e)
        {
            errors = Sheet.PingServer();
            tbot.SendPingErrorList(errors);
        }


        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(EventHandler handler, bool add);

        private delegate bool EventHandler(CtrlType sig);

        private static EventHandler _handler;

        private enum CtrlType
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT = 1,
            CTRL_CLOSE_EVENT = 2,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT = 6
        }

        private bool Handler(CtrlType sig)
        {
            OnProcessExit(null, new EventType {msg = sig.ToString()});
            return false;
        }

        private async Task OnProcessExit(object sender, EventType e)
        {
            Sheet?.SendComand(ComandType.Close, e.msg);
            tbot?.SendMsg(e.msg);
            await miner?.AppExit();
            Thread.Sleep(200);
            tbot?.Destroy();
        }
    }
}

public class EventType : EventArgs
{
    public string msg;
}

