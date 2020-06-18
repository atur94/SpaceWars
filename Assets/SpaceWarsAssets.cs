using UnityEngine;
using Material = UnityEngine.Material;

public class SpaceWarsAssets : MonoBehaviour { 

    public Mesh selectedMesh;
    public Material selectedMaterial;

    public Color selectedColor;

    void Start()
    {
        selectedMaterial.color = selectedColor;
    }
    public static SpaceWarsAssets Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<SpaceWarsAssets>();
            }

            return _instance;
        }
    }

    private static SpaceWarsAssets _instance;
}