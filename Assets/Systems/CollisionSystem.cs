using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Systems;

[UpdateAfter(typeof(EndFramePhysicsSystem))]
public class CollisionSystem  : JobComponentSystem
{
    public struct TriggerJob : ITriggerEventsJob
    {
        [ReadOnly]public ComponentDataFromEntity<Spawner> spawners;
        public ComponentDataFromEntity<UnitInRange> units;
        public EntityCommandBuffer commandBuffer;
        [ReadOnly]public EntityManager entityManager;
        public void Execute(TriggerEvent triggerEvent)
        {
            Entity entityA = triggerEvent.Entities.EntityA;
            Entity entityB = triggerEvent.Entities.EntityB;
            if (units.Exists(entityA) && spawners.Exists(entityB))
            {
                TriggerEvent(entityB, entityA);
            }
            else if (units.Exists(entityB) && spawners.Exists(entityA))
            {
                TriggerEvent(entityA, entityB);
            }

        }

        private void TriggerEvent(Entity spawner, Entity unit)
        {
            UnitInRange range = units[unit];
            if(range.whatsInRange == spawner)
            {
                commandBuffer.AddComponent(unit, new UnitCanBeSwallowed
                {
                    spawnerEntity = spawner
                });

            }
        }


    }

    private StepPhysicsWorld stepPhysicsWorld;
    private BuildPhysicsWorld buildPhysicsWorld;
    private EntityCommandBufferSystem entityCommandBufferSystem;
    private EntityManager entityManager;
    protected override void OnCreate()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        buildPhysicsWorld = World.DefaultGameObjectInjectionWorld.GetExistingSystem<BuildPhysicsWorld>();
        stepPhysicsWorld = World.DefaultGameObjectInjectionWorld.GetExistingSystem<StepPhysicsWorld>();
        entityCommandBufferSystem = World.DefaultGameObjectInjectionWorld
            .GetExistingSystem<EndSimulationEntityCommandBufferSystem>();

    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var job = new TriggerJob();
        job.spawners = GetComponentDataFromEntity<Spawner>();
        job.units = GetComponentDataFromEntity<UnitInRange>();
        job.entityManager = entityManager;
        job.commandBuffer = entityCommandBufferSystem.CreateCommandBuffer();
        JobHandle jobHandle = job.Schedule(stepPhysicsWorld.Simulation, ref buildPhysicsWorld.PhysicsWorld, inputDeps);
        jobHandle.Complete();

        return jobHandle;
    }
}

[UpdateBefore(typeof(CollisionSystem))]
public class UnitRemovalSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        Entities.WithAll(typeof(UnitCanBeDestroyed)).ForEach((Entity entity) => { entityManager.DestroyEntity(entity); });
    }
}

[UpdateBefore(typeof(CollisionSystem))]
public class UnitSwallowingSystem : ComponentSystem
{
    public delegate void UnitEntered(Entity unit, Entity spawner);

    public event UnitEntered onUnitEntered;

    protected override void OnUpdate()
    {
        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        Entities.ForEach((Entity entity, ref UnitCanBeSwallowed swallowedProperty) =>
        {
            var planetEntity = swallowedProperty.spawnerEntity;
            var unitsBuffer = entityManager.GetBuffer<UnitsBufferElement>(planetEntity);
            unitsBuffer.Add(new UnitsBufferElement
            {
                shipKind = entity
            });

            onUnitEntered?.Invoke(entity, swallowedProperty.spawnerEntity);

                
            entityManager.DestroyEntity(entity);

        });
    }

}