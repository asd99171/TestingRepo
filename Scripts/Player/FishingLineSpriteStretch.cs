using UnityEngine;

public sealed class FishingLineSpriteStretch : MonoBehaviour
{
    [SerializeField] private Transform fishermanAnchor;
    [SerializeField] private Transform hookAnchor;

    [SerializeField] private Vector3 fishermanOffset = new Vector3(0.1f, 0.2f, 0f);
    [SerializeField] private Vector3 hookOffset = Vector3.zero;

    [SerializeField] private SpriteRenderer lineSprite;
    [SerializeField] private float lineThicknessWorld = 0.04f;

    private float spriteHeightWorldAtScale1;

    private void Awake()
    {
        if (this.lineSprite == null)
        {
            this.lineSprite = this.GetComponent<SpriteRenderer>();
        }

        if (this.lineSprite != null && this.lineSprite.sprite != null)
        {
            spriteHeightWorldAtScale1 = this.lineSprite.sprite.bounds.size.y;
        }
    }

    private void LateUpdate()
    {
        if (this.fishermanAnchor == null || this.hookAnchor == null || this.lineSprite == null || this.lineSprite.sprite == null)
        {
            return;
        }

        Vector3 a = this.fishermanAnchor.position + this.fishermanOffset;
        Vector3 b = this.hookAnchor.position + this.hookOffset;

        Vector3 mid = (a + b) * 0.5f;
        this.transform.position = mid;

        Vector3 dir = (b - a);
        float len = dir.magnitude;
        if (len < 0.0001f)
        {
            return;
        }

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        this.transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);

        float yScale = len / spriteHeightWorldAtScale1;
        float xScale = lineThicknessWorld / this.lineSprite.sprite.bounds.size.x;

        this.transform.localScale = new Vector3(xScale, yScale, 1f);
    }
}
