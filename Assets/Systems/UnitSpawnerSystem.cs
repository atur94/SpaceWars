using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class UnitSpawnerSystem : ComponentSystem
{
    BeginInitializationEntityCommandBufferSystem m_EntityCommandBufferSystem;
    protected override void OnCreate()
    {
        // Cache the BeginInitializationEntityCommandBufferSystem in a field, so we don't have to create it every frame
        m_EntityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
    }

//    protected override JobHandle OnUpdate(JobHandle inputDeps)
//    {
//        // Clamp delta time so you can't overshoot.
//        var deltaTime = math.min(Time.DeltaTime, 0.05f);
//        var commandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer();
//
//        Entities.ForEach((ref Spawner spawner) =>
//        {
//            var secondsUntilGenerate = spawner.secondsUntilGenerate;
//            secondsUntilGenerate -= deltaTime;
//            if (secondsUntilGenerate <= 0)
//            {
//                commandBuffer.Instantiate(spawner.planetEntity);
//                secondsUntilGenerate = spawner.cooldownSeconds;
//            }
//
//            spawner.secondsUntilGenerate = secondsUntilGenerate;
//        }).Run();
//        return inputDeps;
//
//    }

    
    protected override void OnUpdate()
    {

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D rayHit = Physics2D.Raycast(ray.origin, ray.direction);
        if (rayHit.collider != null)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                var commandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer();

                Entities.ForEach((ref Spawner spawner, ref Translation translation) =>
                {
                    var entity = commandBuffer.Instantiate(SpaceWarsEntities.defaultUnitEntity);
                    commandBuffer.SetComponent(entity, new Translation
                    {
                        Value = translation.Value,
                    });
                });
            }
        }



    }
    public struct UnitSpawnJob : IJob
    {
        public int unitsSpawn;
        public Entity spawner;
        public float3 spawnPosition;
        public bool selectionRequired;
        public Entity target;
        public float3 targetPosition;

        public void Execute()
        {

            var commandBuffer = Unity.Entities.World.DefaultGameObjectInjectionWorld
                .GetExistingSystem<BeginInitializationEntityCommandBufferSystem>().CreateCommandBuffer();
            for (int i = 0; i < unitsSpawn; i++)
            {
                var entity = commandBuffer.Instantiate(SpaceWarsEntities.defaultUnitEntity);

                commandBuffer.SetComponent(entity, new Translation
                {
                    Value = spawnPosition
                });

                commandBuffer.SetComponent(entity, new TargetSelector
                {
                    parent = spawner,
                    Primary = target,
                    PrimaryTranslation = targetPosition,
                });
            }
        }
    }
}

