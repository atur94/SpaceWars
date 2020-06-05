using Unity.Entities;
using UnityEngine;

public class Unit : MonoBehaviour, IConvertGameObjectToEntity
{

    public float acceleration = 1f;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
    }
}
