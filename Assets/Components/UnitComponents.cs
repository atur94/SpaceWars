using Unity.Entities;
using UnityEngine;
using UnityEditor;

public struct Health : IComponentData
{
    public float CurrentValue;
    public float MaxValue;
}

public struct Armor : IComponentData
{
    public float Value;
}

public struct Energy : IComponentData
{
    public float CurrentValue;
    public float MaxValue;
    public float RegenerationPerSecond;
}

public struct Attack : IComponentData
{
    public float Value;
    public float EnergyPerShot;
    public float Cooldown;
}