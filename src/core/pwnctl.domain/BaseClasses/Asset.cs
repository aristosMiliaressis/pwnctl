namespace pwnctl.domain.BaseClasses;

using pwnctl.kernel.BaseClasses;

public abstract class Asset : Entity<Guid>, IEquatable<Asset>
{
    public abstract override string ToString();

    public override bool Equals(object? obj)
    {
        return obj is Asset asset && ToString().Equals(asset.ToString());
    }

    public bool Equals(Asset? asset)
    {
        return Equals((object?)asset);
    }

    public static bool operator ==(Asset left, Asset right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(Asset left, Asset right)
    {
        return !Equals(left, right);
    }

    public override int GetHashCode()
    {
        return ToString().GetHashCode();
    }
}
