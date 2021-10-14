using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetConflictManager : MonoBehaviour
{
    public Camera cam;

    public Planet planet;

    public TimedCallBack timer;

    public ArmyDisplayController armyDisplay;
    public ArmyDisplayController armyDisplay2;

    public GameObject planetConflictMarkerPrefab;

    private bool initialized = false;

    private List<Conflict> conflicts = new List<Conflict>();

    public LayerMask markerLayerMask;

    public void Update()
    {
        //We wait the end of planet generation
        if(planet.generated && !initialized)
        {
            List<Vector3> surfacePoints = planet.surfacePoints;

            int nbPoint = (int)(Random.value * (surfacePoints.Count - 1)) + 1;
            Debug.Log("Genrated " + nbPoint + " conflicts");
            for (int i = 0; i < nbPoint; ++i)
            {
                GameObject marker = Instantiate(planetConflictMarkerPrefab, surfacePoints[i], Quaternion.LookRotation(-surfacePoints[i]), planet.transform);

                Conflict c = marker.GetComponent<Conflict>();
                c.generateRandomEnemyArmy();
                c.transform.name = "Conflict" + i;

                BattleManager bm = c.battleManager;
                bm.timer = timer;
                bm.armyDisplay = armyDisplay;
                bm.armyDisplay2 = armyDisplay2;

                conflicts.Add(c);
            }

            initialized = true;
        }

        if(Input.GetMouseButtonUp(0))
        {
            if(detectClickedConflict() != null)
            {
                //dotings
            }
        }
    }

    private Conflict detectClickedConflict()
    {
        Debug.Log("clic");

        RaycastHit hit;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 60, markerLayerMask))
        {
            Debug.Log("hit " + hit.collider.transform.parent.name);

            return hit.collider.transform.parent.GetComponent<Conflict>();
        }

        return null;
    }
}
