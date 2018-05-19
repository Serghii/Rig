using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Rig.Telegram;

namespace Rig
{
    public interface IGSheet
    {
        ICtrlSheet Data { get; }
        MyPage MyPage { get; }
        int NewVersion { get; }

    }
    public class GSheet: IGSheet
    {
        public int NewVersion { get; private set; }
        private ICtrlSheet data;
        private readonly string[] ScopesSheets = {SheetsService.Scope.Spreadsheets};

        private readonly UserCredential credential;
        private SpreadsheetsResource.ValuesResource.BatchGetRequest request;
        private BatchGetValuesResponse response;
        private IAttributesReader[] AttributesReaderCommands;

        private ISendComand Ping; 
        private ISendComand WaikeUp; 
        private ISendComand Restart; 
        private ISendComand Clouse; 
        private ISendComand CPU; 
        private ISendComand Message;

        private SendComand sendLastIdLine ;
        LastIdComand lastIdLine = new LastIdComand();
        public MyPage MyPage => data.MyPage;
        public ICtrlSheet Data => data;
        public int LastIdComand
        {
            get { return lastIdLine.lineId; }
            set
            {
                lastIdLine.lineId = value;
                sendLastIdLine.Send(lastIdLine.lineId.ToString());
            }
        }

        public GSheet(ICtrlSheet settings)
        {
            data = settings;
            InitAttributesReaderCommands();
            credential = GetSheetCredentials();

            var service = GetService(credential);
            if (service == null)
                return;

            data.MyPage = new MyPage(service);
            
            request = data.MyPage.Service.Spreadsheets.Values.BatchGet(data.MyPage.SpreadsheetId);
            CreateSheets();
            CreateComands();
            CheckList();
            lastIdLine.lineId = UpdateLastidCommand();
            data.UpdateDataAction += OnUpdateDataAction;
            data.MinerActivityAction += OnMinerActivityAction;
            data.AlarmAction += OnAlarmAction;
            Console.WriteLine("Telegram token:");
            data.UserToKen.ForEach(i=> RigEx.WriteLineColors($"{i.Name} - {i.Id}",ConsoleColor.DarkCyan));
            RigEx.WriteLineColors($"my name {data.MyPage.MyServerSheetId.Name} ",ConsoleColor.Yellow);
            Console.WriteLine("\nOther Rigs:");
            if (!data.ServersSheetId.Any())
            {
                Console.WriteLine("other ServSheetId == null");
            }
            else
            {
                foreach (var s in data.ServersSheetId)
                {
                    RigEx.WriteLineColors($"server name:{s.Name}",ConsoleColor.Yellow);
                }
            }
        }

        private void OnAlarmAction(IAlarmSensor alarmSensor)
        {
            SendComand(ComandType.Alarm, $"{alarmSensor.Sensor.Name}:{alarmSensor.SensorType}:{alarmSensor.Sensor.Dictionary[alarmSensor.SensorType]} is to {alarmSensor.AlarmType}");
        }

        private void OnMinerActivityAction()
        {
            SendComand(ComandType.Message,$"MinerActivity:{data.MinerStatus}");
        }

        private void InitAttributesReaderCommands()
        {
            if (data == null)
            {
                RigEx.WriteLineColors("Google sheet Error: ICtrlSheet is null", ConsoleColor.Red);
                return;
            }
            AttributesReaderCommands = new IAttributesReader[]
            {
                new GSCoinCmd(this),
                new GSUserTokenCmd(this),
                new GSsheetIdCmd(this),
                new GSPingNotifyCmd(this),
                new GSMinerLineCmd(this),
                new GSAlarmLineCmd(this),
                new GSStopMinerCmd(this),
                new GSBotIdCmd(this), 
                new GSVersionCmd(this), 
            };
        }

        private void OnUpdateDataAction()
        {
            data.UserToKen.Clear();
            data.AlarmSettingsList.Clear();
            data.StopSettingsList.Clear();
            data.MinersList.Clear();
            data.ServersSheetId.Clear();
            CreateSheets();
        }

        public void PingSend()
        {
            Console.WriteLine("update my ping".AddTimeStamp());
            Ping.Send(string.Empty);
        }

        public List<IToken> PingServer()
        {
            List<IToken> errorServerIds = new List<IToken>();
            
            for (var i = 0; i < data.ServersSheetId.Count; i++)
            {
                var sheetId = data.ServersSheetId[i];
                if (!sheetId.isActive || sheetId.Name == GShSettings.PCname)
                    continue;

                var res = GetComandLineFrom(sheetId.Name);
                if (res == null)
                {
                    Console.WriteLine($"Google sheet cannot read {sheetId.Name}");
                    continue;
                }
                foreach (IList<object> obj in res)
                {
                    if (obj == null)
                        continue;

                    bool pingOk = CheckPing(obj);
                    RigEx.WriteLineColors($"{sheetId.Name}",
                        pingOk ? ConsoleColor.DarkGreen : ConsoleColor.DarkRed);
                    if (!pingOk)
                        errorServerIds.Add(sheetId);
                }
            }
            return errorServerIds;
        }

