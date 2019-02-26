using UnityEngine;

public class PerlinNoise : MonoBehaviour
{
    public int width = 256;
    public int height = 256;
    public float scale = 20f;

    public float offSetX = 1f;
    public float offSetY = 1f;

    private Renderer renderer;
    

    private void Start()
    {
        renderer = GetComponent<Renderer>();
        renderer.sharedMaterial.mainTexture = GenerateTexture(GenerateNoise());
        renderer.transform.localScale = new Vector3(width, height, 1);
    }
    private void Update()
    {
       // renderer.material.mainTexture = GenerateTexture();
    }

    float[,] GenerateNoise()
    {
        float[,] noiseMap = new float[width, height];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                float xCoord = (float)i / width * scale + offSetX;
                float yCoord = (float)j / height * scale + offSetY;

                float sample = Mathf.PerlinNoise(xCoord, yCoord);
                sample += 0.5f * Mathf.PerlinNoise(2 * xCoord, 2 * yCoord);
                sample += 0.25f * Mathf.PerlinNoise(4 * xCoord, 4 * yCoord);

                noiseMap[i, j] = sample;
            }
        }
        return noiseMap;
    }

    Texture2D GenerateTexture(float[,] noiseMap)
    {
        Texture2D texture = new Texture2D(width, height);

        //Generate perlin noise map
        Color[] colourMap = new Color[width * height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                colourMap[y * width + x] = Color.Lerp(Color.black, Color.white, noiseMap[x, y]);
                //Color color = CalculateColour(x, y);
                //texture.SetPixel(x, y, color);
            }
        }
        texture.SetPixels(colourMap);
        texture.Apply();
        return texture;
    }

}
