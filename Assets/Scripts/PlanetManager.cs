using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using JetBrains.Annotations;
using Unity.Entities;
using Unity.Transforms;

public class PlanetManager : MonoBehaviour, INotifyPropertyChanged
{
    public UnitSwallowingSystem unitSwallowingSystem;
    public List<SinglePlanetController> planets;
    public PlayerManager playerManager;
    public SinglePlanetController CurrentlySelectedPlanet
    {
        get => _currentlySelectedPlanet;
        set
        {
            if (value != null && _currentlySelectedPlanet != value && value.owner == playerManager.me)
            {
                _currentlySelectedPlanet = value;
                OnPropertyChanged();
                return;
            }

            if(value == null)
                _currentlySelectedPlanet = value;
        }
    }

    public EntityManager entityManager;
    private EntityCommandBuffer CommandBuffer
    {
        get => commandBufferSystem.CreateCommandBuffer();
    }
    private EntityCommandBufferSystem commandBufferSystem;
    private SinglePlanetController _currentlySelectedPlanet;

    // Use this for initialization
    void Awake()
    {
        playerManager = FindObjectOfType<PlayerManager>();
        planets = new List<SinglePlanetController>();
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        commandBufferSystem = World.DefaultGameObjectInjectionWorld
            .GetOrCreateSystem<EntityCommandBufferSystem>();
    }

    void Start()
    {
        unitSwallowingSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystem<UnitSwallowingSystem>();
        unitSwallowingSystem.onUnitEntered += UnitEntered;
    }

    private void UnitEntered(Entity unit, Entity spawner)
    {
        foreach (var singlePlanetController in planets)
        {
            if (spawner == singlePlanetController.spawnerEntity)
            {
                var unitGroup = entityManager.GetComponentData<UnitGroup>(unit);
                foreach (var unitPrefab in SpaceWarsEntities.availableUnits)
                {
                    if (singlePlanetController.owner == null && singlePlanetController.actualUnits.Count == 0)
                    {
                        var unitOwner = entityManager.GetComponentData<UnitOwner>(unit);
                        var owner = GameManager.players.Find(player => player.id == unitOwner.owner);
                        if(owner != null)
                            singlePlanetController.owner = owner;
                    }
                    if (unitPrefab.key == unitGroup.Value)
                    {
                        singlePlanetController.actualUnits.Add(Instantiate(unitPrefab));
                        break;
                    }
                }
            }
        }
    }

    

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
            var collider = hit.collider;
            if (collider != null)
            {
                if (collider.gameObject.tag.Equals("Background"))
                {
                    CurrentlySelectedPlanet = null;
                    foreach (var spawnerAuthoring in planets)
                    {
                        if (entityManager.HasComponent<SelectedTag>(spawnerAuthoring.spawnerEntity))
                        {
                            entityManager.RemoveComponent<SelectedTag>(spawnerAuthoring.spawnerEntity);
                        }
                    }
                }else if (collider.gameObject.GetComponent<SinglePlanetController>() is SinglePlanetController spawnerTemp && spawnerTemp.owner == playerManager.me)
                {
                    CurrentlySelectedPlanet = spawnerTemp;
                    foreach (var spawnerAuthoring in planets)
                    {
                        if (entityManager.HasComponent<SelectedTag>(spawnerAuthoring.spawnerEntity))
                        {
                            entityManager.RemoveComponent<SelectedTag>(spawnerAuthoring.spawnerEntity);
                        }
                    }
                    entityManager.AddComponent<SelectedTag>(spawnerTemp
                        .spawnerEntity);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            if (CurrentlySelectedPlanet != null)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
                var collider = hit.collider;
                if (collider.GetComponent<SinglePlanetController>() is SinglePlanetController attackedSpawner && attackedSpawner != CurrentlySelectedPlanet)
                {
                    Debug.Log("Zaaakuj en spawner");

                    CurrentlySelectedPlanet.SpawnUnits(attackedSpawner);
                    attackedSpawner.SpawnUnits(CurrentlySelectedPlanet);
                }
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
