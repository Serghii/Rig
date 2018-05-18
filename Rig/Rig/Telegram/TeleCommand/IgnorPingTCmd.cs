using System;

namespace Rig.Telegram.TeleCommand
{
    class IgnorPingTCmd : TCommandBase, ITCommand
    {
        public IgnorPingTCmd(ITCommandService srv) : base(srv){}

        public TCmdType Type => TCmdType.stopPing;
        public override string Name => TeleSettings.stopPing;

        public void Execute()
        {
            Console.WriteLine("Command stop ping server. Error stop ping => nothin to stop");
        }

        public void Execute(JsonData jd)
        {
            srv.Ctrl.SetPingActivityFor(jd.Value, false);
        }
    }
}
