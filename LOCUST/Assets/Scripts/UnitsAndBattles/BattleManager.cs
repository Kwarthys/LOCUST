using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public Army armyA;
    public Army armyB;

    public ArmyDisplayController armyDisplay;
    public ArmyDisplayController armyDisplay2;

    public float coefLossPerTick = 0.01f;
    private float minSizeThreshold = 2000;

    public float tickRoundBattle = 1f; //fight(s) per second

    public TimedCallBack timer;

    public float refreshRate = 1;

    private List<float> scoresA = new List<float>();
    private List<float> scoresB = new List<float>();

    private MyFileWriter logger = new MyFileWriter();

    private int battleTickTimerIndex = -1;
    private int redrawTimerIndex = -1;

    private bool battlePaused = true;
    private bool battleFinished = false;

    public PlayerResources playerResources;

    //debug
    //private int c = 0;

    void Start()
    {
        battleTickTimerIndex = timer.setup(new TimerSettings(tickRoundBattle, new TimerCallBack(this), false));
        redrawTimerIndex = timer.setup(new TimerSettings(refreshRate, new TimerRedrawCallBack(this), true));
    }

    public void startBattle()
    {
        timer.go(battleTickTimerIndex);
    }

    public void setDrawingState(bool state)
    {
        if (state)
        {
            armyDisplay.displayArmy(armyA, "Player army");
            armyDisplay2.displayArmy(armyB, "Defending army");
            timer.go(redrawTimerIndex);
        }
        else if (redrawTimerIndex != -1)
        {
            timer.stop(redrawTimerIndex);
        }
    }

    public void fight()
    {
        scoresA.Add(armyA.computeScore().armySize);
        scoresB.Add(armyB.computeScore().armySize);

        if (armyA.isEmpty() && !battlePaused)
        {
            battlePaused = true;

            timer.stop(battleTickTimerIndex);
            timer.stop(redrawTimerIndex);

            return;
        }

        if (armyB.isEmpty() && !battleFinished)
        {
            battleFinished = true;

            timer.stop(battleTickTimerIndex);
            timer.stop(redrawTimerIndex);

            logger.writeLog("ArmyA", scoresA, false);
            logger.writeLog("ArmyB", scoresB);
        }

        Debug.Log("Fight : bP " + battlePaused + ". bF " + battleFinished);

        if(battlePaused || battleFinished)
        {
            return;
        }

        combat(armyA, armyB);
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
        //Debug.Log("Coef " + coef + " :" + Mathf.Min(a.armySize, b.armySize) + ": " + typeClash + "Clash " + ascore + " A vs B " + bscore + ". A lost " + aLostAmount + ". B lost " + bLostAmount);

        //removetroops

        armyB.removeSomeTroops(bLostAmount, typeClash);

        float armyBtotal = b.getTotalByType(typeClash);

        armyA.removeSomeTroops(aLostAmount * b.getMatrix(typeClash, UnitType.Infantry) / armyBtotal, UnitType.Infantry);
        armyA.removeSomeTroops(aLostAmount * b.getMatrix(typeClash, UnitType.Heavy) / armyBtotal, UnitType.Heavy);
        armyA.removeSomeTroops(aLostAmount * b.getMatrix(typeClash, UnitType.Flying) / armyBtotal, UnitType.Flying);
    }

    private float coefFunction(float score)
    {
        /*
        float a = (1 - coefLossPerTick) / (0 - minSizeThreshold);
        float b = coefLossPerTick - a * minSizeThreshold;

        return (a * score + b);
        */

        float a = (coefLossPerTick - 1) / (- minSizeThreshold * minSizeThreshold);
        float b = -a * 2 * minSizeThreshold;

        return a * score * score + b * score + 1;
    }

    //loss in points of A in the fight A vs B (losses of B are 1-lossesOfA)
    public float lossFunction(float scoreA, float scoreB)
    {
        if (scoreA == 0 && scoreB == 0) return 0;

        return scoreB * scoreB / (scoreB * scoreB + scoreA * scoreA);
    }

    public void redrawArmies()
    {
        armyDisplay.displayArmy();
        armyDisplay2.displayArmy();
    }

    public void recruitForPlayer(UnitList unit, int number)
    {
        if (battleFinished) return;

        if (playerResources.tryBuy(Unit.getCost(unit, number)))
        {
            armyA.addTroops(unit, number);

            if (battlePaused)
            {
                //restartBattle
                timer.go(battleTickTimerIndex);
                timer.go(redrawTimerIndex);

                battlePaused = false;
            }
        }
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

    public class TimerRedrawCallBack : IMyCallBack
    {
        private BattleManager toCall;

        public TimerRedrawCallBack(BattleManager toCall)
        {
            this.toCall = toCall;
        }

        public void call()
        {
            toCall.redrawArmies();
        }
    }
}
