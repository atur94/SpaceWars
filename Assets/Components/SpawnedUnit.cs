using Unity.Entities;
using Unity.Mathematics;

public struct SpawnedUnit : IComponentData
{
    public Entity spawnedUnit;
    public byte team;
    public float3 position;
}