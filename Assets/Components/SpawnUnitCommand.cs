using UnityEngine;
using System.Collections;
using Unity.Entities;

public struct SpawnUnitCommand : IComponentData
{
    public float percent;
    public Entity target;
    public Entity source;
}
