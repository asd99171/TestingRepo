using System;
using UnityEngine;

public class LeaderboardManager : MonoBehaviour
{
    public event Action<int> BestScoreChanged;

    public int BestScore { get; private set; }

    public void RecordScore(int score)
    {
        if (score > BestScore)
        {
            BestScore = score;
            BestScoreChanged?.Invoke(BestScore);
        }
    }

    public void Clear()
    {
        BestScore = 0;
        BestScoreChanged?.Invoke(BestScore);
    }
}
