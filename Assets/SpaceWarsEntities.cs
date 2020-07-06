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
    public static List<Unit> availableUnits;
    public static List<Entity> shipEntities;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        availableUnits = new List<Unit>();
        shipEntities = new List<Entity>();
        defaultUnitEntity = conversionSystem.GetPrimaryEntity(defaultUnit);
        shipFirstEntity = conversionSystem.GetPrimaryEntity(shipFirst);

        availableUnits.AddRange(Resources.LoadAll<Unit>("Prefabs/Ships"));

        foreach (var availableUnit in availableUnits)
        {
            var entityTemp = conversionSystem.GetPrimaryEntity(availableUnit.prefab);
            World.DefaultGameObjectInjectionWorld.EntityManager.AddComponentData(entityTemp, new UnitGroup
            {
                Value = availableUnit.key,
            });
            shipEntities.Add(entityTemp);
        }

    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(defaultUnit);
        referencedPrefabs.Add(shipFirst);
        var availableUnits = Resources.LoadAll<Unit>("Prefabs/Ships");

        foreach (var availableUnit in availableUnits)
        {
            referencedPrefabs.Add(availableUnit.prefab);
        }
    }
}
