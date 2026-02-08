using System;
using UnityEngine;

    public class ScoreManager : MonoBehaviour
    {
        [SerializeField] private float lightComboIncreaseFactor = 1.15f;
        [SerializeField] private float mediumComboIncreaseFactor = 1.25f;
        [SerializeField] private float heavyComboIncreaseFactor = 1.33f;
        [SerializeField] private float anomalyComboIncreaseFactor = 1.75f;
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

        public void AddScore(int basePoints, WeightClass weightClass = WeightClass.Light)
        {
            if (!IsScoringEnabled || basePoints == 0)
            {
                return;
            }

            if (basePoints < 0)
            {
                Score += basePoints;
                ScoreChanged?.Invoke(Score);
                ResetCombo();
                return;
            }

            int addedPoints = Mathf.RoundToInt(basePoints * ComboMultiplier);
            Score += addedPoints;
            ScoreChanged?.Invoke(Score);

            float comboIncreaseFactor = this.GetComboIncreaseFactor(weightClass);
            ComboMultiplier = Mathf.Min(ComboMultiplier * comboIncreaseFactor, maxComboMultiplier);
            ComboChanged?.Invoke(ComboMultiplier);
        }

        public void ResetCombo()
        {
            ComboMultiplier = 1f;
            ComboChanged?.Invoke(ComboMultiplier);
        }

        public void MultiplyScore(float multiplier)
        {
            if (!IsScoringEnabled)
            {
                return;
            }

            Score = Mathf.RoundToInt(Score * multiplier);
            ScoreChanged?.Invoke(Score);
        }

        private float GetComboIncreaseFactor(WeightClass weightClass)
        {
            switch (weightClass)
            {
                case WeightClass.Medium:
                    return this.mediumComboIncreaseFactor;
                case WeightClass.Heavy:
                    return this.heavyComboIncreaseFactor;
                case WeightClass.Anomaly:
                    return this.anomalyComboIncreaseFactor;
                case WeightClass.Light:
                default:
                    return this.lightComboIncreaseFactor;
            }
        }
    }
