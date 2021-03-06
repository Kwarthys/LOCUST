//using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmyScore
{
    public float armySize = 0;

    private float[,] scoresAttackMatrix = new float[3,3];

    public Dictionary<UnitType, float> perTypeSizeScore = new Dictionary<UnitType, float>();
    public Dictionary<UnitType, float> perTypePercent = new Dictionary<UnitType, float>();

    public ArmyScore()
    {
        //Init all dictionaries
        perTypeSizeScore[UnitType.Infantry] = 0;
        perTypeSizeScore[UnitType.Heavy] = 0;
        perTypeSizeScore[UnitType.Flying] = 0;

        foreach (KeyValuePair<UnitType, float> entry in perTypeSizeScore)
        {
            perTypePercent[entry.Key] = 0;
        }
    }

    public float getMatrix(UnitType typeFrom, UnitType typeTo)
    {
        return scoresAttackMatrix[(int)typeFrom, (int)typeTo];
    }

    public void addTypeAttackScore(UnitType typeFrom, UnitType typeTo, float score)
    {
        scoresAttackMatrix[(int)typeFrom, (int)typeTo] += score;
    }

    public float getTotalVersusType(UnitType versus)
    {
        return scoresAttackMatrix[(int)UnitType.Infantry, (int)versus] + scoresAttackMatrix[(int)UnitType.Heavy, (int)versus] + scoresAttackMatrix[(int)UnitType.Flying, (int)versus];
    }

    public float getTotalByType(UnitType from)
    {
        return scoresAttackMatrix[(int)from, (int)UnitType.Infantry] + scoresAttackMatrix[(int)from, (int)UnitType.Heavy] + scoresAttackMatrix[(int)from, (int)UnitType.Flying];
    }

    public void addTypeScore(UnitType type, float score)
    {
        if (!perTypeSizeScore.ContainsKey(type))
        {
            perTypeSizeScore[type] = 0;
        }

        perTypeSizeScore[type] += score;

        computeForces();
    }

    public void computeForces()
    {
        float sum = 0;

        foreach (KeyValuePair<UnitType, float> entry in perTypeSizeScore)
        {
            sum += entry.Value;
        }

        foreach (KeyValuePair<UnitType, float> entry in perTypeSizeScore)
        {
            perTypePercent[entry.Key] = entry.Value / sum;
        }
    }

    public void print(string prefix = "")
    {
        string print = prefix;
        foreach (KeyValuePair<UnitType, float> entry in perTypeSizeScore)
        {
            print += " | " + entry.Key + ":" + entry.Value + " (" + perTypePercent[entry.Key]*100 + ")";
        }

        print += "\n   vi vh vf\n";
        print += "i " + scoresAttackMatrix[(int)UnitType.Infantry, (int)UnitType.Infantry] + " " + scoresAttackMatrix[(int)UnitType.Infantry, (int)UnitType.Heavy] + " " + scoresAttackMatrix[(int)UnitType.Infantry, (int)UnitType.Flying] + "\n";
        print += "h " + scoresAttackMatrix[(int)UnitType.Heavy, (int)UnitType.Infantry] + " " + scoresAttackMatrix[(int)UnitType.Heavy, (int)UnitType.Heavy] + " " + scoresAttackMatrix[(int)UnitType.Heavy, (int)UnitType.Flying] + "\n";
        print += "f " + scoresAttackMatrix[(int)UnitType.Flying, (int)UnitType.Infantry] + " " + scoresAttackMatrix[(int)UnitType.Flying, (int)UnitType.Heavy] + " " + scoresAttackMatrix[(int)UnitType.Flying, (int)UnitType.Flying] + "\n";

        Debug.Log(print);
    }
}

public class Army
{
    public Dictionary<UnitList, int> troops = new Dictionary<UnitList, int>();

    private float scoreOverflow = 0;

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

    public List<UnitList> getTroopsOfType(UnitType type)
    {
        List<UnitList> list = new List<UnitList>();

        foreach(UnitList troop in troops.Keys)
        {
            if(Unit.getUnit(troop).type == type || type == UnitType.None)
            {
                list.Add(troop);
            }
        }

        return list;
    }

    public bool isEmpty()
    {
        return troops.Count == 0;
    }

