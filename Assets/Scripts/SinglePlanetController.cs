using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;

public class SinglePlanetController : MonoBehaviour
{
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

    public List<Unit> actualUnits;
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

        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        FindObjectOfType<PlanetManager>().planets.Add(this);
        commandBufferSystem = World.DefaultGameObjectInjectionWorld
            .GetOrCreateSystem<EntityCommandBufferSystem>();
        playerManager = FindObjectOfType<PlayerManager>();
        actualUnits = new List<Unit>();
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
}
