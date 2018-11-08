using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGeneration : MonoBehaviour {

    public VoronoiDiagram voronoi;


    [Header("Map Generation")]
    public List<MapCell> cells;
    public int seed;

    public float waterThreshold;
    public float noiseScale;

    public int octaves;

    [Range(0,1)]
    public float persistance;
    public float lacunarity;
    public Vector2 offset;

    [Range(0,1)]
    public float islandSize; // Values close to 0 = smaller islands , 1 = larger islands


	// Use this for initialization
	void Start () {
        if (seed == 0)
            seed = Random.Range(1, int.MaxValue);
        Random.InitState(seed);

       cells = voronoi.Initialize(seed);

        //GenerateLandMass();
        GenerateMap();

	}

    // Basic land mass generation
    void GenerateLandMass()
    {
        if (waterThreshold == 0)
            waterThreshold = Random.Range(0.0001f, 1);

        for (int i = 0; i < cells.Count;i++)
        {
            MapCell cell = cells[i];

            var x = cell.position.x * Random.Range(0, 9999);
            var y = cell.position.y * Random.Range(0, 9999);
            var perlin = Mathf.PerlinNoise(x,y);
            Debug.Log(perlin);
            if (perlin < waterThreshold)
                cell.SetBiome(Biomes.GetBiome("Ocean"));
            else
                cell.SetBiome(Biomes.GetBiome("Grasslands"));
        }
    }
    //
    public void GenerateMap()
    {
        float[,] noise = GenerateNoiseMap(voronoi.mapWidth, voronoi.mapHeight,seed, noiseScale,octaves,persistance,lacunarity,offset);

        for (int i = 0; i < cells.Count; i++)
        {
            MapCell cell = cells[i];
            int x = (int)cell.position.x;
            int y = (int)cell.position.y;
            float height = noise[x, y];

            if(height <waterThreshold)
                cell.SetBiome(Biomes.GetBiome("Ocean"));
            else
                cell.SetBiome(Biomes.GetBiome("Grasslands"));
        }


    }


    public static float[,] GenerateNoiseMap(int mapWidth,int mapHeight,int seed,float scale,int octaves,float persistance,float lacunarity,Vector2 offset)
    {
        float[,] noiseMap = new float[mapWidth, mapHeight];

        Random.InitState(seed);
        
        Vector2[] octaveOffsets = new Vector2[octaves];
        for (int i = 0; i < octaves; i++)
        {
            float offsetX = Random.Range(-100000, 100000) +offset.x;
            float offsetY = Random.Range(-100000, 100000) + offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);    
        }
        if(scale <= 0)
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
                    float sampleX = (x-halfWidth) / scale * frequency + octaveOffsets[i].x;
                    float sampleY = (y-halfHeight) / scale * frequency + octaveOffsets[i].y;

                    float perlin = Mathf.PerlinNoise(sampleX, sampleY)* 2 -1;
                    noiseHeight += perlin * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                    
                }
                if(noiseHeight > maxNoiseHeight)
                {
                    maxNoiseHeight = noiseHeight;
                }else if(noiseHeight < minNoiseHeigt)
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
