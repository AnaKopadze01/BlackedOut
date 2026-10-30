
using System.Collections.Generic;
using UnityEngine;

public class FootprintTrail : MonoBehaviour
{
    [Header("Footprint Settings")]
    public float stepDistance = 0.55f;
    public float footSpacing = 0.18f;
    public float footprintLifetime = 2.5f;
    public float footprintSize = 0.32f;

    private Vector2 lastPosition;
    private Vector2 lastStepPosition;
    private bool leftFoot = true;
    private Sprite footprintSprite;

    private class Footprint
    {
        public SpriteRenderer renderer;
        public float age;
    }

    private readonly List<Footprint> footprints =
        new List<Footprint>();

    void Start()
    {
        lastPosition = transform.position;
        lastStepPosition = transform.position;
        footprintSprite = CreateFootprintSprite();
    }

    void Update()
    {
        Vector2 currentPosition = transform.position;
        Vector2 movement = currentPosition - lastPosition;

        if (movement.sqrMagnitude > 0.000001f)
        {
            Vector2 direction = movement.normalized;

            if (Vector2.Distance(
                currentPosition, lastStepPosition
            ) >= stepDistance)
            {
                SpawnFootprint(currentPosition, direction);
                lastStepPosition = currentPosition;
            }
        }

        lastPosition = currentPosition;

        // Fade and remove old footprints.
        for (int i = footprints.Count - 1; i >= 0; i--)
        {
            Footprint footprint = footprints[i];

            footprint.age += Time.deltaTime;

            float alpha = 1f -
                footprint.age / footprintLifetime;

            if (alpha <= 0f)
            {
                Destroy(footprint.renderer.gameObject);
                footprints.RemoveAt(i);
                continue;
            }

            footprint.renderer.color =
                new Color(1f, 1f, 1f, alpha);
        }
    }

    void SpawnFootprint(Vector2 position, Vector2 direction)
    {
        Vector2 sideways =
            new Vector2(-direction.y, direction.x);

        float side = leftFoot ? -1f : 1f;

        Vector2 footPosition =
            position
            - direction * 0.15f
            + sideways * footSpacing * side;

        GameObject foot = new GameObject("GlowingFootprint");

        foot.transform.position = footPosition;

        float angle =
            Mathf.Atan2(direction.y, direction.x)
            * Mathf.Rad2Deg - 90f;

        foot.transform.rotation =
            Quaternion.Euler(0f, 0f, angle);

        foot.transform.localScale =
            Vector3.one * footprintSize;

        SpriteRenderer sr = foot.AddComponent<SpriteRenderer>();

        sr.sprite = footprintSprite;
        sr.color = Color.white;
        sr.sortingOrder = 6;

        // Unlit so the footprints glow in darkness.
        sr.material = new Material(
            Shader.Find("Sprites/Default")
        );

        footprints.Add(new Footprint
        {
            renderer = sr,
            age = 0f
        });

        leftFoot = !leftFoot;
    }

    Sprite CreateFootprintSprite()
    {
        int width = 32;
        int height = 48;

        Texture2D texture = new Texture2D(
            width, height, TextureFormat.RGBA32, false
        );

        texture.filterMode = FilterMode.Bilinear;
        texture.wrapMode = TextureWrapMode.Clamp;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float px = (x - 16f) / 16f;
                float py = (y - 24f) / 24f;

                // Heel and front of a shoe print.
                float heel =
                    Mathf.Pow(px / 0.48f, 2f) +
                    Mathf.Pow((py + 0.52f) / 0.30f, 2f);

                float toe =
                    Mathf.Pow(px / 0.63f, 2f) +
                    Mathf.Pow((py - 0.38f) / 0.45f, 2f);

                bool visible = heel < 1f || toe < 1f;

                texture.SetPixel(
                    x, y,
                    visible ? Color.white : Color.clear
                );
            }
        }

        texture.Apply();

        return Sprite.Create(
            texture,
            new Rect(0, 0, width, height),
            new Vector2(0.5f, 0.5f),
            32f
        );
    }
}
