using Unity.Entities;
using Unity.Jobs;

public class TargetUpdateStateSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        return Entities.ForEach((ref TargetSelector targetSelector) =>
        {

        }).Schedule(inputDeps);
    }
}