using System;
using UnityEngine;
using System.Collections;
using Unity.Entities;
using Unity.Mathematics;

public struct LastPosition : IComponentData
{
    public float2 xy;
}
