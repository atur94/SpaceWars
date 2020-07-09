

using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Rendering;
using Unity.Transforms;
using UnityEditor;
using UnityEngine;
using Ray = UnityEngine.Ray;
using RaycastHit = Unity.Physics.RaycastHit;

public class EntitySpawnerSystem : ComponentSystem
{
    private EntityQuery spawnerQuery;
    private EntityQuery commandsQuery;
    private EntityCommandBufferSystem commandBufferSystem;

    protected override void OnCreate()
    {
        commandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        spawnerQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new [] {ComponentType.ReadOnly<Spawner>(), ComponentType.ReadOnly<Translation>(), ComponentType.ReadOnly<PlanetOwner>(), ComponentType.ReadWrite<PlanetUnitsBuffer>(),   }
        });

        commandsQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new[] {ComponentType.ReadOnly<SpawnUnitCommand>(),}
        });
    }

    private JobHandle dependency;

    protected override void OnUpdate()
    {

//        var em = World.DefaultGameObjectInjectionWorld.EntityManager;
//        Entities.ForEach((Entity command, ref SpawnUnitCommand spawnCommand) =>
//        {

//            float3 startPosition = float3.zero;
//            float3 endPosition = float3.zero;
//            var unitCommand = spawnCommand;
//            Entities.ForEach((Entity spawner,  ref Translation translation) =>
//            {
//                if (spawner == unitCommand.source)
//                {
//                    startPosition = translation.Value;
//                }
//                if (spawner == unitCommand.target)
//                {
//                    endPosition = translation.Value;
//                }
//            });

//            var unitBuffer = em.GetBuffer<PlanetUnitsBuffer>(spawnCommand.source);
//            var spawnedEntity = em.Instantiate(unitBuffer[0].shipKind);
//            em.SetComponentData(spawnedEntity, new TargetSelector
//            {
//                parent = spawnCommand.source,
//                Primary = spawnCommand.target,
//                PrimaryTranslation = endPosition,
//            });
//            em.SetComponentData(spawnedEntity, new Translation
//            {
//                Value = startPosition
//            });
////            unitBuffer.RemoveAt(0);
//            em.DestroyEntity(command);
//        });
    }

}

