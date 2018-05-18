namespace Rig
{
    class PingComand :BaseComand, ISendComand
    {
        public PingComand(MyPage page):base(page)
        {
            row = 1;
            col = GShSettings.ping.LaterToColum();
        }
        public int row;
        public int col;
        private readonly bool timeStamp;

        public void Send(string messege)
        {
            CreateRequest(row);
            SendRequests();
        }
    }
}
