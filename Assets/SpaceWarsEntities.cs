using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;

[UpdateBefore(typeof(UnitSpawnerSystem))]
public class SpaceWarsEntities : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{

    public GameObject defaultUnit;
    public GameObject shipFirst;
    public static Entity defaultUnitEntity;
    public static Entity shipFirstEntity;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        defaultUnitEntity = conversionSystem.GetPrimaryEntity(defaultUnit);
        shipFirstEntity = conversionSystem.GetPrimaryEntity(shipFirst);
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(defaultUnit);
        referencedPrefabs.Add(shipFirst);
    }
}
