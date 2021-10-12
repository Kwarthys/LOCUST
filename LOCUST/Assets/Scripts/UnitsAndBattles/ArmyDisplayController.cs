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

    private List<UnitDisplayControler> unitDisplayers = new List<UnitDisplayControler>();

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

    public UnitDisplayControler getDisplayForUnit(UnitList unit)
    {
        foreach(UnitDisplayControler display in unitDisplayers)
        {
            if(display.toDisplay.unitName == unit)
            {
                return display;
            }
        }

        return null;
    }

    public void displayArmy()
    {
        if (toDisplay == null) return;

        armyScore = toDisplay.computeScore();

        armyNameTxt.text = armyName + " : " + armyScore.armySize;

        List<UnitList> troopsUnits = new List<UnitList>(toDisplay.troops.Keys);
        List<UnitDisplayControler> displayers = new List<UnitDisplayControler>(unitDisplayers);

        foreach(UnitList troop in troopsUnits)
        {
            UnitDisplayControler d = getDisplayForUnit(troop);

            if(d == null)
            {
                //Missing, create it
                d = Instantiate(unitDisplayControlerPrefab, unitDisplayHolder).GetComponent<UnitDisplayControler>();
                unitDisplayers.Add(d);

                d.displayUnit(Unit.getUnit(troop), toDisplay.troops[troop]);
            }
            else
            {
                displayers.Remove(d);
                d.refresh(toDisplay.troops[troop]);
            }
        }

        foreach(UnitDisplayControler d in displayers)
        {
            //These are not needed
            unitDisplayers.Remove(d);
            Destroy(d.gameObject);
        }

        float meanScore = (armyScore.getTotalVersusType(UnitType.Infantry) + armyScore.getTotalVersusType(UnitType.Heavy) + armyScore.getTotalVersusType(UnitType.Flying))/3;
        float vI = armyScore.getTotalVersusType(UnitType.Infantry) / meanScore;
        float vH = armyScore.getTotalVersusType(UnitType.Heavy) / meanScore;

        float iSize = armyScore.perTypePercent[UnitType.Infantry] * 3;
        float hSize = armyScore.perTypePercent[UnitType.Heavy] * 3;

        scoreDisplay.rebuildFor(vI, vH, iSize, hSize);
    }
}
