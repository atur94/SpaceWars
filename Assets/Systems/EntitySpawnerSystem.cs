

using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public class EntitySpawnerSystem : ComponentSystem
{

    protected override void OnCreate()
    {
        base.OnCreate();
    }

    private const int UNITS_SPAWN = 1;
    protected override void OnUpdate()
    {

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D rayHit = Physics2D.Raycast(ray.origin, ray.direction);
            if (rayHit.collider != null)
            {
                Entity target = Entity.Null;

                Entities.WithAll<TargetTag>().ForEach((entity) =>
                {
                    if(target == Entity.Null)
                        target = entity;
                });
                for (int i = 0; i < UNITS_SPAWN; i++)
                {
                    var spawnedUnit = EntityManager.Instantiate(PrefabComponentSystem.defaultPrefabEntity);

                    EntityManager.SetName(spawnedUnit, "Unit");
                    EntityManager.SetComponentData(spawnedUnit, new Translation { Value = new float3(rayHit.point + Vector2.right * i * 2, 0f) });

                    EntityManager.SetComponentData(spawnedUnit, new TargetSelector
                    {
                        Primary = target
                    });
                }

                    // Do something with the object that was hit by the raycast.
            }


        }


        //Entities.ForEach((ref PrefabComponentSystem prefabComponenentSystem) =>
        //{
        //    EntityManager.Instantiate(PrefabComponentSystem.defaultPrefabEntity);
        //});
    }
}