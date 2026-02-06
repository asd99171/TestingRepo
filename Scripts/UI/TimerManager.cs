using System;
using UnityEngine;

public class TimerManager : MonoBehaviour
{
    [SerializeField] private float roundDurationSeconds = 120f;

    public event Action<float> TimeChanged;
    public event Action TimerEnded;

    public float RemainingSeconds { get; private set; }
    public bool IsRunning { get; private set; }
    public float RoundDurationSeconds => roundDurationSeconds;

    public void StartTimer()
    {
        RemainingSeconds = roundDurationSeconds;
        IsRunning = true;
        TimeChanged?.Invoke(RemainingSeconds);
    }

    public void StopTimer()
    {
        IsRunning = false;
    }

    private void Update()
    {
        if (!IsRunning)
        {
            return;
        }

        RemainingSeconds = Mathf.Max(0f, RemainingSeconds - Time.deltaTime);
        TimeChanged?.Invoke(RemainingSeconds);

        if (RemainingSeconds <= 0f)
        {
            IsRunning = false;
            TimerEnded?.Invoke();
        }
    }
}
