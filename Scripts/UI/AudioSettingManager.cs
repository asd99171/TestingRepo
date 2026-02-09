using UnityEngine;
using UnityEngine.InputSystem;

public class AudioSettingsManager : MonoBehaviour
{
    [SerializeField] private float defaultVolume = 1f;
    [SerializeField] private bool requirePointerForVolumeChange = true;

    public float MasterVolume { get; private set; }
    public bool IsMuted { get; private set; }

    private float lastNonMutedVolume;

    private void Awake()
    {
        lastNonMutedVolume = Mathf.Clamp01(defaultVolume);
        ApplyMasterVolume(defaultVolume);
    }

    public void SetMasterVolume(float volume)
    {
        if (requirePointerForVolumeChange && !IsPointerPressed())
        {
            return;
        }

        ApplyMasterVolume(volume);
    }

    public void ToggleMute()
    {
        if (IsMuted)
        {
            float restoreVolume = lastNonMutedVolume > 0f ? lastNonMutedVolume : Mathf.Clamp01(defaultVolume);
            ApplyMasterVolume(restoreVolume);
            return;
        }

        lastNonMutedVolume = MasterVolume;
        ApplyMasterVolume(0f);
    }

    private void ApplyMasterVolume(float volume)
    {
        MasterVolume = Mathf.Clamp01(volume);
        AudioListener.volume = MasterVolume;
        if (MasterVolume > 0f)
        {
            lastNonMutedVolume = MasterVolume;
        }

        IsMuted = Mathf.Approximately(MasterVolume, 0f);
    }

    private static bool IsPointerPressed()
    {
        if (Pointer.current == null)
        {
            return false;
        }

        return Pointer.current.press.isPressed;
    }
}
