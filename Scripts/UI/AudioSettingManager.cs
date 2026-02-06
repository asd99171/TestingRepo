using UnityEngine;

public class AudioSettingsManager : MonoBehaviour
{
    [SerializeField] private float defaultVolume = 1f;

    public float MasterVolume { get; private set; }

    private void Awake()
    {
        SetMasterVolume(defaultVolume);
    }

    public void SetMasterVolume(float volume)
    {
        MasterVolume = Mathf.Clamp01(volume);
        AudioListener.volume = MasterVolume;
    }
}