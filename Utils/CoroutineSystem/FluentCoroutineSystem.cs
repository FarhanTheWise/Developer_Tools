using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using DG.Tweening;

public class FluentCoroutineSystem
{

    private static CoutineRunner runner;
    private readonly Queue<IEnumerator> sequenceSteps = new();
    public Action onCompleteAction;

    public static FluentCoroutineSystem Begin()
    {
        EnsureRunner();
        return new FluentCoroutineSystem();
    }

    public FluentCoroutineSystem OnComplete(Action callback)
    {
        onCompleteAction = callback;
        return this;
    }

    public FluentCoroutineSystem AddRoutine(IEnumerator routine)
    {
        sequenceSteps.Enqueue(routine);
        return this;
    }

    public FluentCoroutineSystem AddRoutine(Func<IEnumerator> routine)
    {
        sequenceSteps.Enqueue(routine());
        return this;
    }

    public FluentCoroutineSystem AddAction(Action action)
    {
        sequenceSteps.Enqueue(ActionWrapper(action));
        return this;
    }
    
    
    public FluentCoroutineSystem AddWait(float waitTime)
    {
        sequenceSteps.Enqueue(WaitRoutine(waitTime));
        return this;
    }
    
    public FluentCoroutineSystem AddTween(Func<Tween> tween)
    {
        sequenceSteps.Enqueue(TweenWrapper(tween));
        return this;
    }

    public void Play()
    {
        runner.StartCoroutine(RunSequence());
    }

    private IEnumerator RunSequence()
    {
        while (sequenceSteps.Count > 0)
        {
            yield return sequenceSteps.Dequeue();
        }
        onCompleteAction?.Invoke();
    }

    private IEnumerator TweenWrapper(Func<Tween> tweenFactory)
    { 
        var tween = tweenFactory.Invoke();

        if (tween == null)
            yield break;

        tween.SetAutoKill(true);
        tween.Pause();      // force stop auto play
        tween.Play();       // now we control it

        while (tween.IsActive() && tween.IsPlaying())
            yield return null;
    }

    private IEnumerator ActionWrapper(Action action)
    {
        action?.Invoke();
        yield break;
    }
   
 
    private IEnumerator WaitRoutine(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
    }

    private static void EnsureRunner()
    {
        if (runner != null) return;

        var runnerObject = new GameObject("FluentCoroutineRunner");
        UnityEngine.Object.DontDestroyOnLoad(runnerObject);
        runner = runnerObject.AddComponent<CoutineRunner>();
       
    }

    private class CoutineRunner : MonoBehaviour { }


}


