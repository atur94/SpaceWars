using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class SpawnerAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public Entity spawnerEntity;

    private bool isSelected = false;
    public GameObject unitPrefab;

    public int unitSpawn = 1;

    public float cooldown = 5f;

    void Start()
    {
        FindObjectOfType<PlanetManager>().spawners.Add(this);
    }


    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        
        dstManager.AddComponentData(entity, new Spawner
        {
            entity = conversionSystem.GetPrimaryEntity(unitPrefab),
            cooldownSeconds = cooldown,
        });
        this.spawnerEntity = entity;
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(unitPrefab);
    }
}