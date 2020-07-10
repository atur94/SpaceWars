using UnityEngine;
using System.Collections;
using Unity.Entities;
using Unity.Mathematics;

public class VelocityAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public float maxVelocity;
    public float maxAcceleration;
    public float minVelocity;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new Velocity
        {
            maxVelocity = maxVelocity,
            maxAcceleration = maxAcceleration,
            minVelocity = minVelocity
        });
    }
}
