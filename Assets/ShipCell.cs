using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShipCell : MonoBehaviour
{
    public Toggle toggle;
    public Image contentImage;
    public TextMeshProUGUI unitCost;
    
    public Unit relatedUnit { get; set; }
    public TextMeshProUGUI unitName;
}