    public void removeSomeTroops(float pointsToRemove, UnitType filter = UnitType.None)
    {
        List<UnitList> troopSubList = getTroopsOfType(filter);

        pointsToRemove += scoreOverflow;
        scoreOverflow = 0;

        while (pointsToRemove > 0 && troopSubList.Count > 0)
        {
            UnitList unitToHurt = troopSubList[(int)(Random.value * troopSubList.Count)];
            Unit unit = Unit.getUnit(unitToHurt);
            float unitScore = unit.scoreValue;

            if(filter == UnitType.None || filter == unit.type)
            {
                //Debug.Log("UnitScore " + unitScore + " / toRemove " + pointsToRemove);

                if(unitScore > pointsToRemove)
                {
                    if(Random.value < pointsToRemove/unitScore)
                    {
                        //killed
                        removeTroops(unitToHurt, 1);
                        //Debug.Log("Removed 1 " + unitToHurt);
                    }
                    else
                    {
                        //survived
                        //Debug.Log(unitToHurt + "Dodged");
                        scoreOverflow = pointsToRemove;
                    }

                    pointsToRemove = 0;
                }
                else
                {
                    float toRemoveHere = Random.value * pointsToRemove;
                    if (toRemoveHere < unitScore) toRemoveHere = unitScore;
                    float unitsToRemove = toRemoveHere / unitScore;
                    int actualRemoved = (int)unitsToRemove;

                    if(actualRemoved > troops[unitToHurt])
                    {
                        actualRemoved = troops[unitToHurt];
                    }

                    //Debug.Log("Maths said remove " + unitsToRemove + " " + unitToHurt + " rounded to " + actualRemoved);
                    //Debug.Log("Removed " + actualRemoved + " " + unitToHurt);
                    removeTroops(unitToHurt, actualRemoved);
                    pointsToRemove -= (int)(actualRemoved * unitScore);
                }
            }


            troopSubList = getTroopsOfType(filter); //preparing next loop
        }
    }

    public void removeTroops(UnitList unit, int number)
    {
        if (!troops.ContainsKey(unit))
        {
            Debug.LogError("Trying to remove troops not in army");
            return;
        }

        troops[unit] -= number;

        if(troops[unit] <= 0)
        {
            troops.Remove(unit);
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
            s.addTypeAttackScore(u.type, UnitType.Infantry, vi);
            s.addTypeAttackScore(u.type, UnitType.Heavy, vh);
            s.addTypeAttackScore(u.type, UnitType.Flying, vf);

            s.addTypeScore(u.type, score);

            //Debug.Log(u.name + " " + u.type + " S:" + score + ", vI:" + vi + " vH:" + vh + " vF:" + vf);
        }

        return s;
    }

    public override string ToString()
    {
        ArmyScore s = computeScore();

        string log = s.armySize.ToString() + " : ";


        foreach (KeyValuePair<UnitList, int> entry in troops)
        {
            log += entry.Value + " " + entry.Key + ". ";
        }

        return log;
    }

    public GameCost getTotalArmyCost()
    {
        GameCost c = new GameCost(0, 0);

        foreach (KeyValuePair<UnitList, int> entry in troops)
        {
            c += Unit.getCost(entry.Key, entry.Value);
        }

        return c;
    }

    public static Army createRandomAmryOfPoints(float points)
    {
        Army army = new Army();

        float currentScore = 0;

        while(currentScore < points)
        {
            UnitList newUnit = Unit.getRandomUnit();
            Unit u = Unit.getUnit(newUnit);

            float remainingPoints = points - currentScore;

            if(remainingPoints < u.scoreValue)
            {
                army.addTroops(newUnit, 1);
                currentScore += u.scoreValue;
            }
            else
            {
                float pointsToAddHere = remainingPoints * Random.value;
                int numberToAdd = Mathf.CeilToInt(pointsToAddHere / u.scoreValue);
                float scoreActuallyAdded = u.scoreValue * numberToAdd;
                currentScore += scoreActuallyAdded;
                army.addTroops(newUnit, numberToAdd);
            }
        }

        return army;
    }

    public static float getRelativeScore(Army armyA, Army armyB)
    {
        if (armyB.isEmpty()) return 1;
        if (armyA.isEmpty()) return 0;

        ArmyScore a = armyA.computeScore();
        ArmyScore b = armyB.computeScore();

        float total = a.armySize + b.armySize;

        return a.armySize / total;
    }
}
