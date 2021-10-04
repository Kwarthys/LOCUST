using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedCallBack : MonoBehaviour
{
    private float timeToWait;
    private float startTime;
    private bool tickInstant;
    private bool running = false;

    private IMyCallBack cb;

    public void setup(float timeToTick, IMyCallBack cb, bool tickInstant = false)
    {
        this.timeToWait = timeToTick;
        this.tickInstant = tickInstant;
        this.cb = cb;
    }

    public void stop()
    {
        running = false;
    }

    public void go()
    {
        startTime = Time.realtimeSinceStartup - (tickInstant ? timeToWait : 0);
        running = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(running)
        {
            //Debug.Log(Time.realtimeSinceStartup + " - " + startTime + " > " + timeToWait + " ? ");
            if(Time.realtimeSinceStartup - startTime > timeToWait)
            {
                cb.call();
                startTime = Time.realtimeSinceStartup;
            }
        }
    }
}

public interface IMyCallBack
{
    void call();
}
