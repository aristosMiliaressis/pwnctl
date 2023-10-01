namespace pwnctl.kernel.BaseClasses;

public abstract class Entity<TPKey> : IEquatable<Entity<TPKey>>
    where TPKey : notnull
{
    protected Entity()
    {
    }

    public TPKey Id { get; set; }

    public override bool Equals(object? obj)
    {
        return obj is Entity<TPKey> entity && Id.Equals(entity.Id);
    }

    public bool Equals(Entity<TPKey>? entity)
    {
        return Equals((object?)entity);
    }

    public static bool operator ==(Entity<TPKey> left, Entity<TPKey> right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(Entity<TPKey> left, Entity<TPKey> right)
    {
        return !Equals(left, right);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}
