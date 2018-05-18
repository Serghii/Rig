namespace Rig
{
    class SendComand : BaseComand, ISendComand
    {
        public SendComand(MyPage page, int row, int col) : base(page)
        {
            this.row = row;
            this.col = col;
        }

        public int row;
        public int col;
        private readonly bool timeStamp;
       

        public void Send(string message)
        {
            CreateRequest(row,col, message);
            SendRequests();
        }
    }
}
