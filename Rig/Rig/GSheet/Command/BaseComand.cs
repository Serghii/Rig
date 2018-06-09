using Google.Apis.Sheets.v4.Data;
using System;
using System.Collections.Generic;

namespace Rig
{
    public enum PcStatus
    {
        none,
        on,
        off
    }
    public abstract class BaseComand
    {
        protected readonly MyPage page;
        protected Color backgroundColor;
        protected PcStatus status = PcStatus.none;


        private List<Request> request ;

        protected BaseComand(MyPage page)
        {
            this.page = page;
        }

        protected void AddToRequestB2(string command)
        {
            request.Add(GetRequest(1, 1, $"_{command}".AddTimeStamp()));
        }
        protected void CreateRequest(int row, int col, string message)
        {
            request = new List<Request>
                        {
                            GetRequest(row, col, message)
                        };
        }
        protected void CreateRequest(int row)
        {
            request = new List<Request>
            {
                GetRequest(row, 0, $"{DateTime.Now:HH:mm:ss tt}")
            };
        }
        protected void CreateRequest(int row, string type)
        {

            request = new List<Request>
                        {
                            GetRequest(row, 0, String.Empty.AddTimeStamp()),
                            GetRequest(row, 1, type)
                        };
        }
        protected void CreateRequest(int row, string type, string message)
        {

            request = new List<Request>
                        {
                            GetRequest(row, 0, String.Empty.AddTimeStamp()),
                            GetRequest(row, 1, type),
                            GetRequest(row, 2, message)
                        };
        }
        protected Request GetRequest(int row, int col, string value)
        {
            List<CellData> values = new List<CellData>();
            Request result;

            if (backgroundColor != null)
            {
                values.Add(new CellData
                {
                    UserEnteredFormat = new CellFormat
                    {
                        BackgroundColor = backgroundColor
                    },
                    UserEnteredValue = new ExtendedValue
                    {
                        StringValue = value

                    }

                });

                result = new Request
                {
                    UpdateCells = new UpdateCellsRequest
                    {
                        Start = new GridCoordinate
                        {
                            SheetId = (int) page.MyServerSheetId.Id,
                            RowIndex = row,
                            ColumnIndex = col
                        },
                        Rows = new List<RowData> {new RowData {Values = values}},
                        Fields = "UserEnteredFormat(BackgroundColor),userEnteredValue"
                    }
                };
            }
            else
            {
                values.Add(new CellData
                {
                    UserEnteredValue = new ExtendedValue
                    {
                        StringValue = value

                    }

                });
                
                result = new Request
                {
                    UpdateCells = new UpdateCellsRequest
                    {
                        Start = new GridCoordinate
                        {
                            SheetId = (int)page.MyServerSheetId.Id,
                            RowIndex = row,
                            ColumnIndex = col
                        },
                        Rows = new List<RowData> {new RowData {Values = values}},
                        Fields = "userEnteredValue"
                    }
                };
            }
            return result;

        }

        protected void SendRequests()
        {
            var busr = new BatchUpdateSpreadsheetRequest { Requests = request };
            try
            {
                var t = page.Service.Spreadsheets.BatchUpdate(busr, page.SpreadsheetId);
                //t.Execute();
                t.ExecuteAsStream();
            }
            catch (Exception e)
            {
                RigEx.WriteLineColors($"Exeption to send request{e.Message}".AddTimeStamp(), ConsoleColor.Red );
                
            }
            
        }

    }
}
