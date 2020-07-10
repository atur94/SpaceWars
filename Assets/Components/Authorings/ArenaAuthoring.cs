using UnityEngine;
using System.Collections;
using Unity.Entities;
using Unity.Transforms;

public struct Arena : IComponentData
{
    public float radius;
    public Entity arenaParent;
}


public class ArenaAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{

    public float radius = 40f;
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new Arena
        {
            radius = radius
        });

        dstManager.AddBuffer<UnitsBufferElement>(entity);
    }
}
