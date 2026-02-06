using System;
using System.Collections.Generic;
using UnityEngine;

public class LeaderboardManager : MonoBehaviour
{
    [SerializeField] private int maxEntries = 10;

    public event Action<IReadOnlyList<int>> LeaderboardChanged;

    private readonly List<int> scores = new List<int>();

    public IReadOnlyList<int> Scores => scores;

    public void RecordScore(int score)
    {
        scores.Add(score);
        scores.Sort((a, b) => b.CompareTo(a));

        if (scores.Count > maxEntries)
        {
            scores.RemoveRange(maxEntries, scores.Count - maxEntries);
        }

        LeaderboardChanged?.Invoke(scores);
    }

    public void Clear()
    {
        scores.Clear();
        LeaderboardChanged?.Invoke(scores);
    }
}