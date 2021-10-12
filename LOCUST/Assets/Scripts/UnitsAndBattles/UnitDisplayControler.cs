using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UnitDisplayControler : MonoBehaviour
{
    public Unit toDisplay { get; private set; }

    public TextMeshProUGUI unitName;

    public GameObject scoreDisplayPrefab;
    public RectTransform scoreDisplayHolder;

    private int number = -1;

    public void displayUnit(Unit toDisplay, int number = -1)
    {
        this.toDisplay = toDisplay;

        refresh(number);

        ScriteDisplayControler scoreDisplay = Instantiate(scoreDisplayPrefab, scoreDisplayHolder).GetComponent<ScriteDisplayControler>();

        scoreDisplay.rebuildFor(toDisplay.vInfantryScore, toDisplay.vHeavyScore);
    }

    public void refresh(int number = -1)
    {
        if (unitName != null)
        {
            if (number != -1)
            {
                unitName.text = number + " ";
            }
            unitName.text += toDisplay.unitName + (number > 1 ? "s\n" + (number * toDisplay.scoreValue) : "") + " (" + toDisplay.scoreValue + ")";
        }
    }
}
