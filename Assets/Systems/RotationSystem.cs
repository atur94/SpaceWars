using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class RotationSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        return Entities.ForEach((ref Translation translation, ref Rotation rotation, ref LastPosition lastPosition, ref Speed speed) =>
        {
            float2 direction = (new float2(translation.Value.xy - lastPosition.xy));

            if (speed.currentVelocity > 0.2f)
            {
                //                var angle = math.atan2(direction.y, direction.x);

                var angle = math.acos(math.dot(math.normalize(direction), new float2(0, 1f)));
                if (direction.x < 0) angle *= -1;
                rotation.Value = quaternion.AxisAngle(new float3(0f, 0, 1), -angle);
            }

            lastPosition.xy = translation.Value.xy;

        }).Schedule(inputDeps);
    }
}