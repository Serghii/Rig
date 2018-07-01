using System;
using System.Text;
using Rig.Telegram.Model;

namespace Rig.Telegram.TeleCommand
{
    class ShowTemperatureTCmd : TCommandBase, ITCommand
    {
        private XmrHrate HashRate = new XmrHrate();
        public ShowTemperatureTCmd(ITCommandService srv) : base(srv) { }
        public TCmdType Type => TCmdType.temperature;
        public void Execute(JsonData jd)
        {
            Execute();
        }

        public override string Name => TeleSettings.temperature;

        public void Execute()
        {
            RigEx.WriteLineColors($"Command show Temperature menu".AddTimeStamp(), ConsoleColor.Yellow);
            StringBuilder sb = new StringBuilder();
            foreach (var sensor in srv.Ctrl.GetSensors.GetSensor)
            {
                sb.AppendLine();
                sb.Append(sensor.Name + ":  ");

                foreach (var f in sensor.Dictionary)
                    sb.Append($"{f.Key.Icon()}: {f.Value} {f.Key.ValueIcon()}");
            }
            sb.AppendLine();
            sb.Append(
                $"{Icons.miner} :  {srv?.CurMiner?.Name}  status: {srv?.Ctrl?.MinerStatus} {(srv?.Ctrl?.MinerStatus == true ? Icons.GreenOk : Icons.RedPoint)}");
            string result = sb.ToString();
            Console.WriteLine(result);
            try
            {
                foreach (IToken token in srv.Ctrl.TelegramUser)
                    Bot.Client?.SendTextMessageAsync(token.Id, result);
            }
            catch (Exception e)
            {
                RigEx.WriteLineColors($"Send message Error: {e.Message}".AddTimeStamp(), ConsoleColor.DarkRed);
            }

        }
    }
}
