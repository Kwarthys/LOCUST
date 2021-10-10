using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetRotator : MonoBehaviour
{
    public float speed = 100;

    // Start is called before the first frame update
    void Start()
    {
        transform.Rotate(Vector3.left, Random.value * 20);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0,speed*Time.deltaTime,0));
    }
}
