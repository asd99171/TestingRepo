using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public sealed class SpawnEntry
{
    public GameObject prefab;

    [Tooltip("Relative chance weight (higher = more common).")]
    public float weight = 1f;
}

public sealed class RiverSpawner : MonoBehaviour
{
    private readonly List<RiverDespawn> aliveObjects = new List<RiverDespawn>();

    [Header("Spawn/Despawn Lines")]
    [SerializeField] private Transform spawnLine;
    [SerializeField] private Transform despawnLine;

    [Header("Spawn Entries")]
    [SerializeField] private SpawnEntry[] entries;

    [Header("Lanes (Y positions in world units)")]
    [SerializeField] private float[] lanesY = new float[] { -1.5f, -2.2f, -2.9f, -3.6f };

    [Header("Spawn Timing")]
    [SerializeField] private float startSpawnInterval = 1.0f;
    [SerializeField] private float minSpawnInterval = 0.35f;
    [SerializeField] private float rampSeconds = 90f;

    [Header("Flow")]
    [SerializeField] private Vector2 startFlowVelocity = new Vector2(-2.0f, 0f);
    [SerializeField] private Vector2 endFlowVelocity = new Vector2(-4.0f, 0f);

    [Header("Variation")]
    [SerializeField] private float spawnXJitter = 0.4f;
    [SerializeField] private float laneYJitter = 0.12f;

    [Header("Safety")]
    [SerializeField] private int maxAliveObjects = 50;

    [Header("Game Flow")]
    [SerializeField] private GameFlowManager gameFlowManager;

    private float elapsed;
    private float nextSpawnTimer;
    private int aliveCount;

    private void Awake()
    {
        if (this.spawnLine == null)
        {
            Debug.LogError("RiverSpawner: spawnLine is not set.");
        }

        if (this.despawnLine == null)
        {
            Debug.LogError("RiverSpawner: despawnLine is not set.");
        }

        this.nextSpawnTimer = 0.2f;
    }

    private void Update()
    {
        if (!this.IsGameRunning())
        {
            return;
        }

        if (this.spawnLine == null || this.despawnLine == null)
        {
            return;
        }

        if (this.entries == null || this.entries.Length == 0)
        {
            return;
        }

        if (this.lanesY == null || this.lanesY.Length == 0)
        {
            return;
        }

        this.elapsed += Time.deltaTime;
        this.nextSpawnTimer -= Time.deltaTime;

        if (this.nextSpawnTimer <= 0f)
        {
            if (this.aliveCount < this.maxAliveObjects)
            {
                this.SpawnOne();
            }

            float interval = this.GetCurrentSpawnInterval();
            this.nextSpawnTimer = interval;
        }
    }

    private void SpawnOne()
    {
        GameObject prefab = this.PickWeightedPrefab();
        if (prefab == null)
        {
            return;
        }

        float t = this.GetRampT01();

        Vector2 flow = Vector2.Lerp(this.startFlowVelocity, this.endFlowVelocity, t);

        float x = this.spawnLine.position.x + UnityEngine.Random.Range(-this.spawnXJitter, this.spawnXJitter);

        int laneIndex = UnityEngine.Random.Range(0, this.lanesY.Length);
        float y = this.lanesY[laneIndex] + UnityEngine.Random.Range(-this.laneYJitter, this.laneYJitter);

        Vector3 pos = new Vector3(x, y, 0f);

        GameObject obj = Instantiate(prefab, pos, Quaternion.identity);
        this.aliveCount += 1;

        RiverDriftMover mover = obj.GetComponent<RiverDriftMover>();
        if (mover != null)
        {
            mover.SetBaseVelocity(flow);
        }

        RiverDespawn despawn = obj.GetComponent<RiverDespawn>();
        if (despawn == null)
        {
            despawn = obj.AddComponent<RiverDespawn>();
        }

        despawn.Init(this, this.despawnLine.position.x);
        this.aliveObjects.Add(despawn);
    }

    private GameObject PickWeightedPrefab()
    {
        float total = 0f;

        for (int i = 0; i < this.entries.Length; i++)
        {
            if (this.entries[i] == null || this.entries[i].prefab == null)
            {
                continue;
            }

            total += Mathf.Max(0f, this.entries[i].weight);
        }

        if (total <= 0f)
        {
            return null;
        }

        float r = UnityEngine.Random.Range(0f, total);

        for (int i = 0; i < this.entries.Length; i++)
        {
            if (this.entries[i] == null || this.entries[i].prefab == null)
            {
                continue;
            }

            float w = Mathf.Max(0f, this.entries[i].weight);
            r -= w;

            if (r <= 0f)
            {
                return this.entries[i].prefab;
            }
        }

        // fallback
        for (int i = 0; i < this.entries.Length; i++)
        {
            if (this.entries[i] != null && this.entries[i].prefab != null)
            {
                return this.entries[i].prefab;
            }
        }

        return null;
    }

    private float GetCurrentSpawnInterval()
    {
        float t = this.GetRampT01();
        float interval = Mathf.Lerp(this.startSpawnInterval, this.minSpawnInterval, t);
        return interval;
    }

    private float GetRampT01()
    {
        if (this.rampSeconds <= 0.01f)
        {
            return 1f;
        }

        return Mathf.Clamp01(this.elapsed / this.rampSeconds);
    }

    // Called by RiverDespawn
    public void NotifyDespawned(RiverDespawn despawn)
    {
        if (despawn != null)
        {
            this.aliveObjects.Remove(despawn);
        }

        if (this.aliveCount > 0)
        {
            this.aliveCount -= 1;
        }
    }

    public void DespawnAllActive()
    {
        for (int i = this.aliveObjects.Count - 1; i >= 0; i--)
        {
            RiverDespawn despawn = this.aliveObjects[i];
            if (despawn != null)
            {
                Destroy(despawn.gameObject);
            }
        }

        this.aliveObjects.Clear();
        this.aliveCount = 0;
    }

    private bool IsGameRunning()
    {
        return this.gameFlowManager == null || this.gameFlowManager.CurrentState == GameState.Running;
    }
}
