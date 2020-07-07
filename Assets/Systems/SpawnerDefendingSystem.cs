using Unity.Entities;

public class SpawnerDefendingSystem : ComponentSystem
{
    private EntityQuery unitsInRangeQuery;
    private EntityQuery planetsInDangerQuery;

    protected override void OnCreate()
    {
        base.OnCreate();
        
    }

    protected override void OnUpdate()
    {
        
    }
}