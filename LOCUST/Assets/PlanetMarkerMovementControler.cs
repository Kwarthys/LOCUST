using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetMarkerMovementControler : MonoBehaviour
{
    public float upDownSpeed = 1;
    public float upDownAmplitude = 1;
    public float rotationSpeed = 100;

    public Transform child;

    private bool animated = false;

    private void Update()
    {
        transform.Rotate(new Vector3(0, 0, rotationSpeed * Time.deltaTime));

        if(animated)
            child.localPosition = new Vector3(0, 0, upDownAmplitude * (Mathf.Sin(upDownSpeed * Time.realtimeSinceStartup) + 1) / -2);
    }

    public void setState(bool activated)
    {
        if(!activated)
        {
            child.localPosition = Vector3.zero;
        }

        animated = activated;
    }

}