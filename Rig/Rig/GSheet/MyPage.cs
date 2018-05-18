using Google.Apis.Sheets.v4;

namespace Rig
{
    public class MyPage
    {
        public MyPage(SheetsService service)
        {
            Service = service;
        }

        public IToken MyServerSheetId ;
        public readonly SheetsService Service;
        public readonly string SpreadsheetId = GShSettings.SpreadsheetId;
        public string ping;
        //public DateTime AlarmDelay;
        //public int PingNotifySec;
        public string lastlineId ;
        public string ServerStatus;
        
    }
}