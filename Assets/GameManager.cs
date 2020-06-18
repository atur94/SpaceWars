
using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    GameObject unitPrefab;

    public List<Player> players;
    public Player aIAsset;

    [SerializeField] private Player activePlayer;
    // Start is called before the first frame update
    void Start()
    {
        var currentPlayer = SceneTransitionSettings.Instance.Player;
        activePlayer = currentPlayer;
        FindObjectOfType<PlayerManager>().me = currentPlayer;
        if (activePlayer == null) throw new Exception("Game needs to have exactly one active player");
        if (players.Count < 2) throw new Exception("Game needs at least 2 players");
        players[0] = activePlayer;

        for (var index = 1; index < players.Count; index++)
        {
            var player = players[index];
            if (player == null)
            {
                players[index] = aIAsset;
            }
        }
    }
}
