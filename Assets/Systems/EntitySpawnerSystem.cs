

using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Rendering;
using Unity.Transforms;
using UnityEditor;
using UnityEngine;
using Ray = UnityEngine.Ray;
using RaycastHit = Unity.Physics.RaycastHit;

public class EntitySpawnerSystem : ComponentSystem, IDeclareReferencedPrefabs
{
    BeginInitializationEntityCommandBufferSystem m_EntityCommandBufferSystem;

    protected override void OnCreate()
    {
        m_EntityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
    }

    private const int UNITS_SPAWN = 1;
    protected override void OnUpdate()
    {

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D rayHit = Physics2D.Raycast(ray.origin, ray.direction);
            if (rayHit.collider != null)
            {
                var conversionSystem = EntityManager.World.GetOrCreateSystem<GameObjectConversionSystem>();
                var commandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer();
                for (int i = 0; i < UNITS_SPAWN; i++)
                {
                    var entity = conversionSystem.GetPrimaryEntity(PrefabComponentSystem.defaultPrefabStatic);
                    entity = commandBuffer.Instantiate(entity);
                    EntityManager.SetComponentData(entity, new Translation { Value = new float3(rayHit.point + Vector2.right * i * 2, 0f) });

                    return;
                    //                    EntityManager.SetName(spawnedUnit, "UnitGroup");
                }


                // Do something with the object that was hit by the raycast.
            }


        }


        //Entities.ForEach((ref PrefabComponentSystem prefabComponenentSystem) =>
        //{
        //    EntityManager.Instantiate(PrefabComponentSystem.defaultPrefabEntity);
        //});
    }


    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(PrefabComponentSystem.defaultPrefabStatic);
    }
}