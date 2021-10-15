using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Conflict : MonoBehaviour
{
    public BattleManager battleManager;

    public void generateRandomEnemyArmy(float points)
    {
        battleManager.armyA = new Army();
        battleManager.armyB = Army.createRandomAmryOfPoints(points);
    }

    
}
