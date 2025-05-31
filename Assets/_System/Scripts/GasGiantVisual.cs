using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GasGiantVisual : MonoBehaviour
{
    public int stripeCount;
    public int blurIndex;
    public float colorSimilarityIndex;
    public float seed;
    public float speedMultiplier;
    public float fragmentZoom;
    public float brightness;
    public Color baseColor;

    MeshRenderer meshRenderer;

    int width = 512;
    int height = 512;

    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        Material material = meshRenderer.sharedMaterial;
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        Texture2D textureSpeed = new Texture2D(width, height, TextureFormat.RGBA32, false);
        Color[] colors = new Color[stripeCount];
        Color[] speed = new Color[stripeCount];

        if (baseColor.r == 0 && baseColor.g == 0 && baseColor.b == 0)
        {
            baseColor = new Color(Random.value, Random.value, Random.value);
        }

        for (int i = 0; i < stripeCount; i++)
        {
            speed[i] = new Color(Random.value, Random.value, Random.value);
            colors[i] = new Color(baseColor.r + (Random.value - 0.5f) * colorSimilarityIndex, baseColor.g + (Random.value - 0.5f) * colorSimilarityIndex, baseColor.b + (Random.value - 0.5f) * colorSimilarityIndex);
        }

        //Setting Stripes
        int stripeHeight = height / stripeCount;

        Color blur;
        float neighborIndex;
        float currentIndex;
        int a;

        //Bottom One
        for (int y = 0; y < stripeHeight - blurIndex; y++)
        {
            for (int x = 0; x < width; x++)
            {
                texture.SetPixel(x, y, colors[0]);
                textureSpeed.SetPixel(x, y, speed[0]);
            }
        }

        for (int y = stripeHeight - blurIndex; y < stripeHeight; y++)
        {
            // - Top Blur
            a = y - stripeHeight + blurIndex + 1;
            currentIndex = (float)(blurIndex + 1 - a) / (blurIndex + 1) * 0.5f + 0.5f;
            neighborIndex = (float)a / (blurIndex + 1) * 0.5f;
            blur = colors[1] * neighborIndex + colors[0] * currentIndex;
            for (int x = 0; x < width; x++)
            {
                texture.SetPixel(x, y, blur);
                textureSpeed.SetPixel(x, y, speed[0]);
            }
        }

        //Middle
        for (int i = 1; i < stripeCount - 1; i++)
        {
            for (int y = 0; y < blurIndex; y++)
            {
                // - Bottom Blur
                currentIndex = (float)(y + 1) / (blurIndex + 1) * 0.5f + 0.5f;
                neighborIndex = (float)(blurIndex - y) / (blurIndex + 1) * 0.5f;
                blur = colors[i - 1] * neighborIndex + colors[i] * currentIndex;
                for (int x = 0; x < width; x++)
                {
                    texture.SetPixel(x, i * stripeHeight + y, blur);
                    textureSpeed.SetPixel(x, i * stripeHeight + y, speed[i]);
                }
            }

            for (int y = blurIndex; y < stripeHeight - blurIndex; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    texture.SetPixel(x, i * stripeHeight + y, colors[i]);
                    textureSpeed.SetPixel(x, i * stripeHeight + y, speed[i]);
                }
            }

            for (int y = stripeHeight - blurIndex; y < stripeHeight; y++)
            {
                // - Top Blur
                a = y - stripeHeight + blurIndex + 1;
                currentIndex = (float)(blurIndex + 1 - a) / (blurIndex + 1) * 0.5f + 0.5f;
                neighborIndex = (float)a / (blurIndex + 1) * 0.5f;
                blur = colors[i + 1] * neighborIndex + colors[i] * currentIndex;
                for (int x = 0; x < width; x++)
                {
                    texture.SetPixel(x, i * stripeHeight + y, blur);
                    textureSpeed.SetPixel(x, i * stripeHeight + y, speed[i]);
                }
            }
        }

        //Top
        for (int y = 0; y < blurIndex; y++)
        {
            // - Bottom Blur
            currentIndex = (float)(y + 1) / (blurIndex + 1) * 0.5f + 0.5f;
            neighborIndex = (float)(blurIndex - y) / (blurIndex + 1) * 0.5f;
            blur = colors[stripeCount - 2] * neighborIndex + colors[stripeCount - 1] * currentIndex;
            for (int x = 0; x < width; x++)
            {
                texture.SetPixel(x, y + (stripeCount - 1) * stripeHeight, blur);
                textureSpeed.SetPixel(x, y + (stripeCount - 1) * stripeHeight, speed[stripeCount - 1]);
            }
        }

        for (int y = blurIndex; y < stripeHeight; y++)
        {
            for (int x = 0; x < width; x++)
            {
                texture.SetPixel(x, y + (stripeCount - 1) * stripeHeight, colors[stripeCount - 1]);
                textureSpeed.SetPixel(x, y + (stripeCount - 1) * stripeHeight, speed[stripeCount - 1]);
            }
        }

        texture.Apply();
        textureSpeed.Apply();

        material.SetFloat("_Stripes", stripeCount);
        material.SetFloat("_Seed", seed);
        material.SetFloat("_Speed", speedMultiplier);
        material.SetFloat("_Zoom", fragmentZoom);
        material.SetFloat("_Brightness", brightness);
        material.SetTexture("_Texture", texture);
        material.SetTexture("_TextureSpeed", textureSpeed);
        meshRenderer.sharedMaterial = material;
    }
}
