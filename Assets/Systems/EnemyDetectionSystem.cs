using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine.Jobs;

public struct UnitInRange : IComponentData
{
    public Entity planetEntity;
}

public class EnemyDetectionSystem : ComponentSystem
{
    private EntityQuery unitQuery;
    private EntityQuery planetQuery;

    [BurstCompile]
    public struct EnemyDetectionJob : IJobChunk
    {
        public NativeArray<Translation> planetsTranslations;
        public NativeArray<Entity> planetEntities;
        [ReadOnly] public float detectionRange;
        [ReadOnly] public ArchetypeChunkComponentType<Translation> unitsTranslationsType;
        [ReadOnly] public ArchetypeChunkComponentType<TargetSelector> unitsTargetsType;
        [ReadOnly] public ArchetypeChunkEntityType entitiesTypes;
        public EntityCommandBuffer.Concurrent entityCommandBuffer;
        public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
        {
            var unitTranslations = chunk.GetNativeArray(unitsTranslationsType);
            var unitTargets = chunk.GetNativeArray(unitsTargetsType); 
            var entitiesChunk = chunk.GetNativeArray(entitiesTypes);

            for (int i = 0; i < planetsTranslations.Length; i++)
            {
                for (int j = 0; j < unitTargets.Length; j++)
                {
                    var distance = math.distance(planetsTranslations[i].Value, unitTranslations[j].Value);
                    if (distance < detectionRange && planetEntities[i] == unitTargets[j].Primary)
                    {
                        entityCommandBuffer.AddComponent(chunkIndex,entitiesChunk[j], new UnitInRange{planetEntity = planetEntities[i]});
                    }
                }
            }
        }
    }

    private EntityCommandBufferSystem commandBufferSystem;
    private EntityManager entityManager;
    protected override void OnCreate()
    {
        commandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        unitQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new[] {ComponentType.ReadOnly<Translation>(), ComponentType.ReadOnly<UnitGroup>(), ComponentType.ReadOnly<TargetSelector>()},
            None = new []{ComponentType.ReadOnly<UnitInRange>() }
        });

        planetQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new[] {ComponentType.ReadOnly<Translation>(), ComponentType.ReadOnly<Spawner>(), ComponentType.ReadOnly<UnitDetectionRange>()}
        });

    }

    private JobHandle dependency;
    protected override void OnUpdate()
    {
        EnemyDetectionJob enemyDetectionJob = new EnemyDetectionJob();
        var planetTranslations = planetQuery.ToComponentDataArray<Translation>(Allocator.TempJob);
        var planetEntities = planetQuery.ToEntityArray(Allocator.TempJob);
        enemyDetectionJob.planetsTranslations = planetTranslations;
        enemyDetectionJob.planetEntities = planetEntities; 
        enemyDetectionJob.detectionRange = 20f;
        enemyDetectionJob.unitsTranslationsType = GetArchetypeChunkComponentType<Translation>(true);
        enemyDetectionJob.unitsTargetsType = GetArchetypeChunkComponentType<TargetSelector>(true);
        enemyDetectionJob.entitiesTypes = GetArchetypeChunkEntityType();
        var ecb = commandBufferSystem.CreateCommandBuffer().ToConcurrent();
        enemyDetectionJob.entityCommandBuffer = ecb;
        commandBufferSystem.AddJobHandleForProducer(dependency);

        var scheduledJobHandle = enemyDetectionJob.ScheduleParallel(unitQuery, dependency);

        dependency = JobHandle.CombineDependencies(scheduledJobHandle, dependency);

        dependency.Complete();
        
        planetEntities.Dispose();
        planetTranslations.Dispose();
    }
}