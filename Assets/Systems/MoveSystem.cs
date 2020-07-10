using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using RaycastHit = Unity.Physics.RaycastHit;

public class MoveSystem : JobComponentSystem
{

//    protected override void OnUpdate()
//    {
//        var breakDistanceDivider = 6f;
//
//
//        Entities.ForEach((ref Translation translation, ref Velocity velocity, ref TargetSelector targetSelector,
//            ref LocalToWorld localToWorld) =>
//        {
//            var currentMoveTarget = targetSelector.SecondaryTranslation.xy;
//            var currentPosition = localToWorld.Position.xy;
//
//            var moveDirection = currentMoveTarget - currentPosition;
//            var moveDirectionNormalized = math.normalize(moveDirection);
//            var distanceBetweenCurrentPosAndTarget = math.length(moveDirection);
//
//            //Calculate acceleration based on current velocity. MaxAcceleraion/CurrentVelocity
//
//
//            if (distanceBetweenCurrentPosAndTarget > velocity.currentVelocity / breakDistanceDivider)
//            {
//                if (velocity.currentVelocity > 1f)
//                    velocity.currentAcceleration = (velocity.maxVelocity - velocity.currentVelocity) /
//                                                   velocity.maxVelocity * velocity.maxAcceleration;
//                else
//                    velocity.currentAcceleration = velocity.maxAcceleration;
//                if (velocity.currentVelocity < 1f && distanceBetweenCurrentPosAndTarget < 0.3f)
//                {
//                    velocity.currentAcceleration = 0f;
//                    velocity.currentVelocity = 0f;
//                }
//
//                // Parameter
//            }
//            else
//            {
//                velocity.currentAcceleration = -velocity.currentVelocity * breakDistanceDivider;
//            }
//
//            velocity.currentVector = math.lerp(velocity.currentVector, moveDirectionNormalized, 0.04f);
//            Debug.DrawRay(localToWorld.Position, localToWorld.Up);
//        });
        /*
            Entities.ForEach((ref Translation translation, ref TargetSelector targetSelector, ref Velocity speed,
                ref PhysicsVelocity velocity) =>
            {
                float2 currentMoveTarget = targetSelector.SecondaryTranslation.Value.xy;
                float2 currentPosition = translation.Value.xy;

                var moveDirection = currentMoveTarget - currentPosition;
                var moveDirectionNormalized = math.normalize(moveDirection);
                var distanceBetweenCurrentPosAndTarget = math.length(moveDirection);


                var currentVelocity = math.length(velocity.Linear);
                float vel;
                if (distanceBetweenCurrentPosAndTarget < 3f)
                {
                    vel = distanceBetweenCurrentPosAndTarget/3f * speed.maxVelocity;

                }
                else
                {
                    vel = speed.maxVelocity;
                }

                currentVelocity += speed.currentAcceleration * Time.DeltaTime;

                if (currentVelocity > speed.maxVelocity) currentVelocity = speed.maxVelocity;

                speed.currentVector = math.lerp(speed.currentVector, moveDirectionNormalized, 0.04f);
                //
                //                translation.Value.x += speed.currentVelocity * speed.currentVector.x * deltaTime;
                //                translation.Value.y += speed.currentVelocity * speed.currentVector.y * deltaTime;

                velocity.Linear.x = vel * speed.currentVector.x;
                velocity.Linear.y = vel * speed.currentVector.y;
            });*/
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
        var breakDistanceDivider = 6f;


        return Entities.ForEach((ref Translation translation, ref Velocity velocity, ref TargetSelector targetSelector,
            ref LocalToWorld localToWorld) =>
        {
            var currentMoveTarget = targetSelector.SecondaryTranslation.xy;
            var currentPosition = localToWorld.Position.xy;
            
            var moveDirection = currentMoveTarget - currentPosition;
            var moveDirectionNormalized = math.normalize(moveDirection);
            var distanceBetweenCurrentPosAndTarget = math.length(moveDirection);

            //Calculate acceleration based on current velocity. MaxAcceleraion/CurrentVelocity
            

            if (distanceBetweenCurrentPosAndTarget > velocity.currentVelocity / breakDistanceDivider)
            {
                if (velocity.currentVelocity > 1f)
                    velocity.currentAcceleration = (velocity.maxVelocity - velocity.currentVelocity) /
                                                   velocity.maxVelocity * velocity.maxAcceleration;
                else
                {
                    velocity.currentAcceleration = velocity.maxAcceleration;
                }
                if (velocity.currentVelocity < 1f && distanceBetweenCurrentPosAndTarget < 0.3f)
                {
                    velocity.currentAcceleration = 0f;
                    velocity.currentVelocity = 0f;
                }

                // Parameter
            }
            else
            {
                velocity.currentAcceleration = -velocity.currentVelocity * breakDistanceDivider;
            }
            
            velocity.currentVector = math.lerp(velocity.currentVector, moveDirectionNormalized, 0.02f);
//            Debug.DrawRay(localToWorld.Position, localToWorld.Up);
        }).Schedule(inputDeps);
        }

    //    protected override JobHandle OnUpdate(JobHandle inputDeps)
    //    {
    //        float time = (float)Time.ElapsedTime;
    //        float deltaTime = Time.DeltaTime;
    //        float acceleration = 1f;
    //
    //        return Entities.ForEach((ref Translation translation, ref TargetSelector targetSelector, ref Velocity speed) =>
    //        {
    //            float2 currentMoveTarget = targetSelector.SecondaryTranslation.Value.xy;
    //            float2 currentPosition = translation.Value.xy;
    //
    //            var moveDirection = currentMoveTarget - currentPosition;
    //            var distanceBetweenCurrentPosAndTarget = math.length(moveDirection);
    //
    //            //Calculate acceleration based on current velocity. MaxAcceleraion/CurrentVelocity
    //            if (distanceBetweenCurrentPosAndTarget > 0.2f)
    //            {
    //                
    //                translation.Value.x += moveDirection.x * deltaTime;
    //                translation.Value.y += moveDirection.y * deltaTime;
    //            }
    //
    //
    //            //            translation.Value.x += 10  * math.sin(Time.DeltaTime) * Time.DeltaTime;
    //        }).Schedule(inputDeps);
    //    }

        
    private Entity Raycast(float3 fromPosition, float3 toPosition)
    {
        var buildPhysicsWorld = World.DefaultGameObjectInjectionWorld.GetExistingSystem<BuildPhysicsWorld>();
        var collisionWorld = buildPhysicsWorld.PhysicsWorld.CollisionWorld;

        var raycastInput = new RaycastInput
        {
            Start = fromPosition,
            End = toPosition,
            Filter = new CollisionFilter
            {
                BelongsTo = ~0u,
                CollidesWith = ~0u,
                GroupIndex = 0
            }
        };

        var raycastHit = new RaycastHit();
        if (collisionWorld.CastRay(raycastInput, out raycastHit))
        {
            var entityHit = buildPhysicsWorld.PhysicsWorld.Bodies[raycastHit.RigidBodyIndex].Entity;
            return entityHit;
        }

        return Entity.Null;
    }
}

//public class PhysicalVelocityRemovalSystem
//{
//    protected override void OnUpdate()
//    {
//        Entities.ForEach((Entity Entity, ref PhysicsVelocity PhysicsVelocity) =>
//            {
//                EntityManager.RemoveComponent<PhysicsVelocity>(Entity);
//            });
//    }
//}