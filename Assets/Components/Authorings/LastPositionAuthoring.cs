using UnityEngine;
using System.Collections;
using Unity.Entities;

public class LastPositionAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new LastPosition());
    }
}
