using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

[UpdateAfter(typeof(MoveSystem))]
public class PhysicsMovingSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var deltaTime = Time.DeltaTime;
        return Entities.ForEach((ref Velocity velocity, ref Translation position) =>
        {

            velocity.currentVelocity += velocity.currentAcceleration * deltaTime;

            if (velocity.currentVelocity > velocity.maxVelocity) velocity.currentVelocity = velocity.maxVelocity;
            // TODO Collision
            position.Value.x += velocity.currentVelocity * velocity.currentVector.x * deltaTime;
            position.Value.y += velocity.currentVelocity * velocity.currentVector.y * deltaTime;
        }).Schedule(inputDeps);
    }
}