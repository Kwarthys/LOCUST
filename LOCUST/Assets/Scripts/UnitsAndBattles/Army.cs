using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmyScore
{
    public float armySize = 0;
    public float VI = 0;
    public float VH = 0;
    public float VF = 0;
}

public class Army
{
    public Dictionary<UnitList, int> troops = new Dictionary<UnitList, int>();

    public void addTroops(UnitList unit, int number)
    {
        if(troops.ContainsKey(unit))
        {
            troops[unit] += number;
        }
        else
        {
            troops[unit] = number;
        }
    }

    public void removeTroops(UnitList unit, int number)
    {
        if (!troops.ContainsKey(unit))
        {
            return;
        }

        troops[unit] -= number;

        if(troops[unit] < 0)
        {
            troops[unit] = 0;
        }
    }

    public ArmyScore computeScore()
    {
        ArmyScore s = new ArmyScore();

        float score, vi, vf, vh;

        foreach(KeyValuePair<UnitList, int> entry in troops)
        {
            Unit u = Unit.getUnit(entry.Key);
            score = u.scoreValue * entry.Value;
            vi = u.getScoreAgainst(UnitType.Infantry) * score;
            vh = u.getScoreAgainst(UnitType.Heavy) * score;
            vf = u.getScoreAgainst(UnitType.Flying) * score;

            s.armySize += score;
            s.VI += vi;
            s.VH += vh;
            s.VF += vf;

            Debug.Log(u.name + " S:" + score + ", vI:" + vi + " vH:" + vh + " vF:" + vf);
        }

        return s;
    }
}
