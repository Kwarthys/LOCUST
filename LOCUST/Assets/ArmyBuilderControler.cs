using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class ArmyBuilderControler : MonoBehaviour
{
    public TMP_Dropdown unitListSelector;
    public TMP_InputField numberInput;
    public TextMeshProUGUI costsPoints;

    public BattleManager battleManager;

    // Start is called before the first frame update
    void Start()
    {
        UnitList[] unitList = (UnitList[])Enum.GetValues(typeof(UnitList));
        foreach(UnitList u in unitList)
        {
            unitListSelector.options.Add(new TMP_Dropdown.OptionData() { text = u.ToString() });
        }

        unitListSelector.onValueChanged.AddListener(delegate { selectedOnChange(); });
        numberInput.onValueChanged.AddListener(delegate { selectedOnChange(); });
    }

    public void selectedOnChange()
    {
        Unit newSelected = Unit.getUnit((UnitList)unitListSelector.value);
        string newCosts = getInputInt() * newSelected.resourceCosts[GameResources.BioMass] + " " + GameResources.BioMass;
        newCosts += " " + getInputInt() * newSelected.resourceCosts[GameResources.Metals] + " " + GameResources.Metals;
        newCosts += " : " + getInputInt() * newSelected.scoreValue + " pts.";

        costsPoints.text = newCosts;
    }

    public void recruitTroops()
    {
        Debug.Log(getInputInt() + " " + Unit.getUnit((UnitList)unitListSelector.value).name);
        battleManager.recruitForPlayer((UnitList)unitListSelector.value, getInputInt());
    }

    private int getInputInt()
    {
        if(int.TryParse(numberInput.text, out int value))
        {
            return value;
        }
        return 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
