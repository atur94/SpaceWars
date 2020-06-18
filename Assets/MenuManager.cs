using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;
using Toggle = UnityEngine.UI.Toggle;

public class MenuManager : MonoBehaviour
{
    public const int maxSelectedUnits = 5;
    public TextMeshProUGUI currentlySelectedText;
    public TextMeshProUGUI moneyText;
    public Button startButton;
    public GameObject itemCellPrefab;
    public ScrollRect scrollRect;
    public GameObject content;
    public ScrollView conten2t;
    public List<Unit> availableUnits;
    private List<Unit> unitList;
    public UnlockWindow unlockWindow;


    private List<ShipCell> generatedCells;


    public Player me;

    private void Start()
    {
        me = FindObjectOfType<PlayerManager>().me;
        UnlockWindowInitialization();
        generatedCells = new List<ShipCell>();
        var buttonEvent = new Button.ButtonClickedEvent();
        buttonEvent.AddListener(StartGame);
        startButton.onClick = buttonEvent;
        unitList = new List<Unit>();

        
        foreach (var availableUnit in availableUnits)
        {
            unitList.Add(Instantiate(availableUnit));
        }
        foreach (var unit in unitList)
        {
            var cell = Instantiate(itemCellPrefab, content.transform);
            var shipCell = cell.GetComponent<ShipCell>();
            generatedCells.Add(shipCell);
            shipCell.relatedUnit = unit;
            shipCell.contentImage.sprite = unit.sprite;
            shipCell.unitName.SetText(unit.unitName);

            if (unit.isLocked)
            {
                shipCell.unitCost.SetText(unit.cost.ToString());
            }
            else
            {
                shipCell.unitCost.enabled = false;
            }

            Toggle.ToggleEvent ev = new Toggle.ToggleEvent();
            ev.AddListener(OnClickHandler);
            shipCell.toggle.onValueChanged = ev;
        }
        LockAllUnits();
        RefreshStates();
        moneyText.SetText($"gold: {me.availableMoney}");
        OnClickHandler(false);
    }

    private void StartGame()
    {
        if (canBeStarted)
        {
            SceneTransitionSettings.Instance.Player = me;
            SceneManager.LoadScene(1, LoadSceneMode.Single);
        }
    }

    private void OnClickHandler(bool arg0)
    {
        var currentlySelected = 0;

        foreach (var generatedCell in generatedCells)
        {
            if (generatedCell.toggle.isOn)
            {
                if (generatedCell.relatedUnit.isLocked)
                {
                    // Odblokuj jednostke
                    if (generatedCell.relatedUnit.cost > me.availableMoney)
                    {
                        moneyText.faceColor = Color.red;
                        StartCoroutine("CantBuy");
                        Debug.Log("Too expensive to buy");
                    }
                    else
                    {
                        UnlockWindowShowup(generatedCell);
                    }
                    generatedCell.toggle.isOn = false;
                }
                else
                {
                    currentlySelected++;

                }
            }
        }

        currentlySelectedText.SetText($"Currently selected {currentlySelected}/{maxSelectedUnits}");
        ValidateAmountOfSelected(currentlySelected);
    }

    public bool canBeStarted;

    private void RefreshStates()
    {
        foreach (var cell in generatedCells)
        {
            cell.unitCost.gameObject.SetActive(cell.relatedUnit.isLocked);
        }
        moneyText.SetText(me.availableMoney.ToString());
    }
    private void ValidateAmountOfSelected(int currentUnits)
    {
        if (currentUnits > maxSelectedUnits)
        {
            currentlySelectedText.color = Color.red;
        }
        else
        {
            currentlySelectedText.color = Color.white;
        }

        if (currentUnits == maxSelectedUnits) canBeStarted = true;
        else canBeStarted = false;

        startButton.interactable = canBeStarted;

    }

    private void UnlockWindowInitialization()
    {
        unlockWindow.gameObject.SetActive(false);
        var cancelEv = new Button.ButtonClickedEvent();
        cancelEv.AddListener(UnlockCancel);
        var unlockEv = new Button.ButtonClickedEvent();
        unlockEv.AddListener(UnlockConfirm);
        unlockWindow.cancel.onClick = cancelEv;
        unlockWindow.unlock.onClick = unlockEv;
    }

    private void LockAllUnits()
    {
        foreach (var availableUnit in availableUnits)
        {
            availableUnit.isLocked = true;
        }
        RefreshStates();
    }

    private ShipCell unitToUnlock;
    private void UnlockWindowShowup(ShipCell genCell)
    {
        unlockWindow.gameObject.SetActive(true);
        unitToUnlock = genCell;
        unlockWindow.unitName.SetText(genCell.relatedUnit.unitName);
        unlockWindow.unitDescription.SetText("Blablabla12456");
        unlockWindow.unitImage.sprite = genCell.relatedUnit.sprite;
    }

    private void UnlockConfirm()
    {
        unitToUnlock.relatedUnit.isLocked = false;
        me.availableMoney -= unitToUnlock.relatedUnit.cost;
        unitToUnlock = null;
        unlockWindow.gameObject.SetActive(false);
        OnClickHandler(false);
        RefreshStates();
    }

    private void UnlockCancel()
    {
        unitToUnlock = null;
        unlockWindow.gameObject.SetActive(false);
    }



    private IEnumerator CantBuy()
    {
        var deltaTime = Time.deltaTime;
        while (moneyText.faceColor != Color.black)
        {
            byte x = 3;
            var str = moneyText.faceColor;
            str.r -= x;
            moneyText.faceColor = str;
            yield return new WaitForSeconds(deltaTime * 2);
        }

    }
}
