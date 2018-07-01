using System;
using System.Text;

namespace Rig.Telegram.TeleCommand
{
    class ShowRatesTCmd : TCommandBase, ITCommand
    {
        private XmrHrate HashRate = new XmrHrate();

        public ShowRatesTCmd(ITCommandService srv) : base(srv)
        {
        }

        public TCmdType Type => TCmdType.showRates;
        public override string Name => TeleSettings.rates;

        public void Execute()
        {
            RigEx.WriteLineColors($"Command  {Name}", ConsoleColor.Yellow);
            StringBuilder sb = new StringBuilder();

            var rates = HashRate.GetHashRate;
            sb.Append($"\nHash Rate:\t {(rates > 0 ? rates.ToString() : "none")}");

            var balance = HashRate.GetBalance;
            var btcPrice = HashRate.GetBTCPrice;
            sb.Append($"\n{Icons.bitok}:{(balance > 0 ? balance.ToString() : "none")}" +
                      $" {Icons.kyrs}: {(btcPrice > 0 ? btcPrice.ToString() : "none")}" +
                      $" {Icons.nalik}: {(balance > 0 && btcPrice > 0 ? (balance * btcPrice).ToString() : "none")}");

            srv.SendMsg(sb.ToString());
        }

        public void Execute(JsonData jd)
        {
            Execute();
        }
    }
}
