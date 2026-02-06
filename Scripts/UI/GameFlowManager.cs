using System;
using UnityEngine;

public class GameFlowManager : MonoBehaviour
{
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private TimerManager timerManager;
    [SerializeField] private LeaderboardManager leaderboardManager;
    [SerializeField] private EvidenceCounterManager evidenceCounterManager;
    [SerializeField] private CatchCounterManager catchCounterManager;

    public event Action<GameState> StateChanged;

    public GameState CurrentState { get; private set; } = GameState.Idle;

    private void Awake()
    {
        if (timerManager != null)
        {
            timerManager.TimerEnded += HandleTimerEnded;
        }

        if (scoreManager != null)
        {
            scoreManager.SetScoringEnabled(false);
        }

        if (evidenceCounterManager != null)
        {
            evidenceCounterManager.SetCountingEnabled(false);
        }

        if (catchCounterManager != null)
        {
            catchCounterManager.SetCountingEnabled(false);
        }
    }

    private void OnDestroy()
    {
        if (timerManager != null)
        {
            timerManager.TimerEnded -= HandleTimerEnded;
        }
    }

    private void Update()
    {
        if (CurrentState != GameState.Running || timerManager == null)
        {
            return;
        }

        if (!timerManager.IsRunning && timerManager.RemainingSeconds <= 0f)
        {
            EndGame();
        }
    }

    public void StartGame()
    {
        if (CurrentState == GameState.Running)
        {
            return;
        }

        scoreManager?.ResetScore();
        scoreManager?.SetScoringEnabled(true);
        evidenceCounterManager?.ResetCounts();
        evidenceCounterManager?.SetCountingEnabled(true);
        catchCounterManager?.ResetCounts();
        catchCounterManager?.SetCountingEnabled(true);
        timerManager?.StartTimer();
        SetState(GameState.Running);
    }

    public void EndGame()
    {
        if (CurrentState == GameState.Ended)
        {
            return;
        }

        timerManager?.StopTimer();
        if (scoreManager != null)
        {
            leaderboardManager?.RecordScore(scoreManager.Score);
            scoreManager.ResetScore();
            scoreManager.SetScoringEnabled(false);
        }

        if (evidenceCounterManager != null)
        {
            evidenceCounterManager.SetCountingEnabled(false);
        }

        if (catchCounterManager != null)
        {
            catchCounterManager.SetCountingEnabled(false);
        }
        SetState(GameState.Ended);
    }

    public void RestartGame()
    {
        EndGame();
        StartGame();
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void HandleTimerEnded()
    {
        if (CurrentState == GameState.Running)
        {
            EndGame();
        }
    }

    private void SetState(GameState newState)
    {
        CurrentState = newState;
        StateChanged?.Invoke(CurrentState);
    }
}
