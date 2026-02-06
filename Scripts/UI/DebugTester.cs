using UnityEngine;

public class DebugTester : MonoBehaviour
{
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private EvidenceCounterManager evidenceCounterManager;

    private const int FishPoints = 1;
    private const int FishCorpsePoints = 10;

    public void AddFish()
    {
        scoreManager?.AddScore(FishPoints);
    }

    public void AddGunEvidence()
    {
        scoreManager?.AddScore(FishPoints);
        evidenceCounterManager?.AddEvidence(EvidenceType.Gun);
    }

    public void AddGloveEvidence()
    {
        scoreManager?.AddScore(FishPoints);
        evidenceCounterManager?.AddEvidence(EvidenceType.Glove);
    }

    public void AddBagEvidence()
    {
        scoreManager?.AddScore(FishPoints);
        evidenceCounterManager?.AddEvidence(EvidenceType.Bag);
    }

    public void AddKnifeEvidence()
    {
        scoreManager?.AddScore(FishPoints);
        evidenceCounterManager?.AddEvidence(EvidenceType.Knife);
    }

    public void AddFishCorpse()
    {
        scoreManager?.AddScore(FishCorpsePoints);
        evidenceCounterManager?.AddEvidence(EvidenceType.Corpse);
    }

    public void ResetCombo()
    {
        scoreManager?.ResetCombo();
    }
}