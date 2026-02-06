using UnityEngine;

public sealed class RiverDespawn : MonoBehaviour
{
    private RiverSpawner owner;
    private float despawnX;

    public void Init(RiverSpawner spawner, float despawnLineX)
    {
        this.owner = spawner;
        this.despawnX = despawnLineX;
    }

    private void Update()
    {
        if (this.transform.position.x <= this.despawnX)
        {
            this.owner?.NotifyDespawned(this);
            Destroy(this.gameObject);
        }
    }
}
