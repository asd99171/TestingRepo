using System;
using UnityEngine;

[Serializable]
public sealed class RingQTEPreset
{
    public WeightClass weightClass;

    [Header("Radii (world units)")]
    [Tooltip("Starting radius of the shrinking white ring (world units).")]
    public float startRadius = 1.20f;

    [Tooltip("If the white ring shrinks to this radius, QTE fails automatically.")]
    public float failRadius = 0.18f;

    [Header("Success Band (Green)")]
    public float successMinRadius = 0.55f;
    public float successMaxRadius = 0.70f;

    [Header("Perfect Band (Yellow)")]
    public float perfectMinRadius = 0.60f;
    public float perfectMaxRadius = 0.64f;

    [Header("Timing")]
    [Tooltip("Seconds from startRadius down to failRadius.")]
    public float shrinkDurationSeconds = 1.2f;

    [Header("Attempts")]
    public int missesAllowed = 0;
}

public enum RingQTEResult
{
    Miss = 0,
    Success = 1,
    Perfect = 2
}

public sealed class RingQTEController : MonoBehaviour
{
    [Header("Visual Root (world space)")]
    [SerializeField] private Transform ringRoot;

    [Header("Sprites (must have SpriteRenderer)")]
    [SerializeField] private SpriteRenderer whiteRing;
    [SerializeField] private SpriteRenderer successGreenRing;
    [SerializeField] private SpriteRenderer perfectYellowRing;
    [SerializeField] private SpriteRenderer failRedDot;


    [Header("Fail Dot")]
    [SerializeField] private float failDotWorldRadius = 0.06f;

    [Header("Follow Hook")]
    [SerializeField] private bool followHook = true;
    [SerializeField] private Vector3 worldOffset = new Vector3(0.6f, 0.6f, 0f);

    [Header("Presets")]
    [SerializeField] private RingQTEPreset[] presets;

    [Header("Debug")]
    [SerializeField] private bool logResults = true;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip qteLoopClip;
    [SerializeField] private float qteLoopFadeOutSeconds = 0.2f;

    private Coroutine qteLoopFadeRoutine;

    private bool isActive;
    private Transform hookToFollow;

    private RingQTEPreset activePreset;
    private HookableObject currentTarget;

    private float previousTimeScale = 1f;
    private float elapsed;

    private float currentRadius;
    private int misses;

    public event Action<HookableObject, RingQTEResult> OnRingQTEResult;

    private void Awake()
    {
        if (this.ringRoot == null)
        {
            Debug.LogError("RingQTEController: ringRoot is not set.");
        }

        this.SetVisualsActive(false);
    }

    private void Update()
    {
        if (!this.isActive)
        {
            return;
        }

        float dt = Time.unscaledDeltaTime;
        this.elapsed += dt;

        float t;
        if (this.activePreset.shrinkDurationSeconds > 0.001f)
        {
            t = Mathf.Clamp01(this.elapsed / this.activePreset.shrinkDurationSeconds);
        }
        else
        {
            t = 1f;
        }

        this.currentRadius = Mathf.Lerp(this.activePreset.startRadius, this.activePreset.failRadius, t);

        this.SetRingRadius(this.whiteRing, this.currentRadius);

        if (this.followHook)
        {
            this.UpdateFollow();
        }

        if (t >= 1f)
        {
            this.Finish(RingQTEResult.Miss, "timeout");
        }
    }

    public void SetHookToFollow(Transform hook)
    {
        this.hookToFollow = hook;
    }

    public bool IsBusy()
    {
        return this.isActive;
    }

    public void BeginQTE(HookableObject target)
    {
        if (target == null)
        {
            return;
        }

        if (this.isActive)
        {
            return;
        }

        this.currentTarget = target;
        this.activePreset = this.GetPreset(target.WeightClass);

        this.elapsed = 0f;
        this.misses = 0;

        this.SnapChildrenToCenter();

        this.ApplyBandVisuals();

        this.currentRadius = this.activePreset.startRadius;
        this.SetRingRadius(this.whiteRing, this.currentRadius);

        target.PlayHookHighlight();

        this.PauseGame();

        this.isActive = true;
        this.SetVisualsActive(true);

        this.StartQteLoop();

        if (this.followHook)
        {
            this.UpdateFollow();
        }
    }
    
    public void Press()
    {
        if (!this.isActive)
        {
            return;
        }

        RingQTEResult res = this.Evaluate(this.currentRadius);

        if (res == RingQTEResult.Miss)
        {
            this.misses += 1;

            if (this.misses > this.activePreset.missesAllowed)
            {
                this.Finish(RingQTEResult.Miss, "miss");
            }

            return;
        }

        this.Finish(res, "hit");
    }

    private RingQTEResult Evaluate(float radius)
    {
        if (radius >= this.activePreset.perfectMinRadius && radius <= this.activePreset.perfectMaxRadius)
        {
            return RingQTEResult.Perfect;
        }

        if (radius >= this.activePreset.successMinRadius && radius <= this.activePreset.successMaxRadius)
        {
            return RingQTEResult.Success;
        }

        return RingQTEResult.Miss;
    }

