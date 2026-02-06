using System;
using UnityEngine;

public class CatchCounterManager : MonoBehaviour
{
    public event Action<CatchType, int> CatchCountChanged;

    private int fishCount;
    private int evidenceCount;
    private int corpseCount;
    private bool isCountingEnabled;

    public int FishCount => fishCount;
    public int EvidenceCount => evidenceCount;
    public int CorpseCount => corpseCount;
    public bool IsCountingEnabled => isCountingEnabled;

    public void SetCountingEnabled(bool isEnabled)
    {
        isCountingEnabled = isEnabled;
    }

    public void ResetCounts()
    {
        fishCount = 0;
        evidenceCount = 0;
        corpseCount = 0;

        CatchCountChanged?.Invoke(CatchType.Fish, fishCount);
        CatchCountChanged?.Invoke(CatchType.Evidence, evidenceCount);
        CatchCountChanged?.Invoke(CatchType.Corpse, corpseCount);
    }

    public void AddCatch(CatchType type)
    {
        if (!isCountingEnabled)
        {
            return;
        }

        switch (type)
        {
            case CatchType.Fish:
                fishCount++;
                CatchCountChanged?.Invoke(CatchType.Fish, fishCount);
                break;
            case CatchType.Evidence:
                evidenceCount++;
                CatchCountChanged?.Invoke(CatchType.Evidence, evidenceCount);
                break;
            case CatchType.Corpse:
                corpseCount++;
                CatchCountChanged?.Invoke(CatchType.Corpse, corpseCount);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }
}
