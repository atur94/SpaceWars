using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public struct Velocity : IComponentData
{
    public float maxVelocity;
    public float currentVelocity;
    public float maxAcceleration;
    public float currentAcceleration;
    public float2 currentVector;
}