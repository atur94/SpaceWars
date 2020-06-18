using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public struct TargetSelector : IComponentData
{
    public Entity Primary;
    public float3 PrimaryTranslation;
    public Entity Secondary;
    public float3 SecondaryTranslation;
    public Entity parent;
}