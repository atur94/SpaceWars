
using System;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Physics.Systems;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    GameObject unitPrefab;

    public static List<Player> players;
    public PlanetManager planetManager;
    public Player aIAsset;

    [SerializeField] private Player activePlayer;
    // Start is called before the first frame update
    void Start()
    {
        var planetManager = FindObjectOfType<PlanetManager>();
        var startingPlanets = planetManager.planets.FindAll(planetAuthoring => planetAuthoring.canBeStartedPlanet);

        players = new List<Player>();
        players.Add(null);
        players.Add(null);
        var currentPlayer = SceneTransitionSettings.Instance.Player;
        activePlayer = currentPlayer;
        FindObjectOfType<PlayerManager>().me = currentPlayer;
        if (activePlayer == null) throw new Exception("Game needs to have exactly one active player");
        if (players.Count < 2) throw new Exception("Game needs at least 2 players");
        if(startingPlanets.Count < players.Count) throw new Exception("Game needs more starting planets");

        // Przypisanie planet 
        players[0] = activePlayer;

        for (var index = 1; index < players.Count; index++)
        {
            var player = players[index];
            if (player == null)
            {
                players[index] = aIAsset;
            }
        }

        for (int i = 0; i < players.Count; i++)
        {
            startingPlanets[i].owner = players[i];
        }

        World.DefaultGameObjectInjectionWorld.GetExistingSystem<TargetSelectionSystem>().mainPlayer = players[0];
//        World.DefaultGameObjectInjectionWorld.GetExistingSystem<BuildPhysicsWorld>().Enabled = false;
    }
}
