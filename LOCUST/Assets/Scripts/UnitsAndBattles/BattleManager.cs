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
    private float minSizeThreshold = 500;

    public float tickRoundBattle = 1f; //fight(s) per second

    public TimedCallBack timer;

    private List<float> scoresA = new List<float>();
    private List<float> scoresB = new List<float>();

    private MyFileWriter logger = new MyFileWriter();

    //debug
    //private int c = 0;

    void Start()
    {
        timer.setup(tickRoundBattle, new TimerCallBack(this), false);
        //timer.go();

        //Test battle

        armyA.addTroops(UnitList.Samurai, 10);
        armyA.addTroops(UnitList.Bomber, 5);
        armyB.addTroops(UnitList.Tank, 50);
        armyB.addTroops(UnitList.Marine, 300);
        armyB.addTroops(UnitList.Fighter, 100);

        armyDisplay.displayArmy(armyA, "ArmyA");
        armyDisplay2.displayArmy(armyB, "ArmyB");
    }

    public void startBattle()
    {
        timer.go();
    }

    public void fight()
    {
        scoresA.Add(armyA.computeScore().armySize);
        scoresB.Add(armyB.computeScore().armySize);

        if (armyA.isEmpty() || armyB.isEmpty())
        {
            combat(armyA, armyB);
            timer.stop();

            string print = "[";
            for(int i = 0; i < scoresA.Count; ++i)
            {
                print += scoresA[i];
                if(i< scoresA.Count-1)
                {
                    print += ", ";
                }
            }
            print += "]";

            Debug.Log(print);

            print = "[";
            for (int i = 0; i < scoresB.Count; ++i)
            {
                print += scoresB[i];
                if (i < scoresB.Count - 1)
                {
                    print += ", ";
                }
            }
            print += "]";

            Debug.Log(print);

            logger.writeLog("ArmyA", scoresA, false);
            logger.writeLog("ArmyB", scoresB);


            return;
        }

        /*
        Debug.Log("A: " + armyA);
        Debug.Log("VERSUS");
        Debug.Log("B: " + armyB);
        */
        combat(armyA, armyB);
        //combat(armyB, armyA);
        
        //Debug.Log("Gives");
        //Debug.Log("A: " + armyA);
        //Debug.Log("B: " + armyB);
        
        armyDisplay.displayArmy();
        armyDisplay2.displayArmy();

        /*
        c++;
        if (c == 25)
        {
            armyA.addTroops(UnitList.Samurai, 1200);
        }
        */
    }


    public void combat(Army a, Army b)
    {
        clash(UnitType.Infantry, a, b);
        clash(UnitType.Heavy, a, b);
        clash(UnitType.Flying, a, b);
    }

    //battle brain
    public void clash(UnitType typeClash, Army armyA, Army armyB)//, out float aLostAmount, out float bLostAmount)
    {
        ArmyScore a = armyA.computeScore();
        ArmyScore b = armyB.computeScore();

        float ascore = a.getMatrix(UnitType.Infantry, typeClash) + a.getMatrix(UnitType.Heavy, typeClash) + a.getMatrix(UnitType.Flying, typeClash);
        ascore *= b.perTypePercent[typeClash];

        //Debug.Log("ascore = (" + a.getMatrix(UnitType.Infantry, typeClash) + " + " + a.getMatrix(UnitType.Heavy, typeClash) + " + " + a.getMatrix(UnitType.Flying, typeClash) + ") * " + b.perTypePercent[typeClash]);

        float bscore = b.getMatrix(typeClash, UnitType.Infantry) * a.perTypePercent[UnitType.Infantry] + b.getMatrix(typeClash, UnitType.Heavy) * a.perTypePercent[UnitType.Heavy] + b.getMatrix(typeClash, UnitType.Flying) * a.perTypePercent[UnitType.Flying];

        //Debug.Log("bscore = " + b.getMatrix(typeClash, UnitType.Infantry) + " * " + a.perTypePercent[UnitType.Infantry] + " + " + b.getMatrix(typeClash, UnitType.Heavy) + " * " + a.perTypePercent[UnitType.Heavy] + " + " + b.getMatrix(typeClash, UnitType.Flying) + " * " + a.perTypePercent[UnitType.Flying]);


        //MANY TESTS, third seems best


        //aLostAmount = lossFunction(ascore, bscore) * a.armySize * b.perTypePercent[typeClash] * coefLossPerTick;
        //bLostAmount = lossFunction(bscore, ascore) * b.armySize * b.perTypePercent[typeClash] * coefLossPerTick;

        //aLostAmount = lossFunction(ascore, bscore) * (a.armySize + b.armySize)/2 * b.perTypePercent[typeClash] * coefLossPerTick;
        //bLostAmount = lossFunction(bscore, ascore) * (a.armySize + b.armySize)/2 * b.perTypePercent[typeClash] * coefLossPerTick;

        float aLostAmount = lossFunction(ascore, bscore) * bscore * b.perTypePercent[typeClash];
        float bLostAmount = lossFunction(bscore, ascore) * ascore * b.perTypePercent[typeClash];

        float coef;

        //Gaining resolution as the armies shrink in sizes
        if (Mathf.Min(a.armySize, b.armySize) < minSizeThreshold)
        {
            coef = coefFunction(Mathf.Min(a.armySize, b.armySize));
        }
        else
        {
            coef = coefLossPerTick;
        }

        aLostAmount *= coef;
        bLostAmount *= coef;

        //Debug.Log(typeClash + " lossFunction(ascore, bscore) " + lossFunction(ascore, bscore));
        //Debug.Log("Coef " + coef + " :: " + typeClash + "Clash " + ascore + " A vs B " + bscore + ". A lost " + aLostAmount + ". B lost " + bLostAmount);

        //removetroops

        armyB.removeSomeTroops(bLostAmount, typeClash);

        float armyBtotal = b.getTotalByType(typeClash);

        armyA.removeSomeTroops(aLostAmount * b.getMatrix(typeClash, UnitType.Infantry) / armyBtotal, UnitType.Infantry);
        armyA.removeSomeTroops(aLostAmount * b.getMatrix(typeClash, UnitType.Heavy) / armyBtotal, UnitType.Heavy);
        armyA.removeSomeTroops(aLostAmount * b.getMatrix(typeClash, UnitType.Flying) / armyBtotal, UnitType.Flying);
    }

    private float coefFunction(float score)
    {
        float a = (1 - coefLossPerTick) / (0 - minSizeThreshold);
        float b = coefLossPerTick - a * minSizeThreshold;

        return a * score + b;
    }

    //loss in points of A in the fight A vs B (losses of B are 1-lossesOfA)
    public float lossFunction(float scoreA, float scoreB)
    {
        if (scoreA == 0 && scoreB == 0) return 0;

        return scoreB * scoreB / (scoreB * scoreB + scoreA * scoreA);
    }

    public class TimerCallBack : IMyCallBack
    {
        private BattleManager toCall;

        public TimerCallBack(BattleManager toCall)
        {
            this.toCall = toCall;
        }

        public void call()
        {
            toCall.fight();
        }
    }

    public void recruitForPlayer(UnitList unit, int number)
    {
        armyA.addTroops(unit, number);
        armyDisplay.displayArmy();
    }
}
