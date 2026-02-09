using System.Collections;
using TMPro;
using UnityEngine;

public sealed class SideQuestMissionManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameFlowManager gameFlowManager;
    [SerializeField] private EvidenceCounterManager evidenceCounterManager;
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private TextMeshProUGUI missionText;

    [Header("Mission Settings")]
    [SerializeField] private float missionCheckIntervalSeconds = 15f;
    [SerializeField, Range(0f, 1f)] private float missionChance = 0.5f;
    [SerializeField] private int requiredStreak = 3;
    [SerializeField] private float successFadeSeconds = 1.5f;
    [SerializeField] private string missionPrefix = "Side Quest";

    private readonly EvidenceType[] evidenceTypes =
    {
        EvidenceType.Gun,
        EvidenceType.Glove,
        EvidenceType.Bag,
        EvidenceType.Knife,
        EvidenceType.Corpse
    };

    private EvidenceType currentTarget;
    private bool missionActive;
    private int currentStreak;
    private Coroutine missionRoutine;
    private Coroutine fadeRoutine;
    private int lastGunCount;
    private int lastGloveCount;
    private int lastBagCount;
    private int lastKnifeCount;
    private int lastCorpseCount;

    private void OnEnable()
    {
        if (gameFlowManager != null)
        {
            gameFlowManager.StateChanged += HandleStateChanged;
        }

        if (evidenceCounterManager != null)
        {
            evidenceCounterManager.EvidenceCountChanged += HandleEvidenceCountChanged;
            CacheEvidenceCounts();
        }

        if (gameFlowManager != null && gameFlowManager.CurrentState == GameState.Running)
        {
            BeginMissionLoop();
        }
    }

    private void OnDisable()
    {
        if (gameFlowManager != null)
        {
            gameFlowManager.StateChanged -= HandleStateChanged;
        }

        if (evidenceCounterManager != null)
        {
            evidenceCounterManager.EvidenceCountChanged -= HandleEvidenceCountChanged;
        }

        EndMissionLoop();
    }

    private void HandleStateChanged(GameState state)
    {
        if (state == GameState.Running)
        {
            CacheEvidenceCounts();
            BeginMissionLoop();
            return;
        }

        EndMissionLoop();
        ClearMissionText();
    }

    private void BeginMissionLoop()
    {
        if (missionRoutine != null)
        {
            return;
        }

        missionRoutine = StartCoroutine(MissionLoop());
    }

    private void EndMissionLoop()
    {
        if (missionRoutine != null)
        {
            StopCoroutine(missionRoutine);
            missionRoutine = null;
        }

        missionActive = false;
        currentStreak = 0;
    }

    private IEnumerator MissionLoop()
    {
        while (gameFlowManager != null && gameFlowManager.CurrentState == GameState.Running)
        {
            yield return new WaitForSeconds(missionCheckIntervalSeconds);

            if (missionActive)
            {
                continue;
            }

            if (Random.value <= missionChance)
            {
                StartMission();
            }
        }

        missionRoutine = null;
    }

    private void StartMission()
    {
        missionActive = true;
        currentStreak = 0;
        currentTarget = evidenceTypes[Random.Range(0, evidenceTypes.Length)];

        if (fadeRoutine != null)
        {
            StopCoroutine(fadeRoutine);
            fadeRoutine = null;
        }

        if (missionText != null)
        {
            missionText.gameObject.SetActive(true);
            missionText.text = $"{missionPrefix}: Catch {currentTarget} x{requiredStreak} in a row!";
            Color color = Color.white;
            color.a = 1f;
            missionText.color = color;
        }
    }

    private void HandleEvidenceCountChanged(EvidenceType type, int count)
    {
        int previousCount = GetPreviousCount(type);
        SetPreviousCount(type, count);

        if (count <= previousCount)
        {
            return;
        }

        if (!missionActive)
        {
            return;
        }

        if (type == currentTarget)
        {
            currentStreak += count - previousCount;
            if (currentStreak >= requiredStreak)
            {
                CompleteMission();
            }

            return;
        }

        currentStreak = 0;
    }

    private void CompleteMission()
    {
        missionActive = false;
        currentStreak = 0;
        scoreManager?.MultiplyScore(2f);

        if (missionText != null)
        {
            missionText.text = $"{missionPrefix}: Success! Score x2!";
            missionText.color = Color.green;
        }

        if (fadeRoutine != null)
        {
            StopCoroutine(fadeRoutine);
        }

        fadeRoutine = StartCoroutine(FadeMissionText());
    }

    private IEnumerator FadeMissionText()
    {
        if (missionText == null)
        {
            yield break;
        }

        float elapsed = 0f;
        Color startColor = missionText.color;

        while (elapsed < successFadeSeconds)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / Mathf.Max(0.01f, successFadeSeconds));
            Color color = startColor;
            color.a = Mathf.Lerp(startColor.a, 0f, t);
            missionText.color = color;
            yield return null;
        }

        ClearMissionText();
        fadeRoutine = null;
    }

    private void ClearMissionText()
    {
        if (missionText == null)
        {
            return;
        }

        missionText.text = string.Empty;
        missionText.gameObject.SetActive(false);
    }

    private void CacheEvidenceCounts()
    {
        if (evidenceCounterManager == null)
        {
            return;
        }

        lastGunCount = evidenceCounterManager.GunCount;
        lastGloveCount = evidenceCounterManager.GloveCount;
        lastBagCount = evidenceCounterManager.BagCount;
        lastKnifeCount = evidenceCounterManager.KnifeCount;
        lastCorpseCount = evidenceCounterManager.CorpseCount;
    }

    private int GetPreviousCount(EvidenceType type)
    {
        switch (type)
        {
            case EvidenceType.Gun:
                return lastGunCount;
            case EvidenceType.Glove:
                return lastGloveCount;
            case EvidenceType.Bag:
                return lastBagCount;
            case EvidenceType.Knife:
                return lastKnifeCount;
            case EvidenceType.Corpse:
                return lastCorpseCount;
            default:
                return 0;
        }
    }

    private void SetPreviousCount(EvidenceType type, int count)
    {
        switch (type)
        {
            case EvidenceType.Gun:
                lastGunCount = count;
                break;
            case EvidenceType.Glove:
                lastGloveCount = count;
                break;
            case EvidenceType.Bag:
                lastBagCount = count;
                break;
            case EvidenceType.Knife:
                lastKnifeCount = count;
                break;
            case EvidenceType.Corpse:
                lastCorpseCount = count;
                break;
        }
    }
}
