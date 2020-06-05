using System.Diagnostics;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEditor.IMGUI.Controls;
using Random = Unity.Mathematics.Random;

public class MoveSystem : ComponentSystem
{

    float maxSpeed = 10f;
    protected override void OnCreate()
    {
        base.OnCreate();
    }

    
    protected override void OnUpdate()
    {
        float time = (float)Time.ElapsedTime;
        float deltaTime = Time.DeltaTime;


        var breakDistanceDivider = 6f;
        Entities.ForEach((ref Translation translation, ref TargetSelector targetSelector, ref Speed speed) =>
        {
            float2 currentMoveTarget = targetSelector.SecondaryTranslation.Value.xy;
            float2 currentPosition = translation.Value.xy;

            var moveDirection = currentMoveTarget - currentPosition;
            var moveDirectionNormalized = math.normalize(moveDirection);
            var distanceBetweenCurrentPosAndTarget = math.length(moveDirection);

            //Calculate acceleration based on current velocity. MaxAcceleraion/CurrentVelocity

            var radius = 3f;
            if (distanceBetweenCurrentPosAndTarget > speed.currentVelocity / breakDistanceDivider)
            {
                var speedMagnitude = math.length(speed.currentVelocity);
                if (speedMagnitude > 1f)
                {
                    speed.currentAcceleration = ( speed.maxVelocity - speed.currentVelocity ) / speed.maxVelocity * speed.maxAcceleration;
                }
                else
                {
                    speed.currentAcceleration = speed.maxAcceleration;
                }
                // Parameter

                UnityEngine.Debug.DrawRay(translation.Value, new float3(speed.currentVector, 0f));
            }
            else
            {
                speed.currentAcceleration = -speed.currentVelocity * breakDistanceDivider;
            }

            speed.currentVelocity += speed.currentAcceleration * Time.DeltaTime;

            if (speed.currentVelocity > speed.maxVelocity) speed.currentVelocity = speed.maxVelocity;

            speed.currentVector = math.lerp(speed.currentVector, moveDirectionNormalized, 0.04f);

            translation.Value.x += speed.currentVelocity * speed.currentVector.x * deltaTime;
            translation.Value.y += speed.currentVelocity * speed.currentVector.y * deltaTime;

            //            translation.Value.x += 10  * math.sin(Time.DeltaTime) * Time.DeltaTime;
        });
    }
    //    protected override JobHandle OnUpdate(JobHandle inputDeps)
    //    {
    //        float time = (float)Time.ElapsedTime;
    //        float deltaTime = Time.DeltaTime;
    //        float acceleration = 1f;
    //
    //        return Entities.ForEach((ref Translation translation, ref TargetSelector targetSelector, ref Speed speed) =>
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

}

