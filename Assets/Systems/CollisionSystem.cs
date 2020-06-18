using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;
using Collider = Unity.Physics.Collider;


public class CollisionSystem 
{

    [BurstCompile]
    public struct TriggerJob : ITriggerEventsJob
    {
        public ComponentDataFromEntity<Velocity> physicsVelocityEntities;
        public void Execute(TriggerEvent triggerEvent)
        {
            if (physicsVelocityEntities.HasComponent(triggerEvent.Entities.EntityA))
            {
                Velocity velocity = physicsVelocityEntities[triggerEvent.Entities.EntityA];
                velocity.currentAcceleration = 0f;
                velocity.currentVelocity = 4f;
                velocity.currentVector *= -1;
            }

            if (physicsVelocityEntities.HasComponent(triggerEvent.Entities.EntityB))
            {
                Velocity velocity = physicsVelocityEntities[triggerEvent.Entities.EntityB];
                velocity.currentAcceleration = 0f;
                velocity.currentVelocity = 4f;
                velocity.currentVector *= -1;
            }
        }
    }


//    private BuildPhysicsWorld buidPhysicsWorld;
//    private StepPhysicsWorld stepPhysicsWorld;
//
//    protected void OnCreate()
//    {
//        buidPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
//        stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
//    }
//
//    protected override JobHandle OnUpdate(JobHandle inputDeps)
//    {
//        TriggerJob triggerJob = new TriggerJob
//        {
//            physicsVelocityEntities = GetComponentDataFromEntity<Velocity>()
//        };
//
//        return triggerJob.Schedule(stepPhysicsWorld.Simulation, ref buidPhysicsWorld.PhysicsWorld, inputDeps);
//    }
}