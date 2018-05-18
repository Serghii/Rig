using Google.Apis.Sheets.v4.Data;

namespace Rig
{
    class WakeUpCommand:BaseComand, ISendComand
    {
        private ILastIdComand row;
        public WakeUpCommand(MyPage page,ILastIdComand row) : base(page)
        {
            this.row = row;
            backgroundColor = new Color{Blue = 0.9f, Red = 0.9f, Green = 1,Alpha = 0.01f};
        }

        public void Send(string messege)
        {
            if (string.IsNullOrEmpty(messege))
                CreateRequest(row.lineId, ComandType.WakeUp.ToString());
            else
                CreateRequest(row.lineId, ComandType.WakeUp.ToString(), messege);
            AddToRequestB2("On");
            SendRequests();
        }
    }
}
