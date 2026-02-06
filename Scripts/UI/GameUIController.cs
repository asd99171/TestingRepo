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
    [SerializeField] private TextMeshProUGUI comboText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private Image timerBarFill;
    [SerializeField] private TextMeshProUGUI stateText;
    [SerializeField] private TextMeshProUGUI leaderboardText;
    [SerializeField] private TextMeshProUGUI gunEvidenceText;
    [SerializeField] private TextMeshProUGUI gloveEvidenceText;
    [SerializeField] private TextMeshProUGUI bagEvidenceText;
    [SerializeField] private TextMeshProUGUI knifeEvidenceText;
    [SerializeField] private TextMeshProUGUI corpseEvidenceText;
    [SerializeField] private GameObject gameFlowPanel;

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
            leaderboardManager.LeaderboardChanged += HandleLeaderboardChanged;
        }

        if (evidenceCounterManager != null)
        {
            evidenceCounterManager.EvidenceCountChanged += HandleEvidenceCountChanged;
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
            leaderboardManager.LeaderboardChanged -= HandleLeaderboardChanged;
        }

        if (evidenceCounterManager != null)
        {
            evidenceCounterManager.EvidenceCountChanged -= HandleEvidenceCountChanged;
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
            HandleLeaderboardChanged(leaderboardManager.Scores);
        }

        if (evidenceCounterManager != null)
        {
            HandleEvidenceCountChanged(EvidenceType.Gun, evidenceCounterManager.GunCount);
            HandleEvidenceCountChanged(EvidenceType.Glove, evidenceCounterManager.GloveCount);
            HandleEvidenceCountChanged(EvidenceType.Bag, evidenceCounterManager.BagCount);
            HandleEvidenceCountChanged(EvidenceType.Knife, evidenceCounterManager.KnifeCount);
            HandleEvidenceCountChanged(EvidenceType.Corpse, evidenceCounterManager.CorpseCount);
        }
    }

    private void HandleScoreChanged(int newScore)
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {newScore}";
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

    private void HandleLeaderboardChanged(System.Collections.Generic.IReadOnlyList<int> scores)
    {
        if (leaderboardText == null)
        {
            return;
        }

        if (scores.Count == 0)
        {
            leaderboardText.text = "Leaderboard: -";
            return;
        }

        string leaderboardOutput = "Leaderboard";
        for (int i = 0; i < scores.Count; i++)
        {
            leaderboardOutput += $"\n{i + 1}. {scores[i]}";
        }

        leaderboardText.text = leaderboardOutput;
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
}