using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UnitDisplayControler : MonoBehaviour
{
    public Unit toDisplay;

    public TextMeshProUGUI unitName;
    public TextMeshProUGUI unitVI;
    public TextMeshProUGUI unitVH;
    public TextMeshProUGUI unitVF;

    public GameObject scoreDisplayPrefab;
    public RectTransform scoreDisplayHolder;

    public float number = -1;

    public void displayUnit()
    {
        if (toDisplay == null) return;

        if(unitName != null)
        {
            if(number != -1)
            {
                unitName.text = number + " ";
            }
            unitName.text += toDisplay.name + (number>1?"s : " + (number * toDisplay.scoreValue) : "") + " (" + toDisplay.scoreValue + ")";
        }

        setScore(unitVI, UnitType.Infantry);
        setScore(unitVH, UnitType.Heavy);
        setScore(unitVF, UnitType.Flying);

        ScriteDisplayControler scoreDisplay = Instantiate(scoreDisplayPrefab, scoreDisplayHolder).GetComponent<ScriteDisplayControler>();

        scoreDisplay.rebuildFor(toDisplay.vInfantryScore, toDisplay.vHeavyScore);
    }

    private void setScore(TextMeshProUGUI slot, UnitType scoreAgainst)
    {
        if (slot != null)
        {
            switch(scoreAgainst)
            {
                case UnitType.Infantry:
                    slot.text = "vI : ";
                    break;

                case UnitType.Heavy:
                    slot.text = "vH : ";
                    break;

                case UnitType.Flying:
                    slot.text = "vF : ";
                    break;

                default:
                    slot.text = "error ";
                    break;
            }

            if(number > 1)
            {
                slot.text += (toDisplay.getScoreAgainst(scoreAgainst) * number * toDisplay.scoreValue) + " (" + toDisplay.getScoreAgainst(scoreAgainst) + ")";
            }
            else
            {
                slot.text += toDisplay.getScoreAgainst(scoreAgainst).ToString();
            }
        }
    }
}
