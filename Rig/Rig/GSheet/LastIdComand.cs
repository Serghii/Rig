namespace Rig
{
    public interface ILastIdComand
    {
        int lineId { get; }
    }
    public class LastIdComand : ILastIdComand
    {
        public int lineId { get ;  set ; }
    }
}