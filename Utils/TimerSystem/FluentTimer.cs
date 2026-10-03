using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FluentTimer
{
    private float hourDuration;
    private float minuteDuration;
    private float secondDuration;
    private float totalDuration;
    private float elapsed;

    private bool isRunning;
    private bool isLooping;
    private bool useUnscaledTime;

    private Action onStart = delegate { };
    private Action<float> onUpdate = delegate { };
    private Action onComplete = delegate { };
    private TextMeshProUGUI timerText;
    private Image timerImage;
    private GameObject timerImageParent;
    private bool isFillUp = false;

    public static FluentTimer Create()
    {
        var newInstance = new FluentTimer();
        TimerRunner.Instance.AddTimer(newInstance);
        return newInstance;
    }

    public FluentTimer SetTimerText(TextMeshProUGUI text)
    {
        timerText = text;
        timerText.gameObject.SetActive(true);
        return this;
    }

    public FluentTimer SetTimerImage(Image fillerImage, GameObject parent, bool isFillerUp = true)
    {
        isFillUp = isFillerUp;
        timerImage = fillerImage;
        timerImage.fillAmount = 1f;
        timerImageParent = parent;
        timerImageParent.SetActive(true);
        return this;
    }

    public FluentTimer SetHours(float hours)
    {
        hourDuration = hours * 3600f;
        totalDuration += hourDuration;
        return this;
    }

    public FluentTimer SetMinutes(float minutes)
    {
        minuteDuration = minutes * 60f;
        totalDuration += minuteDuration;
        return this;
    }

    public FluentTimer SetSeconds(float seconds)
    {
        secondDuration = seconds;
        totalDuration += secondDuration;
        return this;
    }

    public FluentTimer SetLooping(bool looping)
    {
        isLooping = looping;
        return this;
    }

    public FluentTimer SetUseUnscaledTime(bool unscaled)
    {
        useUnscaledTime = unscaled;
        return this;
    }

    public FluentTimer OnStart(Action action)
    {
        onStart += action;
        return this;
    }

    public FluentTimer OnUpdate(Action<float> action)
    {
        onUpdate += action;
        return this;
    }

    public FluentTimer OnComplete(Action action)
    {
        onComplete += action;
        return this;
    }

    public FluentTimer Start()
    {
        elapsed = 0f;
        isRunning = true;

        onStart?.Invoke();
        return this;
    }

     public void Stop()
    {
        isRunning = false;
        //if(timerText != null) timerText.gameObject.SetActive(false);
    }

    public void Restart()
    {
        Start();
    }

    public void Pause()
    {
        isRunning = false;
    }

    public void Resume()
    {
        isRunning = true;
    }

    public void Update()
    {
        if (!isRunning) return;

        float delta = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;

        elapsed += delta;

        FormatText();
        FormatFiller();

        var progress = Mathf.Clamp01(elapsed / totalDuration);
        onUpdate?.Invoke(progress);

        if (elapsed >= totalDuration)
        {
            onComplete?.Invoke();

            if (isLooping)
            {
                elapsed = 0f;
            }
            else
            {
                Stop();
            }
        }
    }

    private void FormatFiller()
    {
        if (timerImage == null) return;

        if (isFillUp)
        {
            var fillAmount = Mathf.Clamp01(1f - (elapsed / totalDuration));
            timerImage.fillAmount = fillAmount;

            if(timerImage.fillAmount <= 0f)
            {
                timerImage.fillAmount = 0f;
                timerImageParent.SetActive(false);
            }
        }
        else
        {
            var fillAmount = Mathf.Clamp01(elapsed / totalDuration);
            timerImage.fillAmount = fillAmount;

            if(timerImage.fillAmount >= 1f)
            {
                timerImage.fillAmount = 1f;
                timerImageParent.SetActive(false);
            }
        }
    }

    private void FormatText()
    {
        if (timerText == null) return;
        
        var timeSpan = TimeSpan.FromSeconds(totalDuration - elapsed);

        var hoursString = timeSpan.Hours > 0 ? $"{timeSpan.Hours:D2} : " : "";
        var minutesString = timeSpan.Minutes > 0 ? $"{timeSpan.Minutes:D2} : " : "";
        var secondsString = $"{timeSpan.Seconds:D2}";

        timerText.text = $"{hoursString}{minutesString}{secondsString}";

        if(elapsed >= totalDuration)
        {
            timerText.gameObject.SetActive(false);
        }

    }
}
