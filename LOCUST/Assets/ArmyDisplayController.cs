using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ArmyDisplayController : MonoBehaviour
{
    public Army toDisplay;

    public string armyName = "";

    public TextMeshProUGUI armyNameTxt;

    private ArmyScore armyScore;

    public GameObject unitDisplayControlerPrefab;
    public RectTransform unitDisplayHolder;

    public GameObject scoreDisplayPrefab;
    public RectTransform scoreDisplayHolder;

    private ScriteDisplayControler scoreDisplay;

    public void Start()
    {
        scoreDisplay = Instantiate(scoreDisplayPrefab, scoreDisplayHolder).GetComponent<ScriteDisplayControler>();
    }

    public void displayArmy(Army toDisplay, string armyName)
    {
        this.armyName = armyName;
        this.toDisplay = toDisplay;
        displayArmy();
    }

    public void displayArmy()
    {
        if (toDisplay == null) return;

        armyScore = toDisplay.computeScore();

        armyNameTxt.text = armyName + " : " + armyScore.armySize;

        foreach (Transform child in unitDisplayHolder)
        {
            Destroy(child.gameObject); //F
        }

        foreach (UnitList unit in toDisplay.troops.Keys)
        {
            UnitDisplayControler unitDisplay = Instantiate(unitDisplayControlerPrefab, unitDisplayHolder).GetComponent<UnitDisplayControler>();

            unitDisplay.toDisplay = Unit.getUnit(unit);
            unitDisplay.number = toDisplay.troops[unit];
            unitDisplay.displayUnit();
        }

        float meanScore = (armyScore.getTotalVersusType(UnitType.Infantry) + armyScore.getTotalVersusType(UnitType.Heavy) + armyScore.getTotalVersusType(UnitType.Flying))/3;
        float vI = armyScore.getTotalVersusType(UnitType.Infantry) / meanScore;
        float vH = armyScore.getTotalVersusType(UnitType.Heavy) / meanScore;

        float iSize = armyScore.perTypePercent[UnitType.Infantry] * 3;
        float hSize = armyScore.perTypePercent[UnitType.Heavy] * 3;

        scoreDisplay.rebuildFor(vI, vH, iSize, hSize);
    }
}
