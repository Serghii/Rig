using Google.Apis.Sheets.v4.Data;

namespace Rig
{
    class CPUComand : BaseComand, ISendComand
    {
        public CPUComand(MyPage page, ILastIdComand row) : base(page)
        {
            this.row = row;
            backgroundColor = new Color { Blue = 0.9f, Red = 1, Green = 1, Alpha = 0.01f };
        }

        private ILastIdComand row;

        public void Send(string messege)
        {
            if (string.IsNullOrEmpty(messege))
                CreateRequest(row.lineId, ComandType.Alarm.ToString());
            else
                CreateRequest(row.lineId, ComandType.Alarm.ToString(), messege);

            SendRequests();
        }
    }
}
