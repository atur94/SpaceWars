using Unity.Entities;

public struct EnemiesInRange : IComponentData
{
    public int Value;
}

public struct ChangeEnemiesInRange : IComponentData
{
    public Entity spawner;
    public bool added;
}

public struct PlanetInDanger : IComponentData
{
}