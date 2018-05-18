using System;
using System.Collections.Generic;
using System.Linq;
using Rig.Telegram.Model;
using Rig.Telegram.TeleCommand;
using Telegram.Bot.Args;
using Telegram.Bot.Types.ReplyMarkups;



namespace Rig.Telegram
{
    public interface ITCommandService
    {
        JsonData Jd { get; }
        ITelegramCtrl Ctrl { get; }
        IMinerInfo CurMiner { get; }
        ReplyKeyboardMarkup Markup { get; }
        KeyboardButton[][] HomeKeyButton { get; }
        InlineKeyboardMarkup HomeInlinekeyBoard { get; }
        InlineKeyboardMarkup GetMinersInlinekeyBoard();
        void InitHomeKeybord();
        void SendMsg(string msg);
    }
    class TelegramBot: ITCommandService
    {
        private  ReplyKeyboardMarkup markup = new ReplyKeyboardMarkup();
        private int stepForAllarm = 5;
        private int curStep = 0;
        private float CriticalCPULoad = 20;
        public KeyboardButton[][] HomeKeyButton { get; set; }
        public InlineKeyboardMarkup HomeInlinekeyBoard { get; set; }
        private  ITCommand[] commands ;
        
         
        public ITelegramCtrl Ctrl { get; set; }
        public JsonData Jd { get; set; }
        public ReplyKeyboardMarkup Markup => markup;
        public IMinerInfo CurMiner => Ctrl.CurMiner;


        public TelegramBot(ITelegramCtrl ctrl)
        {
            this.Ctrl = ctrl;
            InitCommands();
            InitHomeKeybord();
            InitHomeInlinekeyBoard();

            if (Bot.Client != null) Bot.Client.OnMessage += OnMessage;
            if (Bot.Client != null) Bot.Client.OnCallbackQuery += OnCallbackQuery;
            ctrl.AlarmAction += OnAlarmAction;
            ctrl.MinerActivityAction += OnMinerActivityAction;
        }

        private void OnMinerActivityAction()
        {
            SendMsg($"{TeleSettings.Miner}Miner {CurMiner.Name} activity: {Ctrl.MinerStatus}");
        }

        private void InitCommands()
        {
            commands = new ITCommand[]
            {
                new MinerTCmd(this),
                new UpdateTCmd(this),
                new MainKeyboardKTCmd(this),
                new MainKeyboardKeyTCmd(this),
                new StopAllPingTCmd(this),
                new StopAlarmTCmd(this),
                new RunAllTCmd(this),
                new PCMenuTCmd(this),
                new ScreenTCmd(this),
                new RestartTCmd(this),
                new IgnorPingTCmd(this),
                new IgnorAlarmTCmd(this),
                new ShowDifficultyTCmd(this),
                new LounchMinerTCmd(this),
                new ChangeMinerTCmd(this),
                new StopMinerTCmd(this)
            };
        }
        
        #region Home keyboard Command || Message

        public async void SendMsg(string msg)
        {
            try
            {
                foreach (var token in Ctrl.TelegramUser)
                    await Bot.Client?.SendTextMessageAsync(token.Id, $"{msg}");
            }
            catch (Exception e)
            {
                RigEx.WriteLineColors($"Telegram Send:{msg}\nError: {e.Message}".AddTimeStamp(), ConsoleColor.DarkRed);
            }
        }

        private void OnCallbackQuery(object sender, CallbackQueryEventArgs e)
        {
            if (null == e?.CallbackQuery)
                return;

            JsonData cbData = new JsonData(e.CallbackQuery.Data);
            try
            {
                commands.First(i=>i.Type == cbData.type).Execute(cbData);
            }
            catch (Exception ex)
            {
                RigEx.WriteLineColors($"Execute command: {cbData.type}\nError: {ex.Message}".AddTimeStamp(), ConsoleColor.DarkRed);
            }
        }

        private void OnMessage(object sender, MessageEventArgs e)
        {
            if (null == e?.Message || string.IsNullOrEmpty(e.Message.Text) || Ctrl.TelegramUser.All(i => i.Id != e.Message.Chat.Id))
                return;
            try
            {
                foreach (var command in commands)
                {
                    if (command.Is(e.Message.Text))
                    {
                        command.Execute();
                    }
                }
            }
            catch (Exception ex)
            {
                RigEx.WriteLineColors($"Message -> No command for {e.Message.Text} Error{ex.Message}".AddTimeStamp(), ConsoleColor.DarkYellow);
            }
        }

        private void OnAlarmAction(IAlarmSensor sensor)
        {
            var alarm =$"{Icons.ring} Alarm {sensor.SensorType.Icon()}: {sensor.Sensor.Dictionary[sensor.SensorType]} - {sensor.AlarmType.Icon()}";
            try
            {
                foreach (IToken token in Ctrl.TelegramUser)
                    Bot.Client?.SendTextMessageAsync(token.Id, alarm, replyMarkup: GetAlarmInlinekeyBoard(sensor.SensorType));
            }
            catch (Exception e)
            {
                RigEx.WriteLineColors($"Telegram Error: {e.Message}".AddTimeStamp(),ConsoleColor.DarkRed);
            }
        }
        
        #endregion
        
        public void Destroy()
        {
            Bot.Client?.StopReceiving();
        }

