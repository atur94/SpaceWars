using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using UnityEngine;
using UnityEngine.Rendering;

public struct UnitDetectionRange : ISharedComponentData
{
    public float Value;
}

public struct PlanetUnit
{
    public Entity planet;
}

[InternalBufferCapacity(100)]
public struct PlanetUnitsBuffer : IBufferElementData
{
    public Entity shipKind;
}

public class PlanetAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public float enemyDetectionRange = 20f;
    public GameObject unitPrefab;
    public bool canBeStartedPlanet;

    public Entity spawnerEntity;

    public void Start()
    {
        var spc = GetComponent<SinglePlanetController>();
        spc.spawnerEntity = spawnerEntity;
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        
        dstManager.AddComponentData(entity, new Spawner
        {
        });

        dstManager.AddComponentData(entity, new PlanetOwner());
        var buffer = dstManager.AddBuffer<PlanetUnitsBuffer>(entity);
        dstManager.AddSharedComponentData(entity, new UnitDetectionRange
        {
            Value = enemyDetectionRange
        });
        dstManager.AddComponentData(entity, new EnemiesInRange());


        this.spawnerEntity = entity;
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(unitPrefab);
    }
}