
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class VelocitySystem
{

//    protected  JobHandle OnUpdate(JobHandle inputDeps)
//    {
//        var deltaTime = Time.DeltaTime;
//        return Entities.ForEach((LastPosition lastPosition, Translation currentPosition, ref Speed speed) =>
//        {
//            
//            speed.currentVelocity = math.length(currentPosition.Value.xy - lastPosition.xy)/deltaTime;
//        }).Schedule(inputDeps);
//    }
}
