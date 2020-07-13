using Unity.Burst;
using Unity.Entities;
using Unity.Physics.Systems;
using UnityEngine;
using Unity.Transforms;
using BoxCollider = Unity.Physics.BoxCollider;
using Collider = Unity.Physics.Collider;

[UpdateAfter(typeof(ExportPhysicsWorld))]
public class PrefabComponentSystem : MonoBehaviour, IConvertGameObjectToEntity
{
    public GameObject defaultPrefab;
    public static GameObject defaultPrefabStatic;
    public static BlobAssetStore blobAssetStore;
    public static Entity defaultPrefabEntity;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        defaultPrefabStatic = defaultPrefab;
        using (blobAssetStore = new BlobAssetStore())
        {
            Entity convertedEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(defaultPrefab,
                GameObjectConversionSettings.FromWorld(dstManager.World, blobAssetStore));

            PrefabComponentSystem.defaultPrefabEntity = convertedEntity;
        }
    }
}
