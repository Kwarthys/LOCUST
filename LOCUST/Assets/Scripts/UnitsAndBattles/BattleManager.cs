using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public Army armyA = new Army();
    public Army armyB = new Army();

    public ArmyDisplayController armyDisplay;
    public ArmyDisplayController armyDisplay2;

    public float coefLossPerTick = 0.1f;

    void Start()
    {
        //Test battle

        armyA.addTroops(UnitList.Marine, 500);
        armyA.addTroops(UnitList.Samurai, 1200);
        armyA.addTroops(UnitList.Tank, 1);
        armyB.addTroops(UnitList.Marine, 200);
        armyB.addTroops(UnitList.Tank, 50);

        Debug.Log("A: " + armyA);
        Debug.Log("VERSUS");
        Debug.Log("B: " + armyB);

        for(int i = 0; i < 10; ++i)
        {
            //combat(armyA, armyB);
            //combat(armyB, armyA);
        }

        Debug.Log("Gives");
        Debug.Log("A: " + armyA);
        Debug.Log("B: " + armyB);

        armyDisplay.toDisplay = armyA;
        armyDisplay.armyName = "armyA";
        armyDisplay.displayArmy();

        armyDisplay2.toDisplay = armyB;
        armyDisplay2.armyName = "armyB";
        armyDisplay2.displayArmy();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void combat(Army armyA, Army armyB)
    {
        //Debug.Log("ArmyA");
        ArmyScore a = armyA.computeScore();
        //Debug.Log("ArmyB");
        ArmyScore b = armyB.computeScore();

        //a.print("A");
        //b.print("B");

        /*
        float antiInfantryAttack = b.getMatrix(UnitType.Infantry, UnitType.Infantry) + b.getMatrix(UnitType.Heavy, UnitType.Infantry) + b.getMatrix(UnitType.Flying, UnitType.Infantry);
        antiInfantryAttack *= b.perTypePercent[UnitType.Infantry];
        float infantryDefenseScore = b.getMatrix(UnitType.Infantry, UnitType.Infantry) * a.perTypePercent[UnitType.Infantry] + b.getMatrix(UnitType.Infantry, UnitType.Heavy) * a.perTypePercent[UnitType.Heavy] + b.getMatrix(UnitType.Infantry, UnitType.Flying) * a.perTypePercent[UnitType.Flying];

        Debug.Log("InfantryClash " + antiInfantryAttack + " A vs B " + infantryDefenseScore);
        */

        clash(UnitType.Infantry, a, b, out float axvi, out float bivx);
        clash(UnitType.Heavy, a, b, out float axvh, out float bhvx);
        clash(UnitType.Flying, a, b, out float axvf, out float bfvx);

        armyA.removeSomeTroops(axvi);
        armyB.removeSomeTroops(bivx, UnitType.Infantry);
        armyA.removeSomeTroops(axvh);
        armyB.removeSomeTroops(bhvx, UnitType.Heavy);
        armyA.removeSomeTroops(axvf);
        armyB.removeSomeTroops(bfvx, UnitType.Flying);
    }

    public void clash(UnitType typeClash, ArmyScore a, ArmyScore b, out float aLostAmount, out float bLostAmount)
    {
        float ascore = a.getMatrix(UnitType.Infantry, typeClash) + a.getMatrix(UnitType.Heavy, typeClash) + a.getMatrix(UnitType.Flying, typeClash);
        ascore *= b.perTypePercent[typeClash];

        //Debug.Log("ascore = (" + a.getMatrix(UnitType.Infantry, typeClash) + " + " + a.getMatrix(UnitType.Heavy, typeClash) + " + " + a.getMatrix(UnitType.Flying, typeClash) + ") * " + b.perTypePercent[typeClash]);

        float bscore = b.getMatrix(typeClash, UnitType.Infantry) * a.perTypePercent[UnitType.Infantry] + b.getMatrix(typeClash, UnitType.Heavy) * a.perTypePercent[UnitType.Heavy] + b.getMatrix(typeClash, UnitType.Flying) * a.perTypePercent[UnitType.Flying];

        //Debug.Log("bscore = " + b.getMatrix(typeClash, UnitType.Infantry) + " * " + a.perTypePercent[UnitType.Infantry] + " + " + b.getMatrix(typeClash, UnitType.Heavy) + " * " + a.perTypePercent[UnitType.Heavy] + " + " + b.getMatrix(typeClash, UnitType.Flying) + " * " + a.perTypePercent[UnitType.Flying]);

        aLostAmount = lossFunction(ascore, bscore) * a.armySize * b.perTypePercent[typeClash] * coefLossPerTick;
        bLostAmount = lossFunction(bscore, ascore) * b.armySize * b.perTypePercent[typeClash] * coefLossPerTick;

        //Debug.Log(typeClash + "Clash " + ascore + " A vs B " + bscore + ". A lost " + aLostAmount + ". B lost " + bLostAmount);
    }

    //loss in points of A in the fight A vs B (losses of B are 1-lossesOfA)
    public float lossFunction(float scoreA, float scoreB)
    {
        if (scoreA == 0 || scoreB == 0) return 0;

        return scoreB * scoreB / (scoreB * scoreB + scoreA * scoreA);
    }
}
