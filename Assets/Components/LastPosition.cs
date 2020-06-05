using System;
using UnityEngine;
using System.Collections;
using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct LastPosition : IComponentData
{
    public float2 xy;
}
