using System;
using UnityEngine;
using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Unity.Entities;

public class UnitAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{

    public Unit unitScriptableObject;
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new UnitGroup
        {
        });
    }
}

[Serializable]
public struct MenuAttribute
{
    public float cost;
    public bool isLocked;
    public bool isSelected;
    public float unlockCost;
}




[Serializable]
public class UnitAttribute : INotifyPropertyChanged
{
    public static int MaxPoints = 4;
    public float baseValue;

    public float CurrentValue => _currentValue;

    public int addedPoints;
    public float perPoint
    {
        get => (capValue - baseValue) / MaxPoints;
    }
    public float capValue;
    public bool isAddedStatPercentage;

    public void AddPoint()
    {
        addedPoints++;
        var temp = baseValue + addedPoints * perPoint;
        _currentValue = temp > capValue ? capValue : temp;
        OnPropertyChanged();
    }

    private float _currentValue;

    public event PropertyChangedEventHandler PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}