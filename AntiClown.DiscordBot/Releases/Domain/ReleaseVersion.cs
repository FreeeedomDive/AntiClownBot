namespace AntiClown.DiscordBot.Releases.Domain;

public class ReleaseVersion : IComparable
{
    public int Major { get; init; }
    public int Minor { get; init; }
    public int Patch { get; init; }
    public string Description { get; init; } = null!;
    public DateTime CreatedAt { get; init; }

    public override bool Equals(object? obj)
    {
        return CompareTo(obj) == 0;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Major, Minor, Patch);
    }

    public int CompareTo(object? obj)
    {
        if (obj is not ReleaseVersion otherReleaseVersion)
        {
            return 1;
        }

        return Major != otherReleaseVersion.Major
            ? Major.CompareTo(otherReleaseVersion.Major)
            : Minor != otherReleaseVersion.Minor
                ? Minor.CompareTo(otherReleaseVersion.Minor)
                : Patch != otherReleaseVersion.Patch
                    ? Patch.CompareTo(otherReleaseVersion.Patch)
                    : 0;
    }

    public override string ToString()
    {
        return $"{Major}.{Minor}.{Patch}";
    }
}