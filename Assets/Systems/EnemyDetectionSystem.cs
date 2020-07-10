using System.ComponentModel;
using System.Numerics;
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

[UpdateBefore(typeof(CollisionSystem))]
public class EnemyDetectionSystem : ComponentSystem
{
    private EntityQuery unitQuery;
    private EntityQuery planetQuery;
    private EntityQuery arenaQuery;
    private EntityQueryDesc arenaQueryDesc;
    private EntityQuery unitsInRange;

    [BurstCompile]
    public struct EnemyDetectionJob : IJobChunk
    {
        public NativeArray<Translation> planetsTranslations;
        public NativeArray<Entity> planetEntities;
        public NativeArray<PlanetOwner> planetOwners;
        [Unity.Collections.ReadOnly] public float detectionRange;
        [Unity.Collections.ReadOnly] public ArchetypeChunkComponentType<Translation> unitsTranslationsType;
        [Unity.Collections.ReadOnly] public ArchetypeChunkComponentType<TargetSelector> unitsTargetsType;
        [Unity.Collections.ReadOnly] public ArchetypeChunkComponentType<UnitInRange> unitsInRangeType;
        [Unity.Collections.ReadOnly] public ArchetypeChunkEntityType entitiesTypes;
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
                    if (planetEntities[i] == unitTargets[j].Primary)
                    {
                        var distance = math.distance(planetsTranslations[i].Value, unitTranslations[j].Value);
                        if (distance < detectionRange)
                        {
                            entityCommandBuffer.AddComponent(chunkIndex, entitiesChunk[j], new UnitInRange { planetEntity = planetEntities[i] });
                        }
                        else
                        {
                            entityCommandBuffer.RemoveComponent(chunkIndex, entitiesChunk[j], ComponentType.ReadOnly<UnitInRange>());
                        }
                    }
                }
            }
        }
    }

    public struct PlanetInDangerDetectionJob : IJobParallelFor
    {
        public EntityCommandBuffer.Concurrent entityCommandBuffer;
        public NativeArray<UnitInRange> unitsInRange;
        public NativeArray<Entity> planetEntities;
        public NativeArray<Entity> arenaEntities;
        public NativeArray<Arena> activeArenas;
        public NativeArray<Translation> activeArenaTranslations;
        [Unity.Collections.ReadOnly] public BufferFromEntity<UnitsBufferElement> buffers;


        public void Execute(int index)
        {
            var currentPlanet = planetEntities[index];
            bool isInDanger = false;

            for (int i = 0; i < unitsInRange.Length; i++)
            {
                if (unitsInRange[i].planetEntity == currentPlanet && unitsInRange[i].planetEntity == currentPlanet)
                {
                    isInDanger = true;
                }
            }

            if (isInDanger)
            {
                entityCommandBuffer.AddComponent<PlanetInDanger>(index, planetEntities[index]);

            }
            else
            {
                entityCommandBuffer.RemoveComponent<PlanetInDanger>(index, planetEntities[index]);
            }
        }
    }

    private EntityManager entityManager;
    protected override void OnCreate()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        unitQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new[] {ComponentType.ReadOnly<Translation>(), ComponentType.ReadOnly<UnitGroup>(), ComponentType.ReadOnly<TargetSelector>(), ComponentType.ReadOnly<UnitOwner>(), },
//            None = new []{ComponentType.ReadOnly<UnitInRange>() }
        });

        planetQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new[] {ComponentType.ReadOnly<Translation>(), ComponentType.ReadOnly<Spawner>(), ComponentType.ReadOnly<UnitDetectionRange>(), ComponentType.ReadOnly<PlanetOwner>(),  },
        });

        unitsInRange = GetEntityQuery(new EntityQueryDesc
        {
            All = new[] { ComponentType.ReadOnly<UnitInRange>(),ComponentType.ReadOnly<UnitOwner>(),  },
        });

        arenaQueryDesc = new EntityQueryDesc
        {
            All = new[] { ComponentType.ReadOnly<Arena>(), ComponentType.ReadOnly<Translation>(), ComponentType.ReadOnly<UnitsBufferElement>(),  },
        };
        arenaQuery = GetEntityQuery(arenaQueryDesc);
    }

    private JobHandle dependency;
    protected override void OnUpdate()
    {
        EnemyDetectionJob enemyDetectionJob = new EnemyDetectionJob();
        var planetTranslations = planetQuery.ToComponentDataArray<Translation>(Allocator.TempJob);
        var planetEntities = planetQuery.ToEntityArray(Allocator.TempJob);
        var planetOwners = planetQuery.ToComponentDataArray<PlanetOwner>(Allocator.TempJob);

        enemyDetectionJob.planetOwners = planetOwners;
        enemyDetectionJob.planetsTranslations = planetTranslations;
        enemyDetectionJob.planetEntities = planetEntities;
        enemyDetectionJob.detectionRange = 25f;
        enemyDetectionJob.unitsInRangeType = GetArchetypeChunkComponentType<UnitInRange>(true);
        enemyDetectionJob.unitsTranslationsType = GetArchetypeChunkComponentType<Translation>(true);
        enemyDetectionJob.unitsTargetsType = GetArchetypeChunkComponentType<TargetSelector>(true);
        enemyDetectionJob.entitiesTypes = GetArchetypeChunkEntityType();
        enemyDetectionJob.entityCommandBuffer = PostUpdateCommands.ToConcurrent();

        var scheduledJobHandle = enemyDetectionJob.ScheduleParallel(unitQuery, dependency);

        dependency = JobHandle.CombineDependencies(scheduledJobHandle, dependency);
        dependency.Complete();


        var unitsInRangeArray = unitsInRange.ToComponentDataArray<UnitInRange>(Allocator.TempJob);
        var arenasEntities = arenaQuery.ToEntityArray(Allocator.TempJob);
        var activeArenas = arenaQuery.ToComponentDataArray<Arena>(Allocator.TempJob);
        var activeArenasTranslation = arenaQuery.ToComponentDataArray<Translation>(Allocator.TempJob);
        var activeArenasBuffers = GetBufferFromEntity<UnitsBufferElement>(true);

        PlanetInDangerDetectionJob planetInDangerDetectionJob = new PlanetInDangerDetectionJob();
        planetInDangerDetectionJob.entityCommandBuffer = PostUpdateCommands.ToConcurrent();
        planetInDangerDetectionJob.planetEntities = planetEntities;
        planetInDangerDetectionJob.unitsInRange = unitsInRangeArray;
        planetInDangerDetectionJob.arenaEntities = arenasEntities;
        planetInDangerDetectionJob.activeArenaTranslations = activeArenasTranslation;
        planetInDangerDetectionJob.activeArenas = activeArenas;
        planetInDangerDetectionJob.buffers = activeArenasBuffers;

        var planetInDangerJobHandle = planetInDangerDetectionJob.Schedule(planetEntities.Length, 4, dependency);
        planetInDangerJobHandle.Complete();

        planetOwners.Dispose();
        planetEntities.Dispose();
        planetTranslations.Dispose();
        unitsInRangeArray.Dispose();

        arenasEntities.Dispose();
        activeArenas.Dispose();
        activeArenasTranslation.Dispose();

    }
}
