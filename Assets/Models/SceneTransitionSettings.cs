using System.Collections.Generic;
using UnityEngine;

public class SceneTransitionSettings : MonoBehaviour
{
    private SceneTransitionSettings()
    {

    }

    private static SceneTransitionSettings _instance;
    private Player _player;

    public static SceneTransitionSettings Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new SceneTransitionSettings();
            }

            return _instance;
        }
    }

    public Player Player
    {
        get
        {
            if (_player == null)
            {
                _player = ScriptableObject.CreateInstance<Player>();
                _player.pickedUnits = new List<Unit>();
                var availableUnits = Resources.LoadAll<Unit>("Prefabs/Ships");
                var unlockedUnits = new List<Unit>();
                foreach (var availableUnit in availableUnits)
                {
                    unlockedUnits.Add(Instantiate(availableUnit));
                }
                _player.pickedUnits.AddRange(availableUnits);

            }
            return _player;
        }
        set => _player = value;
    }
}