using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropPodControler : MonoBehaviour
{
    public float targetOffSurfaceHeight = 2;
    public float timeOfFlight = 5;
    private float startTime;

    private Vector3 target;
    private Vector3 startPos;

    private bool approaching = false;

    //All pos in local

    public void setupPod(Vector3 target)
    {
        startPos = transform.localPosition;

        if(Vector3.Angle(startPos, target) < 90)
        {
            this.target = target;
        }
        else
        {
            Vector3 normal = Vector3.Cross(target, startPos);
            this.target = Vector3.Cross(startPos, normal).normalized * targetOffSurfaceHeight;
        }

        approaching = true;

        startTime = Time.realtimeSinceStartup;
    }

    private void Start()
    {
        transform.localRotation = Quaternion.LookRotation(-target + startPos);
    }

    private void Update()
    {
        if (!approaching) return;

        float t = (Time.realtimeSinceStartup - startTime) / timeOfFlight;

        float logt = Mathf.Log10(9 * t + 1);

        if (t > 0.7f)
        {
            transform.localPosition = target;
            Destroy(gameObject);

            return;
        }

        transform.localPosition = Vector3.Lerp(startPos, target, logt);

        transform.localScale = new Vector3(.7f - t, .7f - t, .7f - t);
    }

    private void OnDrawGizmosSelected()
    {
        if (approaching)
        {
            Gizmos.DrawWireSphere(target, .5f);
            Gizmos.DrawLine(startPos, target);
        }
    }
}
