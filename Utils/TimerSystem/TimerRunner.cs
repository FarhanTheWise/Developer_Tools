using System;
using System.Collections.Generic;
using UnityEngine;

public class TimerRunner : MonoBehaviour
{
    public static TimerRunner Instance;

    private List<FluentTimer> timers = new List<FluentTimer>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public void AddTimer(FluentTimer timer)
    {
        timers.Add(timer);
    }

    public void RemoveTimer(FluentTimer timer)
    {
        timer.Stop();
        timers.Remove(timer);
    }
    
    private void Update()
    {
        for (int i = timers.Count - 1; i >= 0; i--)
        {
            timers[i].Update();
            //timers[i].FormatText();
        }
    }

    private void OnDestroy()
    {
        timers.Clear();
        if(Instance != null)
        {
            Destroy(gameObject);
        }
    }
}
