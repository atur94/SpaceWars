using UnityEngine;
using System.Collections;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

[UpdateBefore(typeof(TargetSelectionSystem))]
public class UIUpdaterSystem : ComponentSystem
{
    private EntityQuery unitsQuery;
    private EntityQuery isSelectedQuery;

    EntityCommandBuffer commandBuffer;

    protected override void OnCreate()
    {
        unitsQuery = GetEntityQuery(typeof(UnitGroup));
        isSelectedQuery = GetEntityQuery(typeof(SelectedTag), typeof(Translation));
        commandBuffer = World.GetExistingSystem<EntityCommandBufferSystem>().CreateCommandBuffer();

    }

    protected override void OnUpdate()
    {
        var units = unitsQuery.ToComponentDataArray<UnitGroup>(Allocator.Temp);
    }

    public void SpawnUnits(int unitsToSpawn)
    {
        var selectedSpawner = isSelectedQuery.ToEntityArray(Allocator.TempJob);
        if (selectedSpawner.Length > 0)
        {
            var spawnPosition = isSelectedQuery.ToComponentDataArray<Translation>(Allocator.TempJob);
            JobHandle handler = new JobHandle();
            UnitSpawnerSystem.UnitSpawnJob unitSpawnJob = new UnitSpawnerSystem.UnitSpawnJob
            {
                spawner = selectedSpawner[0],
                spawnPosition = spawnPosition[0].Value,
                unitsSpawn = unitsToSpawn
            };
            unitSpawnJob.Schedule(handler);
            handler.Complete();
            spawnPosition.Dispose();
        }

        selectedSpawner.Dispose();
    }
}
