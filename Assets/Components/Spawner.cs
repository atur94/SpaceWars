using Unity.Entities;
using Unity.Mathematics;

public struct Spawner : IComponentData
{
    public Entity entity;
    public float2 position;
    public float secondsUntilGenerate;
    public float cooldownSeconds;
}