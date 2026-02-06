using System;
using UnityEngine;

public class EvidenceCounterManager : MonoBehaviour
{
    public event Action<EvidenceType, int> EvidenceCountChanged;

    private int gunCount;
    private int gloveCount;
    private int bagCount;
    private int knifeCount;
    private int corpseCount;
    private bool isCountingEnabled;

    public int GunCount => gunCount;
    public int GloveCount => gloveCount;
    public int BagCount => bagCount;
    public int KnifeCount => knifeCount;
    public int CorpseCount => corpseCount;
    public bool IsCountingEnabled => isCountingEnabled;

    public void SetCountingEnabled(bool isEnabled)
    {
        isCountingEnabled = isEnabled;
    }

    public void ResetCounts()
    {
        gunCount = 0;
        gloveCount = 0;
        bagCount = 0;
        knifeCount = 0;
        corpseCount = 0;

        EvidenceCountChanged?.Invoke(EvidenceType.Gun, gunCount);
        EvidenceCountChanged?.Invoke(EvidenceType.Glove, gloveCount);
        EvidenceCountChanged?.Invoke(EvidenceType.Bag, bagCount);
        EvidenceCountChanged?.Invoke(EvidenceType.Knife, knifeCount);
        EvidenceCountChanged?.Invoke(EvidenceType.Corpse, corpseCount);
    }

    public void AddEvidence(EvidenceType type)
    {
        if (!isCountingEnabled)
        {
            return;
        }

        switch (type)
        {
            case EvidenceType.Gun:
                gunCount++;
                EvidenceCountChanged?.Invoke(EvidenceType.Gun, gunCount);
                break;
            case EvidenceType.Glove:
                gloveCount++;
                EvidenceCountChanged?.Invoke(EvidenceType.Glove, gloveCount);
                break;
            case EvidenceType.Bag:
                bagCount++;
                EvidenceCountChanged?.Invoke(EvidenceType.Bag, bagCount);
                break;
            case EvidenceType.Knife:
                knifeCount++;
                EvidenceCountChanged?.Invoke(EvidenceType.Knife, knifeCount);
                break;
            case EvidenceType.Corpse:
                corpseCount++;
                EvidenceCountChanged?.Invoke(EvidenceType.Corpse, corpseCount);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }
}