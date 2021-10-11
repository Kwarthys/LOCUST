using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetRotator : MonoBehaviour
{
    public float rps = 1f;

    // Start is called before the first frame update
    void Start()
    {
        transform.Rotate(Vector3.left, Random.value * 20);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0, 360 * rps * Time.deltaTime,0));
    }
}
