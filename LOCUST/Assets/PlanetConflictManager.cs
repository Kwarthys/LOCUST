using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetConflictManager : MonoBehaviour
{
    public Camera cam;

    public Planet planet;

    public TimedCallBack timer;

    public ArmyDisplayController armyDisplayPlayer;
    public ArmyDisplayController armyDisplayDefense;

    public GameObject planetConflictMarkerPrefab;
    public GameObject dropPodPrefab;

    private bool initialized = false;

    private List<Conflict> conflicts = new List<Conflict>();

    private Conflict activeConflict = null;

    public LayerMask markerLayerMask;

    public PlayerResources playerResources;

    private Army playerReserveArmy = new Army();
    public ArmyDisplayController reserveArmyDisplay;

    private void Start()
    {
        reserveArmyDisplay.displayArmy(playerReserveArmy, "Reserves");
    }

    public void Update()
    {
        //We wait the end of planet generation
        if(planet.generated && !initialized)
        {
            List<Vector3> surfacePoints = planet.surfacePoints;

            int nbPoint = (int)(Random.value * (surfacePoints.Count - 2)) + 2;
            for (int i = 0; i < nbPoint; ++i)
            {
                GameObject marker = Instantiate(planetConflictMarkerPrefab, surfacePoints[i], Quaternion.LookRotation(-surfacePoints[i]), planet.transform);

                Conflict c = marker.GetComponent<Conflict>();
                c.generateRandomEnemyArmy(Random.value * 5000 + 5000);
                c.transform.name = "Conflict" + i;

                BattleManager bm = c.battleManager;
                bm.timer = timer;
                bm.armyDisplayPlayer = armyDisplayPlayer;
                bm.armyDisplayDefense = armyDisplayDefense;
                bm.playerResources = playerResources;
                bm.conflict = c;

                c.setState(false);
                conflicts.Add(c);
            }

            initialized = true;
        }

        if(Input.GetMouseButtonUp(0))
        {
            Conflict clickedConflict = detectClickedConflict();
            if (clickedConflict != null)
            {
                if(activeConflict != clickedConflict)
                {
                    if(activeConflict != null)
                    {
                        activeConflict.setState(false);
                    }

                    activeConflict = clickedConflict;
                    activeConflict.setState(true);
                }
            }
        }

        //checking planet won
        int won = 0;
        bool victory = true;
        foreach(Conflict c in conflicts)
        {
            if(c.isWon)
            {
                won++;
            }

            victory = victory && c.isWon;
        }

        if (Random.value > 0.99)
        {
            //Debug.Log("Won " + won + "/" + conflicts.Count);
            if(victory)
            {
                Debug.Log("Victory!");
            }
        }
    }

    public void onSendTroopsClick()
    {
        if (activeConflict != null && !playerReserveArmy.isEmpty())
        {
            if(!activeConflict.battleManager.isBattleFinished())
            {
                DropPodControler dpC = Instantiate(dropPodPrefab, cam.transform.position, Quaternion.identity, planet.transform).GetComponent<DropPodControler>();
                dpC.setupPod(activeConflict.transform.localPosition);

                activeConflict.battleManager.addToPlayerArmy(playerReserveArmy);
                playerReserveArmy = new Army();
                reserveArmyDisplay.displayArmy(playerReserveArmy, "Reserves");
            }
                
        }
    }

    private Conflict detectClickedConflict()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 60, markerLayerMask))
        {
            return hit.collider.transform.parent.GetComponent<Conflict>();
        }

        return null;
    }

    public void recruitForPlayer(UnitList unit, int number)
    {
        if(playerResources.tryBuy(Unit.getCost(unit, number)))
        {
            playerReserveArmy.addTroops(unit, number);
            reserveArmyDisplay.displayArmy();
        }
    }
}
