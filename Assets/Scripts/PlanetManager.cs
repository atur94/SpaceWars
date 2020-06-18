using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;

public class PlanetManager : MonoBehaviour
{
    public List<SpawnerAuthoring> spawners;

    public SpawnerAuthoring currentlySelectedSpawner;

    public EntityManager entityManager;
    private EntityCommandBuffer commandBuffer;
    private EntityCommandBufferSystem commandBufferSystem;

    // Use this for initialization
    void Awake()
    {
        spawners = new List<SpawnerAuthoring>();
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        commandBufferSystem = World.DefaultGameObjectInjectionWorld
            .GetOrCreateSystem<EntityCommandBufferSystem>();
        commandBuffer = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<EntityCommandBufferSystem>()
            .CreateCommandBuffer();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
            var collider = hit.collider;
            if (collider != null)
            {
                if (collider.gameObject.tag.Equals("Background"))
                {
                    currentlySelectedSpawner = null;
                    foreach (var spawnerAuthoring in spawners)
                    {
                        if (entityManager.HasComponent<SelectedTag>(spawnerAuthoring.spawnerEntity))
                        {
                            entityManager.RemoveComponent<SelectedTag>(spawnerAuthoring.spawnerEntity);
                        }
                    }
                }else if (collider.gameObject.GetComponent<SpawnerAuthoring>() is SpawnerAuthoring spawnerTemp)
                {
                    currentlySelectedSpawner = spawnerTemp;
                    foreach (var spawnerAuthoring in spawners)
                    {
                        if (entityManager.HasComponent<SelectedTag>(spawnerAuthoring.spawnerEntity))
                        {
                            entityManager.RemoveComponent<SelectedTag>(spawnerAuthoring.spawnerEntity);
                        }
                    }
                    entityManager.AddComponent<SelectedTag>(spawnerTemp
                        .spawnerEntity);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            if (currentlySelectedSpawner != null)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
                var collider = hit.collider;
                if (collider.GetComponent<SpawnerAuthoring>() is SpawnerAuthoring attackedSpawner && attackedSpawner != currentlySelectedSpawner)
                {
                    Debug.Log("Zaaakuj en spawner");

                    var commandBuff = commandBufferSystem.CreateCommandBuffer();
                    var spawnedEntity = commandBuff.Instantiate(SpaceWarsEntities.shipFirstEntity);
                    commandBuff.SetComponent(spawnedEntity, new Translation
                    {
                        Value = currentlySelectedSpawner.transform.position,
                    });

                    commandBuff.SetComponent(spawnedEntity, new TargetSelector
                    {
                        PrimaryTranslation = attackedSpawner.transform.position,
                        SecondaryTranslation = attackedSpawner.transform.position,
                    });
                }
            }
        }
    }
}
