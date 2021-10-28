using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Conflict : MonoBehaviour
{
    public BattleManager battleManager;

    public Color activatedColor;
    public Color deactivatedColor;
    public Color wonColor;

    public MarkerControler marker;
    public PlanetMarkerMovementControler markerMovement;

    public bool isWon { get; private set; } = false;

    public float lightIntensity = 3;

    private bool currentState = false;

    public void generateRandomEnemyArmy(float points)
    {
        battleManager.armyA = new Army();
        battleManager.armyB = Army.createRandomAmryOfPoints(points);
    }

    public void setState(bool activated)
    {
        if(isWon)
        {
            marker.mRenderer.material.SetColor("_EmissionColor", wonColor * lightIntensity / 2);
        }
        else
        {
            Color c;
            if (activated)
                c = activatedColor;
            else
                c = deactivatedColor;

            marker.mRenderer.material.SetColor("_EmissionColor", c * lightIntensity);
        }        

        markerMovement.setState(activated);

        battleManager.setDrawingState(activated);

        currentState = activated;
    }

    public void setWon()
    {
        isWon = true;
        setState(currentState);
    }


}
