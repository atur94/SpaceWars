using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using Unity.Entities.Conversion;

[CreateAssetMenu(fileName = "Unit", menuName = "Unit")]
public class Unit : ScriptableObject
{
    public int key;

    public int cost;
    public bool isLocked;

    [HideInInspector]
    public bool isSelected;
        
    public string unitName;

    public GameObject prefab;


    public Sprite sprite;
    public UnitAttribute health;
    public UnitAttribute attackDamage;
    public UnitAttribute attackCooldown;
    public UnitAttribute movementSpeed;
    public UnitAttribute energy;
    public UnitAttribute energyRegeneration;
    public UnitAttribute energyPerAttack;


}

