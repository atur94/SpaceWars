using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

[Serializable]
[CreateAssetMenu(fileName = "AI Config", menuName = "AI Assets")]
public class Player : ScriptableObject
{
    public static int idIncrement = 1;

    public int availableMoney;
    [SerializeField] public int id;

    [ReadOnly(true)] public bool isAi;
    [ReadOnly(true)] public bool isCurrentPlayer;
    [SerializeField]
    private string playerName;

    public List<Unit> unlockedUnits;
    public List<Unit> pickedUnits;

    private void Awake()
    {
        unlockedUnits = new List<Unit>();
        id = idIncrement++;
    }
}