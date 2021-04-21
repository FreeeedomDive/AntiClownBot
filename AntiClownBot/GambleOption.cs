namespace AntiClownBot
{
    public class GambleOption
    {
        public readonly string Name;
        public readonly double Ratio;

        public GambleOption(string name, double ratio)
        {
            Name = name;
            Ratio = ratio;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (!(obj is GambleOption option)) return false;
            return Name == option.Name;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode() * 3571;
        }

        public override string ToString()
        {
            return $"{Name} : {Ratio}";
        }
    }
}