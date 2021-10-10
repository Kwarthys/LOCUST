using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightRotator : MonoBehaviour
{
    public Transform target;

    private void OnValidate()
    {
        transform.rotation = Quaternion.LookRotation(target.position - transform.position);
    }
}
