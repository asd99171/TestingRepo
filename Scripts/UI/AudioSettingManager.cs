using UnityEngine;
<<<<<<< HEAD
using UnityEngine.InputSystem;
=======
>>>>>>> parent of 8c59b99 (Play and fade QTE loop audio)

public class AudioSettingsManager : MonoBehaviour
{
    [SerializeField] private float defaultVolume = 1f;
<<<<<<< HEAD
    [SerializeField] private bool requirePointerForVolumeChange = true;

    public float MasterVolume { get; private set; }
    public bool IsMuted { get; private set; }

    private float lastNonMutedVolume;

    private void Awake()
    {
        lastNonMutedVolume = Mathf.Clamp01(defaultVolume);
        ApplyMasterVolume(defaultVolume);
=======

    public float MasterVolume { get; private set; }

    private void Awake()
    {
        SetMasterVolume(defaultVolume);
>>>>>>> parent of 8c59b99 (Play and fade QTE loop audio)
    }

    public void SetMasterVolume(float volume)
    {
<<<<<<< HEAD
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
=======
        MasterVolume = Mathf.Clamp01(volume);
        AudioListener.volume = MasterVolume;
    }
}
>>>>>>> parent of 8c59b99 (Play and fade QTE loop audio)
