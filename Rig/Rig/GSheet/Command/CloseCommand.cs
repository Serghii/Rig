using Google.Apis.Sheets.v4.Data;

namespace Rig
{
    internal class CloseCommand : BaseComand, ISendComand
    {
        public CloseCommand(MyPage page, ILastIdComand row) : base(page)
        {
            this.row = row;
            backgroundColor = new Color { Blue = 0.8f, Red = 1, Green = 0.8f, Alpha = 0.01f };
        }

        private ILastIdComand row;

        public void Send(string messege)
        {
            if (string.IsNullOrEmpty(messege))
                CreateRequest(row.lineId, ComandType.Close.ToString());
            else
                CreateRequest(row.lineId, ComandType.Close.ToString(), messege);
            AddToRequestB2("Off");
            SendRequests();
        }
    }
}
