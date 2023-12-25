namespace SpaceBots;

public interface Base
{
}
public class Me : Base
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public uint Credits { get; set; }
    public string CreatedAt { get; set; }
    public bool Registered { get; set; }
    public static string Url => "users/me";
}

public class Resource : Base
{
    public string Id { get; set; }
    public string Name { get; set; }
    public uint Price { get; set; }

    public static string Url => "resources";
}

public class ShipType : Base
{
    public string Id { get; set; }
    public string Name { get; set; }
    public uint Price { get; set; }

    public static string Url => "ship-types";
}

public class FleetOwner
{
    public string Type { get; set; }
    public Guid UserId { get; set; }
}

public class Fleet : Base
{
    public Guid Id { get; set; }
    public FleetOwner Owner { get; set; }
    public string LocationSystemId { get; set; }
    public string CurrentAction { get; set; }
    public object Cargo { get; set; }
    public Dictionary<string, uint> Ships { get; set; }
}

public class NeighboringSystem
{
    public string SystemId { get; set; }
}
public class SpaceSystem : Base
{
    public string Id { get; set; }
    public string Name { get; set; }
    public NeighboringSystem[] NeighboringSystems { get; set; }
    public Dictionary<string, bool> Station { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
}