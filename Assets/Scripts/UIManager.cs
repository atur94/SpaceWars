using System;
using UnityEngine;
using System.Collections;
using TMPro;
using Unity.Collections;
using Unity.Entities;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    public TextMeshProUGUI unitsNumber;
    private EntityManager em;

    public InputField inputField;

    public int units
    {
        get { return _units; }
        set
        {
            if (_units != value)
            {
                _units = value;
                UpdateText(_units.ToString());
            }
        }
    }

    private UIUpdaterSystem uiUpdaterSystem;
    private int _units;

    // Use this for initialization
    private EntityQuery query;
    EntityCommandBufferSystem test;

    void Start()
    {
        em = World.DefaultGameObjectInjectionWorld.EntityManager;
        uiUpdaterSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystem<UIUpdaterSystem>();
        test = em.World.GetExistingSystem<EntityCommandBufferSystem>();
    }




    public void SpawnUnits()
    {
        try
        {
            var unitsToSpawn = int.Parse(inputField.text);
            uiUpdaterSystem.SpawnUnits(unitsToSpawn);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public void UpdateText(string text)
    {
        unitsNumber.SetText(text);
    }
}
