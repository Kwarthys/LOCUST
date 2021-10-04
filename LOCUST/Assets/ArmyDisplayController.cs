using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ArmyDisplayController : MonoBehaviour
{
    public Army toDisplay;

    public string armyName = "";

    public TextMeshProUGUI armyNameTxt;
    public TextMeshProUGUI armyVI;
    public TextMeshProUGUI armyVH;
    public TextMeshProUGUI armyVF;

    private ArmyScore armyScore;

    public GameObject unitDisplayControlerPrefab;
    public RectTransform unitDisplayHolder;

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

        setScore(armyVI, UnitType.Infantry);
        setScore(armyVH, UnitType.Heavy);
        setScore(armyVF, UnitType.Flying);

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
    }

    private void setScore(TextMeshProUGUI slot, UnitType scoreAgainst)
    {
        if (slot != null)
        {
            switch (scoreAgainst)
            {
                case UnitType.Infantry:
                    slot.text = "<b>vI : </b>";
                    break;

                case UnitType.Heavy:
                    slot.text = "<b>vH : </b>";
                    break;

                case UnitType.Flying:
                    slot.text = "<b>vF : </b>";
                    break;

                default:
                    slot.text = "error ";
                    break;
            }
            slot.text += armyScore.getTotalVersusType(scoreAgainst).ToString();
        }
    }
}
