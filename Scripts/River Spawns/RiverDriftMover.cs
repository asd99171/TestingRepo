using UnityEngine;

public sealed class RiverDriftMover : MonoBehaviour
{
    [Header("Flow")]
    [SerializeField] private Vector2 baseVelocity = new Vector2(-2.0f, 0.0f);

    [Header("Variation")]
    [SerializeField] private float speedJitterPercent = 0.20f;
    [SerializeField] private float bobAmplitude = 0.08f;
    [SerializeField] private float bobFrequency = 1.2f;

    [Header("Spin")]
    [SerializeField] private bool allowSpin = true;
    [SerializeField] private float spinDegPerSec = 40f;

    [Header("Game Flow")]
    [SerializeField] private GameFlowManager gameFlowManager;

    private Vector2 velocity;
    private float bobOffset;
    private float bobPhase;
    private float spinDir;
    private Vector3 startPos;

    private void Awake()
    {
        float jitter = Random.Range(1f - this.speedJitterPercent, 1f + this.speedJitterPercent);
        this.velocity = this.baseVelocity * jitter;

        this.bobPhase = Random.Range(0f, 1000f);
        this.bobOffset = Random.Range(-1000f, 1000f);

        this.spinDir = Random.value < 0.5f ? -1f : 1f;
        this.startPos = this.transform.position;
    }

    private void Update()
    {
        if (!this.IsGameRunning())
        {
            return;
        }

        float dt = Time.deltaTime;

        Vector3 pos = this.transform.position;
        pos.x += this.velocity.x * dt;
        pos.y += this.velocity.y * dt;

        float bob = Mathf.Sin((Time.time + this.bobPhase) * this.bobFrequency) * this.bobAmplitude;
        pos.y = pos.y + bob;

        this.transform.position = pos;

        if (this.allowSpin)
        {
            this.transform.Rotate(0f, 0f, this.spinDegPerSec * this.spinDir * dt);
        }
    }

    public void SetBaseVelocity(Vector2 v)
    {
        this.baseVelocity = v;
        float jitter = Random.Range(1f - this.speedJitterPercent, 1f + this.speedJitterPercent);
        this.velocity = this.baseVelocity * jitter;
    }

    private bool IsGameRunning()
    {
        return this.gameFlowManager == null || this.gameFlowManager.CurrentState == GameState.Running;
    }
}
