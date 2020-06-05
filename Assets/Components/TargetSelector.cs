using Unity.Entities;
using Unity.Transforms;

[GenerateAuthoringComponent]
public struct TargetSelector : IComponentData
{
    public Entity Primary;
    public Translation PrimaryTranslation;
    public Entity Secondary;
    public Translation SecondaryTranslation;
}