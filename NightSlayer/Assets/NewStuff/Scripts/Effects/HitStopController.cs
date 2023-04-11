using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitStopController : MonoBehaviour
{
    public static HitStopController Instance { get; private set; }

    bool waiting;

    private void Awake()
    {
        Instance = this;
    }

    public void Stop(float duration)
    {
        if (waiting)
            return;
        Time.timeScale = 0f;
        StartCoroutine(Wait(duration));
    }

    IEnumerator Wait(float duration)
    {
        waiting = true;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1.0f;
        waiting = false;
    }
}