    private void Finish(RingQTEResult result, string reason)
    {
        HookableObject finishedTarget = this.currentTarget;

        if (this.logResults && finishedTarget != null)
        {
            Debug.Log("REEL " + result + " (" + reason + "): " + finishedTarget.name);
        }

        this.isActive = false;

        this.SetVisualsActive(false);
        this.UnpauseGame();

        this.currentTarget = null;
        this.activePreset = null;

        if (this.OnRingQTEResult != null && finishedTarget != null)
        {
            this.OnRingQTEResult(finishedTarget, result);
        }

        this.StopQteLoop();
    }

    private void ApplyBandVisuals()
    {
        float successCenter = (this.activePreset.successMinRadius + this.activePreset.successMaxRadius) * 0.5f;
        float perfectCenter = (this.activePreset.perfectMinRadius + this.activePreset.perfectMaxRadius) * 0.5f;

        this.SetRingRadius(this.successGreenRing, successCenter);
        this.SetRingRadius(this.perfectYellowRing, perfectCenter);

        if (this.failRedDot != null && this.failRedDot.sprite != null)
        {
            this.failRedDot.transform.localPosition = Vector3.zero;
            this.failRedDot.transform.localRotation = Quaternion.identity;

            float spriteDiameter = this.failRedDot.sprite.bounds.size.x;
            if (spriteDiameter > 0.0001f)
            {
                float desiredDiameter = this.failDotWorldRadius * 2f;
                float scale = desiredDiameter / spriteDiameter;
                this.failRedDot.transform.localScale = new Vector3(scale, scale, 1f);
            }
        }


    }

    private void SetRingRadius(SpriteRenderer sr, float radiusWorld)
    {
        if (sr == null)
        {
            return;
        }

        if (sr.sprite == null)
        {
            return;
        }

        // sprite bounds are in world units at scale = 1
        float spriteDiameter = sr.sprite.bounds.size.x;
        if (spriteDiameter <= 0.0001f)
        {
            return;
        }

        float desiredDiameter = radiusWorld * 2f;
        float scale = desiredDiameter / spriteDiameter;

        sr.transform.localScale = new Vector3(scale, scale, 1f);
        sr.transform.localPosition = Vector3.zero;
        sr.transform.localRotation = Quaternion.identity;
    }

    private void SnapChildrenToCenter()
    {
        if (this.whiteRing != null) { this.whiteRing.transform.localPosition = Vector3.zero; }
        if (this.successGreenRing != null) { this.successGreenRing.transform.localPosition = Vector3.zero; }
        if (this.perfectYellowRing != null) { this.perfectYellowRing.transform.localPosition = Vector3.zero; }
    }

    private void PauseGame()
    {
        this.previousTimeScale = Time.timeScale;
        Time.timeScale = 0f;
    }

    private void UnpauseGame()
    {
        Time.timeScale = this.previousTimeScale;
    }

    private void UpdateFollow()
    {
        if (this.ringRoot == null || this.hookToFollow == null)
        {
            return;
        }

        this.ringRoot.position = this.hookToFollow.position + this.worldOffset;
    }

    private void SetVisualsActive(bool active)
    {
        if (this.ringRoot != null)
        {
            this.ringRoot.gameObject.SetActive(active);
        }
    }

    private void StartQteLoop()
    {
        if (this.qteLoopClip == null || this.audioSource == null)
        {
            return;
        }

        if (this.qteLoopFadeRoutine != null)
        {
            this.StopCoroutine(this.qteLoopFadeRoutine);
            this.qteLoopFadeRoutine = null;
        }

        this.audioSource.volume = 1f;
        this.audioSource.clip = this.qteLoopClip;
        this.audioSource.loop = true;
        this.audioSource.Play();
    }

    private void StopQteLoop()
    {
        if (this.audioSource == null)
        {
            return;
        }

        if (this.qteLoopFadeRoutine != null)
        {
            this.StopCoroutine(this.qteLoopFadeRoutine);
        }

        this.qteLoopFadeRoutine = this.StartCoroutine(this.FadeOutQteLoop());
    }

    private System.Collections.IEnumerator FadeOutQteLoop()
    {
        float startVolume = this.audioSource != null ? this.audioSource.volume : 0f;
        float duration = Mathf.Max(0.01f, this.qteLoopFadeOutSeconds);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            if (this.audioSource == null)
            {
                yield break;
            }

            float t = Mathf.Clamp01(elapsed / duration);
            this.audioSource.volume = Mathf.Lerp(startVolume, 0f, t);
            yield return null;
        }

        if (this.audioSource != null)
        {
            this.audioSource.Stop();
            this.audioSource.loop = false;
            this.audioSource.volume = startVolume;
        }

        this.qteLoopFadeRoutine = null;
    }

    private RingQTEPreset GetPreset(WeightClass wc)
    {
        if (this.presets != null)
        {
            for (int i = 0; i < this.presets.Length; i++)
            {
                if (this.presets[i] != null && this.presets[i].weightClass == wc)
                {
                    return this.presets[i];
                }
            }
        }

        RingQTEPreset fallback = new RingQTEPreset();
        fallback.weightClass = wc;
        return fallback;
    }
}
