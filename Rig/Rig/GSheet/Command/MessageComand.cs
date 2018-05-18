namespace Rig
{
    public class MessageComand :BaseComand, ISendComand
    {
        private ILastIdComand row;
        public MessageComand(MyPage page, ILastIdComand lastIdLine):base(page)
        {
            row = lastIdLine;
        }

        public void Send(string messege)
        {
            CreateRequest(row.lineId,ComandType.Message.ToString(),messege);
            SendRequests();
        }
    }
}
