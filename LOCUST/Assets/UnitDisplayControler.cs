using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UnitDisplayControler : MonoBehaviour
{
    public Unit toDisplay;

    public TextMeshProUGUI unitName;

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
            unitName.text += toDisplay.name + (number>1?"s\n" + (number * toDisplay.scoreValue) : "") + " (" + toDisplay.scoreValue + ")";
        }

        ScriteDisplayControler scoreDisplay = Instantiate(scoreDisplayPrefab, scoreDisplayHolder).GetComponent<ScriteDisplayControler>();

        scoreDisplay.rebuildFor(toDisplay.vInfantryScore, toDisplay.vHeavyScore);
    }
}
