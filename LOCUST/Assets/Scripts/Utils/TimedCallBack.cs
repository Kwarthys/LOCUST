using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TimerSettings
{
    public float timeToWait;
    public float startTime;
    public bool tickInstant;
    public bool running = false;

    public IMyCallBack cb;

    public TimerSettings(float timeToWait, IMyCallBack cb, bool tickInstant = false)
    {
        this.timeToWait = timeToWait;
        this.cb = cb;
        this.tickInstant = tickInstant;
    }
}

public class TimedCallBack : MonoBehaviour
{
    private List<TimerSettings> timers = new List<TimerSettings>();

    public int setup(TimerSettings timer)
    {
        timers.Add(timer);
        return timers.Count - 1;//returning the index of the newly added timer for further referencing
    }

    //putting this here, but trying not to use it
    public int getIndexOfTimer(TimerSettings timer)
    {
        return timers.IndexOf(timer);
    }

    public void stop(int timerIndex)
    {
        timers[timerIndex].running = false;
    }

    public void destroy(int timerIndex)
    {
        timers[timerIndex] = null;//keeping that index taken to avoid changing all indecies of following timers


        bool found = false;
        for(int i = timers.Count-1; !found && i >= 0; --i) //starting at the end
        {
            found = timers[i] != null;
        }

        if(!found)
        {
            //list does not contain any in use timers, we can then reset it
            timers.Clear();
        }
    }

    public void go(int timerIndex)
    {
        timers[timerIndex].startTime = Time.realtimeSinceStartup - (timers[timerIndex].tickInstant ? timers[timerIndex].timeToWait : 0);
        timers[timerIndex].running = true;
    }

    // Update is called once per frame
    void Update()
    {
        foreach(TimerSettings timer in timers)
        {
            if(timer != null)
            {
                if(timer.running)
                {
                    if(Time.realtimeSinceStartup - timer.startTime > timer.timeToWait)
                    {
                        timer.cb.call();
                        timer.startTime = Time.realtimeSinceStartup;
                    }
                }
            }
        }
    }
}

public interface IMyCallBack
{
    void call();
}
