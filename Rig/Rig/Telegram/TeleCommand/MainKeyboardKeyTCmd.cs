
namespace Rig.Telegram.TeleCommand
{
    class MainKeyboardKeyTCmd : MainKeyboardKTCmd
    {
        public MainKeyboardKeyTCmd(ITCommandService srv) : base(srv){}

        public override string Name => TeleSettings.key;



    }
}
