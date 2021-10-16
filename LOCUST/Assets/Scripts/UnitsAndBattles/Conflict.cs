using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Conflict : MonoBehaviour
{
    public BattleManager battleManager;

    public Color activatedColor;
    public Color deactivatedColor;

    public MarkerControler marker;
    public PlanetMarkerMovementControler markerMovement;

    public float lightIntensity = 3;

    public void generateRandomEnemyArmy(float points)
    {
        battleManager.armyA = new Army();
        battleManager.armyB = Army.createRandomAmryOfPoints(points);
    }

    public void setState(bool activated)
    {
        Color c;
        if (activated)
            c = activatedColor;
        else
            c = deactivatedColor;

        marker.mRenderer.material.SetColor("_EmissionColor", c * lightIntensity);

        markerMovement.setState(activated);

        battleManager.setDrawingState(activated);
    }


}
