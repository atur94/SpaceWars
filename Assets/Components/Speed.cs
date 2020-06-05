using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[GenerateAuthoringComponent]
public struct Speed : IComponentData
{
    public float maxVelocity;
    public float currentVelocity;
    public float maxAcceleration;
    public float currentAcceleration;
    public float2 currentVector;
}