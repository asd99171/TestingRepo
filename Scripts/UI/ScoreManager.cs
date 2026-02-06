using System;
using UnityEngine;

    public class ScoreManager : MonoBehaviour
    {
        [SerializeField] private float comboIncreaseFactor = 1.25f;
        [SerializeField] private float maxComboMultiplier = 10f;

        public event Action<int> ScoreChanged;
        public event Action<float> ComboChanged;

        public int Score { get; private set; }
        public float ComboMultiplier { get; private set; } = 1f;
        public bool IsScoringEnabled { get; private set; }

        public void SetScoringEnabled(bool isEnabled)
        {
            IsScoringEnabled = isEnabled;
        }

        public void ResetScore()
        {
            Score = 0;
            ComboMultiplier = 1f;
            ScoreChanged?.Invoke(Score);
            ComboChanged?.Invoke(ComboMultiplier);
        }

        public void AddScore(int basePoints)
        {
            if (!IsScoringEnabled || basePoints <= 0)
            {
                return;
            }

            int addedPoints = Mathf.RoundToInt(basePoints * ComboMultiplier);
            Score += addedPoints;
            ScoreChanged?.Invoke(Score);

            ComboMultiplier = Mathf.Min(ComboMultiplier * comboIncreaseFactor, maxComboMultiplier);
            ComboChanged?.Invoke(ComboMultiplier);
        }

        public void ResetCombo()
        {
            ComboMultiplier = 1f;
            ComboChanged?.Invoke(ComboMultiplier);
        }
    }
