using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerResources : MonoBehaviour
{
    public TextMeshProUGUI resourceDisplayText;

    private Dictionary<GameResources, int> resources = new Dictionary<GameResources, int>();

    private void Start()
    {
        resources[GameResources.BioMass] = 0;
        resources[GameResources.Metals] = 0;

        addResource(500000, 300000);
    }

    public void addResource(GameResources r, int amount)
    {
        resources[r] += amount;
        refreshDisplay();
    }

    public void addResource(GameCost c)
    {
        resources[GameResources.BioMass] += c.bioMassCost;
        resources[GameResources.Metals] += c.metalCost;
        refreshDisplay();
    }

    public void addResource(int bioMassNumber, int metalsNumber)
    {
        resources[GameResources.BioMass] += bioMassNumber;
        resources[GameResources.Metals] += metalsNumber;
        refreshDisplay();
    }

    private void refreshDisplay()
    {
        string display = "";

        foreach (GameResources r in resources.Keys)
        {
            display += r + ": " + resources[r] + "  ";
        }

        resourceDisplayText.text = display;
    }


    public bool tryBuy(GameCost cost)
    {
        bool isOK = hasEnough(cost);

        if(isOK)
        {
            resources[GameResources.BioMass] -= cost.bioMassCost;
            resources[GameResources.Metals] -= cost.metalCost;
            refreshDisplay();
        }

        return isOK;
    }


    public bool hasEnough(GameCost cost)
    {
        bool isOK = true;

        foreach(GameResources r in resources.Keys)
        {
            if(resources[r] < cost.getCost(r))
            {
                isOK = false;
                Debug.Log("Not enough " + r);
            }
        }

        return isOK;
    }
}
