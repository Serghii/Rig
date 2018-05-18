

namespace Rig
{
    public interface IName
    {
        string Name { get; }
    }
    public interface IServer: IName
    {
        bool isActive { get; set; }
    }

    public interface IToken : IName
    {
        long Id { get; }
    }
    public class ServerInfo :  IToken, IServer
    {
        public ServerInfo(long idToken, string name, bool isActive)
        {
            Id = idToken;
            Name = name;
            this.isActive = isActive;
        }

        public ServerInfo(string name, bool isActive)
        {
            Name =GShSettings.PCname;
            this.isActive = true;
            Id = 0;
        }

        public ServerInfo() : this(GShSettings.PCname, true){}

        public string Name { get; private set; }
        public bool isActive { get; set; }

        public long Id { get; private set; }
    }
}