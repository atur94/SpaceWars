using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public Player me;

    public void Awake()
    {
        // Czytaj z pamięci dane gracza
        // Na chwilę obecną utwórz pustego z konstruktora

        me = ScriptableObject.CreateInstance<Player>();
        me.isCurrentPlayer = true;
        me.id = GUID.Generate();
        me.unlockedUnits = new List<Unit>();
        me.availableMoney = 1000;
    }
}