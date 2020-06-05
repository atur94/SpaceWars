using Unity.Burst;
using Unity.Entities;
using UnityEngine;
using Unity.Transforms;

public class PrefabComponentSystem : MonoBehaviour, IConvertGameObjectToEntity
{
    public GameObject defaultPrefab;

    public static Entity defaultPrefabEntity;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        using (BlobAssetStore blobAssetStore = new BlobAssetStore())
        {
            Entity convertedEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(defaultPrefab,
                GameObjectConversionSettings.FromWorld(dstManager.World, blobAssetStore));
            
            PrefabComponentSystem.defaultPrefabEntity = convertedEntity;
        }
    }
}
