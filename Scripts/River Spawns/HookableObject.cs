using System.Collections;
using UnityEngine;

public enum HookableType
{
    Trash = 0,
    Wildlife = 1,
    Anomaly = 2
}

public enum WeightClass
{
    Light = 0,
    Medium = 1,
    Heavy = 2,
    Anomaly = 3
}

public sealed class HookableObject : MonoBehaviour
{
    [Header("Classification")]
    [SerializeField] private HookableType objectType = HookableType.Trash;
    [SerializeField] private WeightClass weightClass = WeightClass.Light;

    [Header("Scoring")]
    [SerializeField] private int baseScore = 100;

    [Header("Evidence")]
    [SerializeField] private bool grantsEvidence = false;
    [SerializeField] private EvidenceType evidenceType = EvidenceType.Gun;

    [Header("Hooking")]
    [SerializeField] private bool isHookable = true;

    [Header("Visuals")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float highlightScaleMultiplier = 1.15f;
    [SerializeField] private float highlightSecondsRealtime = 0.12f;

    [Header("Fail Behavior")]
    [SerializeField] private bool sinkOnFail = false;
    [SerializeField] private float sinkSpeed = 1.5f;
    [SerializeField] private float sinkLifetimeSeconds = 1.0f;

    private Vector3 originalScale;

    public HookableType ObjectType
    {
        get { return this.objectType; }
    }

    public WeightClass WeightClass
    {
        get { return this.weightClass; }
    }

    public int BaseScore
    {
        get { return this.baseScore; }
    }

    public bool GrantsEvidence
    {
        get { return this.grantsEvidence; }
    }

    public EvidenceType EvidenceType
    {
        get { return this.evidenceType; }
    }

    public bool IsHookable
    {
        get { return this.isHookable; }
    }

    private void Awake()
    {
        this.originalScale = this.transform.localScale;

        if (this.spriteRenderer == null)
        {
            this.spriteRenderer = this.GetComponentInChildren<SpriteRenderer>();
        }
    }

    public void Consume()
    {
        Destroy(this.gameObject);
    }

    public void MarkUnhookableAndFail()
    {
        this.isHookable = false;

        if (this.sinkOnFail)
        {
            this.StartCoroutine(this.SinkAndDestroyRealtime());
        }
        else
        {
            // Fast pace: just remove it.
            Destroy(this.gameObject);
        }
    }

    public void PlayHookHighlight()
    {
        this.StopAllCoroutines();
        this.StartCoroutine(this.HighlightPulseRealtime());
    }

    private IEnumerator HighlightPulseRealtime()
    {
        Vector3 up = this.originalScale * this.highlightScaleMultiplier;
        this.transform.localScale = up;

        yield return new WaitForSecondsRealtime(this.highlightSecondsRealtime);

        this.transform.localScale = this.originalScale;
    }

    private IEnumerator SinkAndDestroyRealtime()
    {
        float elapsed = 0f;

        while (elapsed < this.sinkLifetimeSeconds)
        {
            float dt = Time.unscaledDeltaTime;
            this.transform.position += Vector3.down * (this.sinkSpeed * dt);

            elapsed += dt;
            yield return null;
        }

        Destroy(this.gameObject);
    }
}
