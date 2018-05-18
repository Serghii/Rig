using Google.Apis.Sheets.v4.Data;

namespace Rig
{
    internal class RestartCommand : BaseComand, ISendComand
    {
        private ILastIdComand row;

        public RestartCommand(MyPage page, ILastIdComand row) : base(page)
        {
            this.row = row;
            backgroundColor = new Color { Blue = 0.9f, Red = 1, Green = 0.9f, Alpha = 0.01f };
        }

        public void Send(string messege)
        {
            if (string.IsNullOrEmpty(messege))
                CreateRequest(row.lineId,ComandType.Restart.ToString());
            else
                CreateRequest(row.lineId, ComandType.Restart.ToString(),messege);
            AddToRequestB2("Restart");
            SendRequests();
        }
    }
}
