using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public struct UnitInRange : IComponentData
{
    public Entity whatsInRange;
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
        public NativeArray<Translation> unitTranslationsNativeArray;
        public NativeArray<Entity> unitEntitiesNativeArray;
        [Unity.Collections.ReadOnly] public ComponentDataFromEntity<UnitOwner> unitOwners;
        [Unity.Collections.ReadOnly] public ComponentDataFromEntity<UnitArenaBound> unitAreBound;
        [Unity.Collections.ReadOnly] public ComponentDataFromEntity<UnitInRange> unitIsInRange;
        [Unity.Collections.ReadOnly] public float detectionRange;
        [Unity.Collections.ReadOnly] public ArchetypeChunkComponentType<Translation> unitsTranslationsType;
        [Unity.Collections.ReadOnly] public ArchetypeChunkComponentType<TargetSelector> unitsTargetsType;
        [Unity.Collections.ReadOnly] public ArchetypeChunkComponentType<UnitInRange> unitsInRangeType;
        [Unity.Collections.ReadOnly] public ArchetypeChunkEntityType unitEntitiesTypes;

        public EntityCommandBuffer.Concurrent entityCommandBuffer;
        public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
        {
            var unitTranslations = chunk.GetNativeArray(unitsTranslationsType);
            var unitTargets = chunk.GetNativeArray(unitsTargetsType); 
            var unitEntities = chunk.GetNativeArray(unitEntitiesTypes);

            for (int i = 0; i < planetsTranslations.Length; i++)
            {
                for (int j = 0; j < unitTargets.Length; j++)
                {
                    if (planetEntities[i] == unitTargets[j].Primary)
                    {
                        var distance = math.distance(planetsTranslations[i].Value, unitTranslations[j].Value);
                        if (distance < detectionRange)
                        {
                            if(!unitIsInRange.Exists(unitEntities[j]))
                                entityCommandBuffer.AddComponent(chunkIndex, unitEntities[j], new UnitInRange { whatsInRange = planetEntities[i] });
                            if (!unitAreBound.Exists(unitEntities[j]))
                            {
                                entityCommandBuffer.AddComponent(chunkIndex, unitEntities[j], new UnitArenaBound
                                {
                                    arenaEntity = planetEntities[i]
                                });
                            }
                        }
                        else
                        {
                            entityCommandBuffer.RemoveComponent(chunkIndex, unitEntities[j], ComponentType.ReadOnly<UnitInRange>());
                        }
                    }
                }

            }

            for (int i = 0; i < unitTranslationsNativeArray.Length; i++)
            {
                for (int j = 0; j < unitTranslations.Length; j++)
                {
                    var distance = math.distance(unitTranslations[j].Value, unitTranslationsNativeArray[i].Value);
                    var ownerA = unitOwners[unitEntitiesNativeArray[i]].owner;
                    var ownerB = unitOwners[unitEntities[j]].owner;

                    if (distance < detectionRange && ownerB != ownerA)
                    {
                        if(!unitIsInRange.Exists(unitEntities[j]))
                            entityCommandBuffer.AddComponent(chunkIndex, unitEntities[j], new UnitInRange { whatsInRange = unitEntitiesNativeArray[i] });

                        if (!unitAreBound.Exists(unitEntities[j]))
                        {
                            entityCommandBuffer.AddComponent(chunkIndex, unitEntities[j], new UnitArenaBound
                            {
                                arenaEntity = unitEntitiesNativeArray[i],
                            });
                        }
                    }
                }
            }
        }
    }

    public struct PlanetInDangerDetectionJob : IJobParallelFor
    {
        public EntityCommandBuffer.Concurrent entityCommandBuffer;
        [ReadOnly] public NativeArray<UnitInRange> unitsInRange;
        public NativeArray<Entity> planetEntities;
        public NativeArray<Entity> arenaEntities;
        public NativeArray<Arena> activeArenas;
        public NativeArray<Translation> activeArenaTranslations;

        public void Execute(int index)
        {

            var currentPlanet = planetEntities[index];
            bool isInDanger = false;

            if (unitsInRange.Length <= 0) return;
            for (int i = 0; i < unitsInRange.Length - 1; i++)
            {
                if (unitsInRange[i].whatsInRange == currentPlanet)
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
        var unitsTranslationsNativeArray = unitQuery.ToComponentDataArray<Translation>(Allocator.TempJob);
        var unitEntitiesNativeArray = unitQuery.ToEntityArray(Allocator.TempJob);
        enemyDetectionJob.planetOwners = planetOwners;
        enemyDetectionJob.planetsTranslations = planetTranslations;
        enemyDetectionJob.planetEntities = planetEntities;
        enemyDetectionJob.detectionRange = 25f;
        enemyDetectionJob.unitsInRangeType = GetArchetypeChunkComponentType<UnitInRange>(true);
        enemyDetectionJob.unitsTranslationsType = GetArchetypeChunkComponentType<Translation>(true);
        enemyDetectionJob.unitsTargetsType = GetArchetypeChunkComponentType<TargetSelector>(true);
        enemyDetectionJob.unitEntitiesTypes = GetArchetypeChunkEntityType();
        enemyDetectionJob.entityCommandBuffer = PostUpdateCommands.ToConcurrent();
        enemyDetectionJob.unitTranslationsNativeArray = unitsTranslationsNativeArray;
        enemyDetectionJob.unitEntitiesNativeArray = unitEntitiesNativeArray;
        enemyDetectionJob.unitOwners = GetComponentDataFromEntity<UnitOwner>();
        enemyDetectionJob.unitIsInRange = GetComponentDataFromEntity<UnitInRange>();
        enemyDetectionJob.unitAreBound = GetComponentDataFromEntity<UnitArenaBound>();
        var scheduledJobHandle = enemyDetectionJob.ScheduleParallel(unitQuery, dependency);

        dependency = JobHandle.CombineDependencies(scheduledJobHandle, dependency);
        dependency.Complete();


        var unitsInRangeArray = unitsInRange.ToComponentDataArray<UnitInRange>(Allocator.TempJob);
        var arenasEntities = arenaQuery.ToEntityArray(Allocator.TempJob);
        var activeArenas = arenaQuery.ToComponentDataArray<Arena>(Allocator.TempJob);
        var activeArenasTranslation = arenaQuery.ToComponentDataArray<Translation>(Allocator.TempJob);

        PlanetInDangerDetectionJob planetInDangerDetectionJob = new PlanetInDangerDetectionJob();
        planetInDangerDetectionJob.entityCommandBuffer = PostUpdateCommands.ToConcurrent();
        planetInDangerDetectionJob.planetEntities = planetEntities;
        planetInDangerDetectionJob.unitsInRange = unitsInRangeArray;
        planetInDangerDetectionJob.arenaEntities = arenasEntities;
        planetInDangerDetectionJob.activeArenaTranslations = activeArenasTranslation;
        planetInDangerDetectionJob.activeArenas = activeArenas;
        var planetInDangerJobHandle = planetInDangerDetectionJob.Schedule(planetEntities.Length, 1, dependency);
        planetInDangerJobHandle.Complete();

        planetOwners.Dispose();
        planetEntities.Dispose();
        planetTranslations.Dispose();
        unitsInRangeArray.Dispose();

        arenasEntities.Dispose();
        activeArenas.Dispose();
        activeArenasTranslation.Dispose();
        unitsTranslationsNativeArray.Dispose();
        unitEntitiesNativeArray.Dispose();

    }

}