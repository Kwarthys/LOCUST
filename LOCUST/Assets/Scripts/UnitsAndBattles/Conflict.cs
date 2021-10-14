using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Conflict : MonoBehaviour
{
    private Army playerArmy;
    private Army defenderArmy;

    public BattleManager battleManager;

    public void generateRandomEnemyArmy()
    {
        defenderArmy = new Army();
        defenderArmy.addTroops(UnitList.Samurai, 5000);

        playerArmy = new Army();
    }

    
}
