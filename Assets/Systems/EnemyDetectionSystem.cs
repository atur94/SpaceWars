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
        [Unity.Collections.ReadOnly] public ArchetypeChunkComponentType<UnitOwner> unitOwnersType;
        [Unity.Collections.ReadOnly] public ArchetypeChunkComponentType<UnitInRange> unitsInRangeType;
        [Unity.Collections.ReadOnly] public ArchetypeChunkEntityType entitiesTypes;
        public EntityCommandBuffer.Concurrent entityCommandBuffer;
        public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
        {
            var unitTranslations = chunk.GetNativeArray(unitsTranslationsType);
            var unitTargets = chunk.GetNativeArray(unitsTargetsType); 
            var entitiesChunk = chunk.GetNativeArray(entitiesTypes);
            var unitOwners = chunk.GetNativeArray(unitOwnersType);
            var unitsLeft = chunk.GetNativeArray(unitsInRangeType);

            for (int i = 0; i < planetsTranslations.Length; i++)
            {
                bool enemiesWasInRange = false;

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

                for (int j = 0; j < unitsLeft.Length; j++)
                {
                    var jj = unitsLeft[j];
                    if (planetOwners[i].owner != unitOwners[j].owner)
                        enemiesWasInRange = true;
                }
                


            }
        }
    }

    public struct PlanetInDangerDetectionJob : IJobParallelFor
    {
        public EntityCommandBuffer.Concurrent entityCommandBuffer;
        public NativeArray<UnitInRange> unitsInRange;
        public NativeArray<Entity> planetEntities;

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
        enemyDetectionJob.unitOwnersType = GetArchetypeChunkComponentType<UnitOwner>(true);
        enemyDetectionJob.unitsTranslationsType = GetArchetypeChunkComponentType<Translation>(true);
        enemyDetectionJob.unitsTargetsType = GetArchetypeChunkComponentType<TargetSelector>(true);
        enemyDetectionJob.entitiesTypes = GetArchetypeChunkEntityType();
        enemyDetectionJob.entityCommandBuffer = PostUpdateCommands.ToConcurrent();

        var scheduledJobHandle = enemyDetectionJob.ScheduleParallel(unitQuery, dependency);

        dependency = JobHandle.CombineDependencies(scheduledJobHandle, dependency);
        dependency.Complete();


        var unitsInRangeArray = unitsInRange.ToComponentDataArray<UnitInRange>(Allocator.TempJob);
        PlanetInDangerDetectionJob planetInDangerDetectionJob = new PlanetInDangerDetectionJob();
        planetInDangerDetectionJob.entityCommandBuffer = PostUpdateCommands.ToConcurrent();
        planetInDangerDetectionJob.planetEntities = planetEntities;
        planetInDangerDetectionJob.unitsInRange = unitsInRangeArray;

        var planetInDangerJobHandle = planetInDangerDetectionJob.Schedule(planetEntities.Length, 4, dependency);
        planetInDangerJobHandle.Complete();

        planetOwners.Dispose();
        planetEntities.Dispose();
        planetTranslations.Dispose();
        unitsInRangeArray.Dispose();

    }
}
