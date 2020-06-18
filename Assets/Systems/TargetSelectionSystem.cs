using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;
using Object = UnityEngine.Object;
using Ray = UnityEngine.Ray;
using RaycastHit = UnityEngine.RaycastHit;

[AlwaysUpdateSystem]
public class TargetSelectionSystem : ComponentSystem
{
    BeginInitializationEntityCommandBufferSystem m_EntityCommandBufferSystem;
    protected override void OnCreate()
    {
        base.OnCreate();
        m_EntityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();

    }

    protected override void OnUpdate()
    {

//        if (Input.GetKeyUp(KeyCode.Mouse0))
//        {
//            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
//
//            float rayDistance = 100f;
//            var entitySelected = Raycast(ray.origin, (ray.direction * rayDistance) + ray.origin);
//            Entities.ForEach((Entity entity, ref SelectedTag tag) => { EntityManager.RemoveComponent<SelectedTag>(entity); });
//
//            if (entitySelected != Entity.Null)
//            {
//                try
//                {
//                    EntityManager.GetComponentData<Spawner>(entitySelected);
//                    EntityManager.AddComponent<SelectedTag>(entitySelected);
//                }
//                catch (Exception e)
//                {
//                    Debug.Log("Nieprawidlowy obiekt");
//                }
//            }
//        }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
//
//            // sprawdzić czy kliknięto w obiekt typu spawner. Jezeli tak i jest on zaznaczony to zaatakuj
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
//
//            float rayDistance = 100f;
//            var entitySelected = Raycast(ray.origin, (ray.direction * rayDistance) + ray.origin);
//            bool isAnySelected = false;
//
//            // Sprawdzić czy jest coś zaznaczone. Jezeli tak to wyslac jednostki na ta zaznaczoną jednostkę
//            Entity parentEntity = Entity.Null;
//            Entities.WithAll<SelectedTag>().ForEach(entity =>
//            {
//                isAnySelected = true;
//                parentEntity = entity;
//            });
//
//            if (isAnySelected && entitySelected != Entity.Null)
//            {
//                try
//                {
//                    EntityManager.GetComponentData<Spawner>(entitySelected);
//                    var parentPosition = EntityManager.GetComponentData<Translation>(parentEntity);
//                    var targetPosition = EntityManager.GetComponentData<Translation>(entitySelected);
//
//                    if (parentEntity != entitySelected)
//                    {
//                        SpawnUnits(parentPosition.Value, entitySelected, entitySelected, parentEntity);
//                        Debug.Log("Zaatakuj ten spawner");
//                    }
//
//                }
//                catch (ArgumentException e)
//                {
//                    Debug.Log("Nieprawidlowy target");
//                }
//            }


            RaycastHit2D rayHit = Physics2D.Raycast(ray.origin, ray.direction);
            if (rayHit.collider != null)
            {
                Entities.ForEach((ref TargetSelector targetSelector) =>
                {
                    targetSelector.SecondaryTranslation = new float3(rayHit.point, 0f);
                }); 
            }
        }
    }

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

        if (collisionWorld.CastRay(raycastInput, out var raycastHit))
        {
            var entityHit = buildPhysicsWorld.PhysicsWorld.Bodies[raycastHit.RigidBodyIndex].Entity;
            return entityHit;
        }

        return Entity.Null;
    }

    private void SpawnUnits(float3 spawnPosition, Entity target, Entity secondary, Entity parent)
    {
        var commandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer();
        var targetPosition = EntityManager.GetComponentData<Translation>(target).Value;
        var secondaryPosition = EntityManager.GetComponentData<Translation>(secondary).Value;
        for (int i = 0; i < 10; i++)
        {
            var entity = commandBuffer.Instantiate(SpaceWarsEntities.defaultUnitEntity);
            commandBuffer.SetComponent(entity, new Translation
            {
                Value = spawnPosition,
            });
            commandBuffer.SetComponent(entity, new TargetSelector
            {
                Primary = target,
                PrimaryTranslation = targetPosition,
                SecondaryTranslation = targetPosition,
                Secondary = target,
                parent = parent
            });
        }
    }
}

public class TargetSelectedRenderer : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.WithAll<SelectedTag>().ForEach((ref Translation translation, ref PlanetScale planetScale) =>
        {

            var matrix = Matrix4x4.TRS(translation.Value, Quaternion.identity, planetScale.scale);
            Graphics.DrawMesh(SpaceWarsAssets.Instance.selectedMesh, matrix, SpaceWarsAssets.Instance.selectedMaterial, 5);
        });
    }
}