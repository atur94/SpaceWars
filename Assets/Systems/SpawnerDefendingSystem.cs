using Unity.Entities;

public class SpawnerDefendingSystem : ComponentSystem
{
    private EntityQuery unitsInRangeQuery;
    private EntityQuery planetsInDangerQuery;

    protected override void OnCreate()
    {
        unitsInRangeQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new[] { ComponentType.ReadOnly<UnitInRange>() }
        });

    }

    protected override void OnUpdate()
    {
        
    }
}