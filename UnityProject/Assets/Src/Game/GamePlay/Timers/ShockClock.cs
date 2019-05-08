using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockClock
{
    private Action<double> _intervalCallback;
    private Action _finishCallback;
    private double _totalTime;
    private double _endTime;
    private double _currentTime;
    private double _startTime;
    
    private double _interval;
    private Coroutine _timerRoutine;

    // Time / interval measured in milliseconds
	public void Start(double totalTime, double interval, Action<double> intervalCallback, Action finishCallback)
    {
        _startTime = GetTime();
        _endTime = _startTime + totalTime;
        _totalTime = totalTime;
        _interval = interval;

        _finishCallback = finishCallback;
        _intervalCallback = intervalCallback;
        Debug.LogErrorFormat("TotalTIme: {0}, interval: {1}, startTime: {2}", _totalTime, _interval, _startTime);

        if(_timerRoutine != null)
        {
            Singleton.instance.StopCoroutine(_timerRoutine);
        }

        _timerRoutine = Singleton.instance.StartCoroutine(onTimer());
    }

    public void Stop()
    {
        Debug.Log("Stopped Timer!");
        Singleton.instance.StopCoroutine(_timerRoutine);
    }


    protected IEnumerator onTimer()
    {
        while (true)
        {
            float waitTime = (float)(_interval / 1000.0f);
            Debug.Log("WaitTime: " + waitTime);
            yield return new WaitForSecondsRealtime(waitTime);

            double now = GetTime();
            _currentTime = now - _startTime;

            if (_intervalCallback != null)
            {
                _intervalCallback(_currentTime);
            }
            Debug.LogFormat("CurrentTIme: {0}/{1}", _currentTime, _totalTime);
            if (now >= _endTime)
            {
                if (_finishCallback != null)
                {
                    _finishCallback();
                }
                Debug.LogFormat("Timer Finished!");
                break;
            }
        }
    }

    public virtual double GetTime()
    {
        return System.DateTime.UtcNow.Millisecond;
    }
}
