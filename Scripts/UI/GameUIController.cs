using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUIController : MonoBehaviour
{
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private TimerManager timerManager;
    [SerializeField] private GameFlowManager gameFlowManager;
    [SerializeField] private LeaderboardManager leaderboardManager;
    [SerializeField] private CatchCounterManager catchCounterManager;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI currentScoreText;
    [SerializeField] private TextMeshProUGUI comboText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private Image timerBarFill;
    [SerializeField] private TextMeshProUGUI stateText;
    [SerializeField] private TextMeshProUGUI leaderboardText;
    [SerializeField] private TextMeshProUGUI evidenceCountText;
    [SerializeField] private TextMeshProUGUI fishCountText;
    [SerializeField] private TextMeshProUGUI corpseCountText;
    [SerializeField] private GameObject gameFlowPanel;
    [SerializeField] private Button startButton;
    [SerializeField] private Button endButton;

    private void OnEnable()
    {
        if (scoreManager != null)
        {
            scoreManager.ScoreChanged += HandleScoreChanged;
            scoreManager.ComboChanged += HandleComboChanged;
        }

        if (timerManager != null)
        {
            timerManager.TimeChanged += HandleTimeChanged;
        }

        if (gameFlowManager != null)
        {
            gameFlowManager.StateChanged += HandleStateChanged;
        }

        if (leaderboardManager != null)
        {
            leaderboardManager.BestScoreChanged += HandleBestScoreChanged;
        }

        if (catchCounterManager != null)
        {
            catchCounterManager.CatchCountChanged += HandleCatchCountChanged;
        }

        if (startButton != null)
        {
            startButton.onClick.AddListener(HandleStartButtonClicked);
        }

        if (endButton != null)
        {
            endButton.onClick.AddListener(HandleEndButtonClicked);
        }
    }

    private void OnDisable()
    {
        if (scoreManager != null)
        {
            scoreManager.ScoreChanged -= HandleScoreChanged;
            scoreManager.ComboChanged -= HandleComboChanged;
        }

        if (timerManager != null)
        {
            timerManager.TimeChanged -= HandleTimeChanged;
        }

        if (gameFlowManager != null)
        {
            gameFlowManager.StateChanged -= HandleStateChanged;
        }

        if (leaderboardManager != null)
        {
            leaderboardManager.BestScoreChanged -= HandleBestScoreChanged;
        }

        if (catchCounterManager != null)
        {
            catchCounterManager.CatchCountChanged -= HandleCatchCountChanged;
        }

        if (startButton != null)
        {
            startButton.onClick.RemoveListener(HandleStartButtonClicked);
        }

        if (endButton != null)
        {
            endButton.onClick.RemoveListener(HandleEndButtonClicked);
        }
    }

    private void Start()
    {
        if (scoreManager != null)
        {
            HandleScoreChanged(scoreManager.Score);
            HandleComboChanged(scoreManager.ComboMultiplier);
        }

        if (timerManager != null)
        {
            HandleTimeChanged(timerManager.RemainingSeconds);
        }

        if (gameFlowManager != null)
        {
            HandleStateChanged(gameFlowManager.CurrentState);
        }

        if (leaderboardManager != null)
        {
            HandleBestScoreChanged(leaderboardManager.BestScore);
        }

        if (catchCounterManager != null)
        {
            UpdateCatchCountTexts();
        }
    }

    private void HandleScoreChanged(int newScore)
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {newScore}";
        }

        if (currentScoreText != null)
        {
            currentScoreText.text = $"Current: {newScore}";
        }
    }

    private void HandleComboChanged(float newCombo)
    {
        if (comboText != null)
        {
            comboText.text = $"Combo x{newCombo:0.00}";
        }
    }

    private void HandleTimeChanged(float remainingSeconds)
    {
        if (timerManager != null && timerBarFill != null)
        {
            float normalized = timerManager.RemainingSeconds / Mathf.Max(1f, timerManager.RoundDurationSeconds);
            timerBarFill.fillAmount = Mathf.Clamp01(normalized);
        }

        if (timerText != null)
        {
            timerText.text = $"{Mathf.CeilToInt(remainingSeconds)}";
        }
    }

    private void HandleStateChanged(GameState state)
    {
        if (gameFlowPanel != null)
        {
            gameFlowPanel.SetActive(state == GameState.Ended);
        }

        if (stateText != null)
        {
            stateText.text = $"State: {state}";
        }
    }

    private void HandleBestScoreChanged(int bestScore)
    {
        if (leaderboardText == null)
        {
            return;
        }

        leaderboardText.text = $"Best: {bestScore}";
    }

    private void HandleCatchCountChanged(CatchType type, int count)
    {
        switch (type)
        {
            case CatchType.Fish:
                if (fishCountText != null)
                {
                    fishCountText.text = $"Fish: {count}";
                }
                break;
            case CatchType.Evidence:
                if (evidenceCountText != null)
                {
                    evidenceCountText.text = $"Evidence: {count}";
                }
                break;
            case CatchType.Corpse:
                if (corpseCountText != null)
                {
                    corpseCountText.text = $"Corpse: {count}";
                }
                break;
        }
    }

    private void UpdateCatchCountTexts()
    {
        if (catchCounterManager == null)
        {
            return;
        }

        if (fishCountText != null)
        {
            fishCountText.text = $"Fish: {catchCounterManager.FishCount}";
        }

        if (evidenceCountText != null)
        {
            evidenceCountText.text = $"Evidence: {catchCounterManager.EvidenceCount}";
        }

        if (corpseCountText != null)
        {
            corpseCountText.text = $"Corpse: {catchCounterManager.CorpseCount}";
        }
    }

    private void HandleStartButtonClicked()
    {
        gameFlowManager?.StartGame();
    }

    private void HandleEndButtonClicked()
    {
        gameFlowManager?.EndGame();
    }
}