        public  InlineKeyboardMarkup GetMinersInlinekeyBoard()
        {
            int allEl = Ctrl.Miners.Count();
            var minerList = Ctrl.Miners.ToArray();
            int line = (int)Math.Ceiling(allEl/ 3f);
            int col = 3;
            InlineKeyboardButton[][] keyboardButtons = new InlineKeyboardButton[line][];
            int cyr = 0;
            for (int l = 0; l < line; l++)
            {
                int ll = Math.Min(allEl - cyr, 3);
                keyboardButtons[l] = new InlineKeyboardButton[ll];
                for (int cc = 0; cc < ll; cc++, cyr++)
                {
                    keyboardButtons[l][cc] = new InlineKeyboardButton {Text = minerList[cyr].Name,CallbackData = JsonData.Serialize(TCmdType.changeMiner, minerList[cyr].Name) };
                }
            }
            return new InlineKeyboardMarkup(keyboardButtons);
        }

        public void SendPingErrorList(List<IToken> errors)
        {
            foreach (IToken token in errors)
            {
                InlineKeyboardButton[][] testInlinekeyBoard = new InlineKeyboardButton[1][];
                testInlinekeyBoard[0] = new InlineKeyboardButton[1];
                testInlinekeyBoard[0][0] = new InlineKeyboardButton { Text = $"stop ping", CallbackData = $"{TCmdType.stopPing}:{token.Name}"};
            
                InlineKeyboardMarkup keyboardInlineMarkUp = new InlineKeyboardMarkup(testInlinekeyBoard);
                try
                {
                    foreach (IToken user in Ctrl.TelegramUser)
                    {
                        try
                        {
                            Bot.Client?.SendTextMessageAsync(user.Id, $"Error {token.Name}", replyMarkup: keyboardInlineMarkUp);
                        }
                        catch (Exception e)
                        {
                            RigEx.WriteLineColors($"cannot send ping error {user.Id} message:Error {token.Name} e:{e.Message}".AddTimeStamp(),ConsoleColor.Red);
                        }
                    }
                }
                catch (Exception e)
                {
                    RigEx.WriteLineColors($"cannot send ping error message:Error {token.Name} e:{e.Message}".AddTimeStamp(), ConsoleColor.DarkRed);

                }

            }
        }

        private void InitHomeInlinekeyBoard()
        {
            InlineKeyboardButton[][] keyboardButtons = new InlineKeyboardButton[3][];
            keyboardButtons[0] = new InlineKeyboardButton[2];
            keyboardButtons[0][0] = new InlineKeyboardButton { Text = $"{Icons.miner} {TCmdType.miner.ToCommandString()}", CallbackData = JsonData.Serialize(TCmdType.miner)};
            keyboardButtons[0][1] = new InlineKeyboardButton { Text = TCmdType.showDifficulty.ToCommandString(), CallbackData = JsonData.Serialize(TCmdType.showDifficulty)};

            keyboardButtons[1] = new InlineKeyboardButton[2];
            keyboardButtons[1][0] = new InlineKeyboardButton { Text = TeleSettings.restart, CallbackData = JsonData.Serialize(TCmdType.restart) };
            keyboardButtons[1][1] = new InlineKeyboardButton { Text = TeleSettings.screen, CallbackData = JsonData.Serialize(TCmdType.screen) };

            keyboardButtons[2] = new InlineKeyboardButton[2];
            keyboardButtons[2][0] = new InlineKeyboardButton { Text = TeleSettings.stopMining, CallbackData = JsonData.Serialize(TCmdType.stopMining) };
            keyboardButtons[2][1] = new InlineKeyboardButton { Text = TeleSettings.startMining, CallbackData = JsonData.Serialize(TCmdType.lounchMiner) };
            
            HomeInlinekeyBoard =  new InlineKeyboardMarkup(keyboardButtons);
        }

        private InlineKeyboardMarkup GetAlarmInlinekeyBoard(SensorsType type)
        {
            InlineKeyboardButton[][] keyboardButtons = new InlineKeyboardButton[1][];
            keyboardButtons[0] = new InlineKeyboardButton[2];
            keyboardButtons[0][0] = new InlineKeyboardButton { Text = TeleSettings.stopMining, CallbackData = JsonData.Serialize(TCmdType.stopMining)};
            keyboardButtons[0][1] = new InlineKeyboardButton { Text = TeleSettings.ignoreAlarm, CallbackData = JsonData.Serialize(TCmdType.AlarmIgnor, type.ToString())};
            return new InlineKeyboardMarkup(keyboardButtons);
        }

        public void InitHomeKeybord()
        {
            HomeKeyButton = new []
                {
                    GetKeyboardButtonsServerListButtons(),
                    new []
                    {
                        new KeyboardButton(commands.First(i=>i.Type == TCmdType.UpdateGSheet).Name),
                        new KeyboardButton(commands.First(i=>i.Type == TCmdType.runAll).Name),
                        new KeyboardButton(commands.First(i=>i.Type == TCmdType.stopAllPing).Name),
                        new KeyboardButton(commands.First(i=>i.Type == TCmdType.StopAlarm).Name),
                    }
                };
        }

        private KeyboardButton[] GetKeyboardButtonsServerListButtons()
        {
            var srvList = Ctrl.ServerList.OrderBy(i=>i.Name).ToList();
            var result = new KeyboardButton[srvList.Count+1];
            for (int i = 0; i < srvList.Count; i++)
            {
                result[i] = new KeyboardButton(srvList[i].Name);
            }
            result[srvList.Count]=new KeyboardButton(TeleSettings.MyName);
            return result;
        }
    }
}
