using UnityEngine;

public class DebugTester : MonoBehaviour
{
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private EvidenceCounterManager evidenceCounterManager;

    private const int FishPoints = 1;
    private const int FishCorpsePoints = 10;

    public void AddFish()
    {
        scoreManager?.AddScore(FishPoints, WeightClass.Light);
    }

    public void AddGunEvidence()
    {
        scoreManager?.AddScore(FishPoints, WeightClass.Light);
        evidenceCounterManager?.AddEvidence(EvidenceType.Gun);
    }

    public void AddGloveEvidence()
    {
        scoreManager?.AddScore(FishPoints, WeightClass.Light);
        evidenceCounterManager?.AddEvidence(EvidenceType.Glove);
    }

    public void AddBagEvidence()
    {
        scoreManager?.AddScore(FishPoints, WeightClass.Light);
        evidenceCounterManager?.AddEvidence(EvidenceType.Bag);
    }

    public void AddKnifeEvidence()
    {
        scoreManager?.AddScore(FishPoints, WeightClass.Light);
        evidenceCounterManager?.AddEvidence(EvidenceType.Knife);
    }

    public void AddFishCorpse()
    {
        scoreManager?.AddScore(FishCorpsePoints, WeightClass.Heavy);
        evidenceCounterManager?.AddEvidence(EvidenceType.Corpse);
    }

    public void ResetCombo()
    {
        scoreManager?.ResetCombo();
    }
}
