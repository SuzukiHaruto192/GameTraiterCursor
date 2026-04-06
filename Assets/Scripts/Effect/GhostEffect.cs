using UnityEngine;

public class GhostEffect : MonoBehaviour
{
    private SpriteRenderer sr;
    private Color color;
    [SerializeField] private float fadeSpeed = 3f;
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        color = sr.color;
    }

    void Update()
    {
        color.a -= fadeSpeed * Time.deltaTime;
        sr.color = color;

        if (color.a <= 0)
            Destroy(gameObject);
    }

    public void SetGhost(Sprite currentSprite, bool flipped, Vector3 scale)
    {
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = currentSprite;
        sr.flipX = flipped;
        transform.localScale = scale;
    }
}
