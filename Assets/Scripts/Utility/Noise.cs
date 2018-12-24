using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise  {

    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset)
    {
        float[,] noiseMap = new float[mapWidth, mapHeight];

        Random.InitState(seed);

        Vector2[] octaveOffsets = new Vector2[octaves];
        for (int i = 0; i < octaves; i++)
        {
            float offsetX = Random.Range(-100000, 100000) + offset.x;
            float offsetY = Random.Range(-100000, 100000) + offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }
        if (scale <= 0)
            scale = 0.0001f;

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeigt = float.MaxValue;

        float halfWidth = mapWidth / 2f;
        float halfHeight = mapHeight / 2f;

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                float frequency = 1;
                float amplitude = 1;
                float noiseHeight = 0;

                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = (x - halfWidth) / scale * frequency + octaveOffsets[i].x;
                    float sampleY = (y - halfHeight) / scale * frequency + octaveOffsets[i].y;

                    float perlin = Mathf.PerlinNoise(sampleX/10f, sampleY/10f) * 2 - 1;
                    noiseHeight += perlin * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;

                }
                if (noiseHeight > maxNoiseHeight)
                {
                    maxNoiseHeight = noiseHeight;
                }
                else if (noiseHeight < minNoiseHeigt)
                {
                    minNoiseHeigt = noiseHeight;
                }

                noiseMap[x, y] = noiseHeight;
            }
        }

        // Normalising noise map 
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeigt, maxNoiseHeight, noiseMap[x, y]);
            }
        }

        return noiseMap;
    }

}