        public void SendComand(ComandType type, string msg)
        {
            
            switch (type)
            {
                case ComandType.WakeUp:
                    WaikeUp.Send(msg);
                    break;
                case ComandType.Restart:
                    Restart.Send(msg);
                    break;
                case ComandType.Alarm:
                    CPU.Send(msg);
                    break;
                case ComandType.Ping:
                    Ping.Send(msg);
                    break;
                case ComandType.Close:
                    Clouse.Send(msg);
                    break;
                case ComandType.Message:
                default:
                    Message.Send(msg);break;
            }
            LastIdComand ++;
        }

        private void CreateSheets()
        {
            var Firstline = GetRange("bios!1:1");
            if (Firstline == null || !Firstline.Any())
                return;

            int newVersion;
            if (!int.TryParse( Firstline[0][2].ToString(),out newVersion))
            {
               RigEx.WriteLineColors("Google Sheet => Cannot read new version on C1 ",ConsoleColor.Red);
               return;
            }
            NewVersion = newVersion;

            var range = Firstline[0][0].ToString();
            var res = GetRange(range);

            for (int i = 0; i < res.Count; i++)
            {
                var line = res[i];
                if (line.Count == 0 || string.IsNullOrEmpty(line[0].ToString()))
                    continue;

                string attribute = line[0].ToString().ToLower();
                var com = AttributesReaderCommands.FirstOrDefault(o => o.GetAttributesType == attribute);
                if (com != null)
                {
                    com.Execute(line);
                    
                }
            }
        }

        private void CreateComands()
        {
            Ping = new PingComand(data.MyPage);
            WaikeUp = new WakeUpCommand(data.MyPage,lastIdLine);
            Restart = new RestartCommand(data.MyPage,lastIdLine);
            Clouse = new CloseCommand(data.MyPage,lastIdLine);
            CPU = new CPUComand(data.MyPage,lastIdLine);
            Message = new MessageComand(data.MyPage, lastIdLine);
            sendLastIdLine = new SendComand(data.MyPage, 1, GShSettings.lastlineId.LaterToColum());
        }

        private void CheckList()
        {
            if (data.MyPage.MyServerSheetId == null)
            {
                RigEx.WriteLineColors($"ERROR: not found Sheet Id for this PC will write on Default page".AddTimeStamp(),ConsoleColor.Red);
                data.MyPage.MyServerSheetId = new ServerInfo();
            }
        }

        private int UpdateLastidCommand()
        {
            int result = 3;
            string range = $"{data.MyPage.MyServerSheetId.Name}!{GShSettings.lastlineId}2";
            var res = GetRange(range);
            if (res != null)
            {
                foreach (IList<object> objects in res)
                {
                    if (objects != null)
                    {
                        if (objects[0] != null && !int.TryParse(objects[0].ToString(), out result))
                        {
                            Console.WriteLine(" not found last command line");
                        }
                    }
                }
            }
            return result < 3 ? 3 : result;
        }

        private bool CheckStatus(IList<object> comandLine)
        {
            var serverstatus = comandLine[GShSettings.ServerStatus.LaterToColum()].ToString();
            return serverstatus.ToLower().Contains("on");
        }

        private bool CheckPing(IList<object> comandLine)
        {
            var serverPing = Convert.ToDateTime(comandLine[GShSettings.ping.LaterToColum()]);
            //var diferent = DateTime.Today.Add(DateTime.Now - serverPing);
            TimeSpan diferents = DateTime.Now - serverPing;
            Console.Write($"ping: [ {diferents:hh\\:mm\\:ss} ] ".AddTimeStamp());
            return data.PingDelayMillisec > Math.Abs(diferents.TotalMilliseconds);
            //return diferent.Hour < 1 && data.MyPage.AlarmDelay > diferent;
        }

        private IList<IList<object>> GetComandLineFrom(string sheetId)
        {
            var comandline = GetRange($"{sheetId}!A2:F2");
            if (comandline == null)
                RigEx.WriteLineColors($"cannot read ComandLine A2:Z2 from{sheetId}",ConsoleColor.DarkRed);
            return comandline;
        }

        private IList<IList<object>> GetRange(params string[] range)
        {
            request = data.MyPage.Service.Spreadsheets.Values.BatchGet(data.MyPage.SpreadsheetId);
            request.Ranges = new Repeatable<string>(range);
            IList<IList<object>> result;
            try
            {
                response = request.Execute();
                result = response.ValueRanges.SelectMany(x => x.Values).ToList();
                return result;
            }
            catch (Exception)
            {
                Console.WriteLine($"Google Sheet read error from {range[0]}");
                return null;
            }
        }

        private UserCredential GetSheetCredentials()
        {
            var path = Path.GetFullPath(Path.Combine(System.Reflection.Assembly.GetEntryAssembly().Location, @"..\..\"));
            try
            {
                using (var stream = new FileStream(Path.Combine(path, GShSettings.ClientSecret), FileMode.Open,
                    FileAccess.Read))
                {
                    var creadPath = Path.Combine(path, "sheetsCreds.json");
                    return GoogleWebAuthorizationBroker.AuthorizeAsync(GoogleClientSecrets.Load(stream).Secrets,
                        ScopesSheets, "user", CancellationToken.None, new FileDataStore(creadPath, true)).Result;
                }
            }
            catch (Exception e)
            {
                RigEx.WriteLineColors($"Cannot read Google Sheet secret key Error:{e.Message}".AddTimeStamp(),ConsoleColor.Red);
                return null;
            }
        }

        private SheetsService GetService(UserCredential credential)
        {
            return new SheetsService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential, ApplicationName = GShSettings.AppName
            });
        }

    }
}
