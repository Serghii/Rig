using System;

namespace Rig.Telegram
{
    public interface IEventArgs
    {
        string Value { get; }
    }
    public class JsonData : IEventArgs
    {
        public TCmdType type = TCmdType.none;

        public string Value { get; set; }
        public JsonData(string jsonValue)
        {
            string[] data = jsonValue.Split(':');
            if (data == null || data.Length < 1)
            {
                RigEx.WriteLineColors($"Cannot convert value {jsonValue}".AddTimeStamp(), ConsoleColor.Red);
                return;
            }
            if (!Enum.TryParse(data[0], out type))
            {
                RigEx.WriteLineColors($"Cannot convert callback {data[0]}", ConsoleColor.Red);
                return;
            }
            Value = data.Length > 1 ? data[1] : String.Empty;
        }

        public static string Serialize(TCmdType e,string value)
        {
            return $"{((int)e)}:{value}";
        }

        public static string Serialize(TCmdType e)
        {
            return ((int)e).ToString();
        }
    }
    
}