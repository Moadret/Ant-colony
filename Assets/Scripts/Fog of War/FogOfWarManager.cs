using UnityEngine;

public class FogOfWarManager : MonoBehaviour
{
    public static FogOfWarManager Instance;

    [Header("Fog Settings")]
    [SerializeField] int textureSize = 512;
    [SerializeField] float worldSize = 100f;
    [SerializeField] Material fogMaterial;

    public Texture2D FogTexture => fogTexture;

    Texture2D fogTexture;
    Color[] pixels;

    void Awake()
    {
        Instance = this;
        CreateFogTexture();
        fogMaterial.SetTexture("_FogTex", fogTexture);
    }

    void CreateFogTexture()
    {
        fogTexture = new Texture2D(
            textureSize,
            textureSize,
            TextureFormat.R8,   // single grayscale channel
            false               
        );

        fogTexture.filterMode = FilterMode.Bilinear;
        fogTexture.wrapMode = TextureWrapMode.Clamp;

        pixels = new Color[textureSize * textureSize];

        for (int i = 0; i < pixels.Length; i++)
            pixels[i] = Color.black;

        fogTexture.SetPixels(pixels);
        fogTexture.Apply();
    }

    Vector2Int WorldToFogCoord(Vector3 worldPos)
    {
        float x = (worldPos.x / worldSize + 0.5f) * textureSize;
        float y = (worldPos.y / worldSize + 0.5f) * textureSize;

        return new Vector2Int(
            Mathf.Clamp((int)x, 0, textureSize - 1),
            Mathf.Clamp((int)y, 0, textureSize - 1)
        );
    }

    public void RevealArea(Vector3 position, float radius)
    {
        Vector2Int center = WorldToFogCoord(position);
        int radiusPixels = Mathf.RoundToInt(radius / worldSize * textureSize);

        for (int y = -radiusPixels; y <= radiusPixels; y++)
        {
            for (int x = -radiusPixels; x <= radiusPixels; x++)
            {
                int px = center.x + x;
                int py = center.y + y;

                if (px < 0 || py < 0 || px >= textureSize || py >= textureSize)
                    continue;

                int index = py * textureSize + px;

                float dist = Mathf.Sqrt(x * x + y * y);
                if (dist > radiusPixels)
                    continue;

                pixels[index].r = 1.0f;


                /*   smooth edge version (optional)
                float dist = Mathf.Sqrt(x * x + y * y);
                if (dist > radiusPixels)
                    continue;

                float normalized = dist / radiusPixels;

                // Smooth edge (0 at edge, 1 at center)
                float strength = Mathf.SmoothStep(1f, 0f, normalized);

                // Keep the highest visibility ever achieved
                pixels[index].r = Mathf.Max(pixels[index].r, strength);
                */

            }
        }

        fogTexture.SetPixels(pixels);
        fogTexture.Apply();
    }

    void LateUpdate()
    {
        for (int i = 0; i < pixels.Length; i++)
        {
            if (pixels[i].r > 0.5f)
                pixels[i].r = Mathf.Max(0.5f, pixels[i].r - Time.deltaTime);
        }

        fogTexture.SetPixels(pixels);
        fogTexture.Apply();
    }



}
