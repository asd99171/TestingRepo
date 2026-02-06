using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUIController : MonoBehaviour
{
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private TimerManager timerManager;
    [SerializeField] private GameFlowManager gameFlowManager;
    [SerializeField] private LeaderboardManager leaderboardManager;
    [SerializeField] private EvidenceCounterManager evidenceCounterManager;
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
    [SerializeField] private TextMeshProUGUI gunEvidenceText;
    [SerializeField] private TextMeshProUGUI gloveEvidenceText;
    [SerializeField] private TextMeshProUGUI bagEvidenceText;
    [SerializeField] private TextMeshProUGUI knifeEvidenceText;
    [SerializeField] private TextMeshProUGUI corpseEvidenceText;
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

        if (evidenceCounterManager != null)
        {
            evidenceCounterManager.EvidenceCountChanged += HandleEvidenceCountChanged;
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

        if (evidenceCounterManager != null)
        {
            evidenceCounterManager.EvidenceCountChanged -= HandleEvidenceCountChanged;
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

        if (evidenceCounterManager != null)
        {
            HandleEvidenceCountChanged(EvidenceType.Gun, evidenceCounterManager.GunCount);
            HandleEvidenceCountChanged(EvidenceType.Glove, evidenceCounterManager.GloveCount);
            HandleEvidenceCountChanged(EvidenceType.Bag, evidenceCounterManager.BagCount);
            HandleEvidenceCountChanged(EvidenceType.Knife, evidenceCounterManager.KnifeCount);
            HandleEvidenceCountChanged(EvidenceType.Corpse, evidenceCounterManager.CorpseCount);
        }

        if (catchCounterManager != null)
        {
            HandleCatchCountChanged(CatchType.Fish, catchCounterManager.FishCount);
            HandleCatchCountChanged(CatchType.Evidence, catchCounterManager.EvidenceCount);
            HandleCatchCountChanged(CatchType.Corpse, catchCounterManager.CorpseCount);
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

    private void HandleEvidenceCountChanged(EvidenceType type, int count)
    {
        switch (type)
        {
            case EvidenceType.Gun:
                if (gunEvidenceText != null)
                {
                    gunEvidenceText.text = $"Gun: {count}";
                }
                break;
            case EvidenceType.Glove:
                if (gloveEvidenceText != null)
                {
                    gloveEvidenceText.text = $"Glove: {count}";
                }
                break;
            case EvidenceType.Bag:
                if (bagEvidenceText != null)
                {
                    bagEvidenceText.text = $"Bag: {count}";
                }
                break;
            case EvidenceType.Knife:
                if (knifeEvidenceText != null)
                {
                    knifeEvidenceText.text = $"Knife: {count}";
                }
                break;
            case EvidenceType.Corpse:
                if (corpseEvidenceText != null)
                {
                    corpseEvidenceText.text = $"Corpse: {count}";
                }
                break;
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
