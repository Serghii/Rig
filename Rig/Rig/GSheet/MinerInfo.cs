
public interface IMinerInfo
    {
        string Name { get; }
        string Path { get; }
    }


public class MinerInfo : IMinerInfo
{
        public MinerInfo(string name, string path)
        {
            Name = name;
            Path = path;
        }

    public bool IsActive { get; set; }
    public string Name { get; private set; }
        public string Path { get; private set; }
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            MinerInfo p = (MinerInfo)obj;
            return (Name == p.Name) && (Path == p.Path);
        }

    public override int GetHashCode()
    {
        int hash = 13;
        hash = (hash * 7) + Name.GetHashCode();
        hash = (hash * 7) + Path.GetHashCode();
        return hash;
    }
}

