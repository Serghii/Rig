public interface ICoin
{
    int Hashrate { get; }
    string Name { get; }
}

public class Coin: ICoin
{
        public int Hashrate { get; set; }
        public string Name { get; set; }
        public Coin(string name, int Hs)
        {
            Hashrate = Hs;
            Name = name;
        }

   
    public override bool Equals(object obj)
    {
        ICoin c = obj as ICoin;
        return Name == c?.Name;
    }

    public override int GetHashCode()
    {
        int hash = 13;
        hash = (hash * 7) + Name.GetHashCode();
        hash = (hash * 7) + Hashrate.GetHashCode();
        return hash;

    }
}

