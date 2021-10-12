using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform horizontalHelper;
    public Transform verticalHelper;

    public Vector2 speeds;

    // Update is called once per frame
    void Update()
    {
        float v = Input.GetAxis("Vertical");
        float h = -Input.GetAxis("Horizontal");

        horizontalHelper.Rotate(new Vector3(0, h * speeds.x * Time.deltaTime, 0));
        verticalHelper.Rotate(new Vector3(v * speeds.y * Time.deltaTime, 0, 0));
    }
}
