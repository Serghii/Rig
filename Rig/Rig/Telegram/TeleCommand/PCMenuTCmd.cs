using System;
using System.Text;
using Rig.Telegram.Model;

namespace Rig.Telegram.TeleCommand
{
    class PCMenuTCmd : TCommandBase, ITCommand
    {
        private XmrHrate HashRate = new XmrHrate();
        public PCMenuTCmd(ITCommandService srv) : base(srv){}
        public TCmdType Type => TCmdType.pcmenu;
        public void Execute(JsonData jd)
        {
            Execute();
        }

        public override string Name => TeleSettings.MyName;

        public void Execute()
        {
            RigEx.WriteLineColors($"Command show PC menu".AddTimeStamp(), ConsoleColor.Yellow);
            StringBuilder sb = new StringBuilder();
            foreach (var sensor in srv.Ctrl.GetSensors.GetSensor)
            {
                sb.AppendLine();
                sb.Append(sensor.Name+":  ");

                foreach (var f in sensor.Dictionary)
                     sb.Append($"{f.Key.Icon()}: {f.Value} {f.Key.ValueIcon()}");
            }
            sb.AppendLine();
            sb.Append(
                $"{Icons.miner} :  {srv?.CurMiner?.Name}  status: {srv?.Ctrl?.MinerStatus} {(srv?.Ctrl?.MinerStatus == true ? Icons.GreenOk : Icons.RedPoint)}");
            Console.WriteLine(sb);

//            var rates = HashRate.GetHashRate;
//            if (rates > 0)
//            {
//                sb.Append($"\nHash Rate:\t {rates}");
//            }

            //            var btcPrice = HashRate.GetBTCPrice;
            //            if (btcPrice > 0)
            //            {
            //                sb.Append($" : {btcPrice} : {(balance > 0 ? (balance * btcPrice) : 0)}"); 
            //            }

            try
            {
                foreach (IToken token in srv.Ctrl.TelegramUser)
                Bot.Client?.SendTextMessageAsync(token.Id, sb.ToString(), replyMarkup: srv.HomeInlinekeyBoard);
            }
            catch (Exception e)
            {
                RigEx.WriteLineColors($"Send message Error: {e.Message}".AddTimeStamp(),ConsoleColor.DarkRed);   
            }
            
        }
    }
}
