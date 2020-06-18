using UnityEngine;
using System.Collections;
using JetBrains.Annotations;
using Unity.Entities;

public class PlanetScaleAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{

    public float selectionOutlineMultiplier = 1f;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new PlanetScale
        {
            scale = transform.localScale * selectionOutlineMultiplier
        });
    }
}
