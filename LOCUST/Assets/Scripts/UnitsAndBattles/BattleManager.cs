using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public Army armyA = new Army();
    public Army armyB = new Army();


    void Start()
    {
        armyA.addTroops(UnitList.Marine, 1000);
        armyB.addTroops(UnitList.Tank, 50);

        combat(armyA, armyB);

        //will have to remove that later, when combat will be working
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
      Application.Quit();
#endif
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void combat(Army armyA, Army armyB)
    {
        Debug.Log("ArmyA");
        ArmyScore a = armyA.computeScore();
        Debug.Log("ArmyB");
        ArmyScore b = armyB.computeScore();
    }
}
