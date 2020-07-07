using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using TMPro;
using Unity.Entities;
using Unity.Transforms;

public class SinglePlanetController : MonoBehaviour
{
    private TextMeshPro unitsTextMesh;
    [Range(min: 0.3f, max: 0.9f)]
    public float spawnPercentage = 0.5f;
    public Entity spawnerEntity;
    // Use this for initialization
    public Player owner
    {
        get => _owner;
        set
        {
            _owner = value;
            entityManager.SetComponentData(spawnerEntity, new PlanetOwner
            {
                owner = _owner.id
            });
        }
    }

    public ObservableCollection<Unit> actualUnits;
    public bool canBeStartedPlanet;
    [SerializeField]
    private bool isAnyEnemyInRange = false;



    public int unitSpawn = 1;
    private EntityCommandBuffer CommandBuffer
    {
        get => commandBufferSystem.CreateCommandBuffer();
    }
    public float cooldown = 5f;
    private Player _owner;
    private EntityCommandBufferSystem commandBufferSystem;
    private PlayerManager playerManager;
    private EntityManager entityManager;
    void Start()
    {
        unitsTextMesh = GetComponentInChildren<TextMeshPro>();
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        FindObjectOfType<PlanetManager>().planets.Add(this);
        commandBufferSystem = World.DefaultGameObjectInjectionWorld
            .GetOrCreateSystem<EntityCommandBufferSystem>();
        playerManager = FindObjectOfType<PlayerManager>();
        actualUnits = new ObservableCollection<Unit>();
        actualUnits.CollectionChanged += UpdateUnitNumber;
    }

    private void UpdateUnitNumber(object sender, NotifyCollectionChangedEventArgs e)
    {
        unitsTextMesh.SetText(actualUnits.Count.ToString());
    }

    private float nextSpawn;
    public void FixedUpdate()
    {
        float currentTime = Time.time;
        if (nextSpawn < currentTime && !isAnyEnemyInRange)
        {
            var unitBuffer = entityManager.GetBuffer<PlanetUnitsBuffer>(spawnerEntity);

            nextSpawn = currentTime + 2f;
            if (owner != null && actualUnits.Count < 50)
            {

                foreach (var pickedUnit in owner.pickedUnits)
                {
                    actualUnits.Add(Instantiate(pickedUnit));
                }
            }
        }
    }

    public void SpawnUnits(SinglePlanetController planetTarget)
    {
        List<Unit> unitsToCleanup = new List<Unit>();
        var lookupUnits = actualUnits.ToLookup(unit => unit.key, unit => unit);
        var lookupUnitsKeys = lookupUnits.Select(grouping => grouping.Key);

        foreach (var groupKey in lookupUnitsKeys)
        {
            var unitsGrouped = lookupUnits[groupKey];
            var unitsInGroup = unitsGrouped.Count();

            int unitsToSpawn =  Mathf.FloorToInt(unitsInGroup * spawnPercentage);

            for (int i = 0; i < unitsToSpawn; i++)
            {
                var futureUnit = unitsGrouped.ElementAt(i);
                unitsToCleanup.Add(futureUnit);

                var spawnedEntity = entityManager.Instantiate(SpaceWarsEntities.shipEntities[futureUnit.key - 1]);
                entityManager.SetComponentData(spawnedEntity, new Translation
                {
                    Value = transform.position,
                });

                entityManager.SetComponentData(spawnedEntity, new UnitOwner
                {
                    owner = owner.id
                });

                entityManager.SetComponentData(spawnedEntity, new TargetSelector
                {
                    PrimaryTranslation = planetTarget.transform.position,
                    Primary = planetTarget.spawnerEntity,
                    SecondaryTranslation = planetTarget.transform.position,
                });
            }

        }

        foreach (var unitToClean in unitsToCleanup)
        {
            actualUnits.Remove(unitToClean);
        }

    }
}
