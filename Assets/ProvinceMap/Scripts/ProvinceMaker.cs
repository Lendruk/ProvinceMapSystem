
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using TMPro;
using System.Linq;
using UnityEditor;
using Random = UnityEngine.Random;

public class ProvinceMaker : MonoBehaviour
{
    public static ProvinceMaker instance;

    public ImageSplitter spliter;
    public Camera cam;
    public Texture2D mapBmp;
    public Texture2D heightMap; //GreyScale Height map;
    public Transform labelHolder;
    public Terrain terrain;
    //public TerrainGrid grid;

    [Header("Map Config")]

    public Color borderColour;
    public Material provinceMaterial;
    //public float borderWidth;
    public float sphereRadius;
    public float borderYoffset = 0f;
    //public float provinceMeshOffset = 0f;
    //public float waterLevel;

    [Header("Terrain Config")]
    public int seed;
    public float mapHeight;

    [Range(0, 1)]
    public float TreeScale;
    public int MaxTreeCount;
    public int MaxClusterCount;

    public TerrainLayer[] TerrainLayers;
    public GameObject[] TreeTypes;

    [Header("Height Map Config")]

    public float octaves;
    

    void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        spliter.bmpImage = mapBmp;
        //spliter.SplitByBorderNoSeed();
        spliter.SplitByBorderOrdered();
        spliter.AddMedianPoint();
        BuildPlane();
        BuildTerrain();
        //OrderPositions();
        //BuildBasicInfo();

        BuildProvinceInfo();
        //BuildProvinceTerrain();
        PaintTrees();

        LoadCountries();
        AddProvincesToCountries();

        BuildText();

        BuildHeightmap(ProvinceController.instance.provinces[0]);

    }

    //Assigns the border points to the province
    private void AssignPoints(Province prov)
    {
        foreach (ImageCell cell in spliter.cells)
        {
            if (cell.cell_Color == prov.ProvinceInfo.provinceColour)
            {

                prov.points = cell.borderPoints.Select(e => new Vector2(e.x, e.y)).ToList();
                return;
            }
        }
    }

    public void PaintTrees()
    {
        int curTreeCount = 0;
        int curClustersAmt = 0;

        Random.InitState(seed);

        List<TreeInstance> trees = new List<TreeInstance>();
        List<Province> provinces = ProvinceController.instance.provinces;
        for (int j = 0; j < provinces.Count; j++)
        {
            int provClusters = 0;
            Province province = provinces[j];
            Debug.Log(province.ProvinceInfo.provinceName);
            AssignPoints(province);

            ProvinceTerrain provTerrain = ProvinceController.instance.GetTerrainType(province.ProvinceInfo.terrain);
            if (!provTerrain.HasTrees)
                continue;

            //Add Terrain checks for different types

            //Calculates the size for the clusters
            float x = provClusters * curClustersAmt;
            if (x == 0) x = 1;
            float clusterSize = (Random.value / x) * (province.points.Count / 4f);
            int clusterAmmount = 3;

            float minX = province.points.Min(e => e.x);
            float maxX = province.points.Max(e => e.x);
            float minY = province.points.Min(e => e.y);
            float maxY = province.points.Max(e => e.y);

            Vector2 median = province.ProvinceInfo.center;
            //GameObject.CreatePrimitive(PrimitiveType.Cube).transform.position = new Vector3(median.x, 0, median.y);
            float circleRadious = 1f;
            float[] distances = new float[4];
            float yDistanceTop = province.points.Where(e => (int)e.x == (int)median.x).Max(e => e.y);
            distances[0] = yDistanceTop;
            float yDistanceBottom = province.points.Where(e => (int)e.x == (int)median.x).Min(e => e.y);
            distances[1] = yDistanceBottom;
            float xDistanceTop = province.points.Where(e => (int)e.y == (int)median.y).Max(e => e.x);
            distances[2] = xDistanceTop;
            float xDistanceBottom = province.points.Where(e => (int)e.y == (int)median.y).Min(e => e.x);
            distances[3] = xDistanceBottom;

            circleRadious = distances.Min();
            circleRadious = circleRadious / 100f;
            //Debug.Log(circleRadious);

            for (int k = 0; k < clusterAmmount; k++)
            {
                float posX;
                float posY;

                var angle = Random.value * Mathf.PI * 2;
                var radius = Mathf.Sqrt(Random.Range(1f, 3f)) * circleRadious;
                posX = median.x + radius * Mathf.Cos(angle);
                posY = median.y + radius * Mathf.Sin(angle);


                bool flag = IntToBool(Random.Range(0, 2));
                //Debug.Log("flag: " + flag);
                posX += (flag) ? posX * Random.Range(0f, 0.1f) : -posX * Random.Range(0f, 0.1f);
                posY += (flag) ? posY * Random.Range(0f, 0.1f) : -posY * Random.Range(0f, 0.1f);

                //GameObject.CreatePrimitive(PrimitiveType.Sphere).transform.position = new Vector3(posX, 0, posY);
                //Debug.Log("posX: " + posX);
                //Debug.Log("posY: " + posY);

                for (int i = 0; i < clusterSize; i++)
                {
                    TreeInstance tree = new TreeInstance();
                    tree.prototypeIndex = provTerrain.PrototypeIndex;
                    float tempX;
                    float tempY;

                    angle = Random.value * Mathf.PI * 2;
                    radius = Mathf.Sqrt(Random.Range(1f, 3f)) * 1.5f;

                    tempX = posX + radius * Mathf.Cos(angle);
                    tempY = posY + radius * Mathf.Sin(angle);

                    tempX = tempX + tempX * Random.Range(0f, 0.05f);
                    tempY = tempY + tempY * Random.Range(0f, 0.05f);

                    //Check if tree is out of province bounds
                    if (tempX < minX || tempX > maxX) continue;
                    if (tempY < minY || tempY > maxY) continue;

                    tree.position = new Vector3(tempX / (float)mapBmp.width, 0 /*(float)terrain.terrainData.GetHeight((int)tempX, (int)tempY) / mapHeight*/, tempY / (float)mapBmp.height);

                    tree.heightScale = TreeScale;
                    tree.widthScale = TreeScale;
                    tree.color = Color.green;

                    //Attribute a random rotation
                    //tree.rotation = Random.value;
                    trees.Add(tree);
                }
                curClustersAmt++;
                provClusters++;
            }

        }

        terrain.terrainData.treeInstances = trees.ToArray();
    }

    private bool IntToBool(int num)
    {
        if (num == 1) return true;
        return false;
    }

    public void TextureTerrainMapStyle()
    {
        TerrainData terrainData = terrain.terrainData;

        float[,,] splatMap = new float[terrainData.alphamapWidth, terrainData.alphamapHeight, terrainData.alphamapLayers];

        for (int i = 0; i < terrainData.alphamapWidth; i++)
        {
            for (int j = 0; j < terrainData.alphamapHeight; j++)
            {
                // Normalise x/y coordinates to range 0-1 
                float y_01 = (float)j / (float)terrainData.alphamapHeight;
                float x_01 = (float)i / (float)terrainData.alphamapWidth;

                // Sample the height at this location (note GetHeight expects int coordinates corresponding to locations in the heightmap array)
                float height = terrainData.GetHeight(Mathf.RoundToInt(y_01 * terrainData.heightmapHeight), Mathf.RoundToInt(x_01 * terrainData.heightmapWidth));

                // Calculate the normal of the terrain (note this is in normalised coordinates relative to the overall terrain dimensions)
                Vector3 normal = terrainData.GetInterpolatedNormal(y_01, x_01);

                // Calculate the steepness of the terrain
                float steepness = terrainData.GetSteepness(y_01, x_01);

                // Setup an array to record the mix of texture weights at this point
                float[] splatWeights = new float[terrainData.alphamapLayers];

                // CHANGE THE RULES BELOW TO SET THE WEIGHTS OF EACH TEXTURE ON WHATEVER RULES YOU WANT

                // Texture[0] has constant influence
                splatWeights[0] = 0.5f;

                // Texture[1] is stronger at lower altitudes
                splatWeights[1] = Mathf.Clamp01((terrainData.heightmapHeight - height));

                // Texture[2] stronger on flatter terrain
                // Note "steepness" is unbounded, so we "normalise" it by dividing by the extent of heightmap height and scale factor
                // Subtract result from 1.0 to give greater weighting to flat surfaces
                splatWeights[2] = 1.0f - Mathf.Clamp01(steepness * steepness / (terrainData.heightmapHeight / 5.0f));

                // Texture[3] increases with height but only on surfaces facing positive Z axis 
                splatWeights[3] = height * Mathf.Clamp01(normal.z);

                // Sum of all textures weights must add to 1, so calculate normalization factor from sum of weights
                float z = splatWeights.Sum();

                // Loop through each terrain texture
                for (int x = 0; x < terrainData.alphamapLayers; x++)
                {

                    // Normalize so that sum of all texture weights = 1
                    splatWeights[x] /= z;

                    // Assign this point to the splatmap array
                    splatMap[i, j, x] = splatWeights[x];
                }
            }
        }

        // Finally assign the new splatmap to the terrainData:
        terrainData.SetAlphamaps(0, 0, splatMap);
    }

    private void BuildTerrain()
    {
        string name = "MapTerrain";
        TerrainData terrainData = new TerrainData();

        terrainData.name = name;
        terrainData.heightmapResolution = 513;
        terrainData.size = new Vector3(mapBmp.width, mapHeight, mapBmp.height);


        terrainData.terrainLayers = TerrainLayers;

        //Handle Tree Prototypes
        if (TreeTypes.Length > 0)
        {
            TreePrototype[] prototypesArr = new TreePrototype[TreeTypes.Length];
            for (int i = 0; i < TreeTypes.Length; i++)
            {
                GameObject prefab = TreeTypes[i];
                TreePrototype prototype = new TreePrototype();
                prototype.prefab = prefab;
                prototypesArr[i] = prototype;
            }

            terrainData.treePrototypes = prototypesArr;
        }

        int w = terrainData.alphamapWidth;
        int h = terrainData.alphamapHeight;

        float[,] heights = new float[w, h];

        //Height Map
        for (int i = 0; i < w; i++)
        {
            for (int j = 0; j < h; j++)
            {
                float cur = heightMap.GetPixel(i, j).grayscale;
                heights[i, j] = cur;
            }
        }
        terrainData.SetHeights(0, 0, heights);

        //Terrain GameObject initialization
        GameObject terrainObj = Terrain.CreateTerrainGameObject(terrainData);
        terrainObj.transform.position = new Vector3(0, 0, 0);
        terrainObj.name = name;

        terrain = terrainObj.GetComponent<Terrain>();
        terrain.heightmapPixelError = 0;
        terrain.treeCrossFadeLength = 200;

        AssetDatabase.CreateAsset(terrainData, "Assets/ProvinceMap/TerrainData/" + name + ".asset");

        TextureTerrainMapStyle();
    }


    public void BuildPlane()
    {
        GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        plane.layer = 11;
        Renderer renderer = plane.GetComponent<Renderer>();
        ScaleObject(renderer, mapBmp.width, mapBmp.height);
        renderer.material.mainTexture = mapBmp;

        Transform planeT = plane.transform;
        planeT.localRotation = Quaternion.Euler(new Vector3(0, -180.0f, 0));
        planeT.position = new Vector3(mapBmp.width / 2, 0.01f, mapBmp.height / 2);

        ImageRayDetector detector = plane.AddComponent<ImageRayDetector>();
        detector.cam = cam;
    }
    public void BuildBasicInfo()
    {
        string[] data = File.ReadAllLines("Assets/ProvinceMap/Resources/provBasic.txt");

        foreach (string entry in data)
        {
            //Basic info
            ProvinceInfo prov = new ProvinceInfo();
            string[] elements = entry.Split('#');
            prov.provinceID = int.Parse(elements[0]);
            prov.provinceColour = new Color32((byte)int.Parse(elements[1]), (byte)int.Parse(elements[2]), (byte)int.Parse(elements[3]), 255);


            //GameObject Creation
            GameObject go = new GameObject();
            go.name = "provinceID: " + prov.provinceID;
            //Province province = go.AddComponent<Province>();
            //province.ProvinceInfo = prov;

            //BuildBorders(province);
            //go.transform.SetParent(this.transform);
            //ProvinceController.instance.AddProvince(province);
        }
    }
    public void BuildProvinceInfo()
    {
        string jsonData = File.ReadAllText("Assets/ProvinceMap/Resources/provInfo.json");
        ProvinceInfo[] info = JsonHelper.FromJson<ProvinceInfo>(jsonData);

        foreach (ProvinceInfo i in info)
        {

            Province province = new Province(i);

            i.UpdatePopulation();



            BuildBorders(province);

            ProvinceController.instance.AddProvince(province);
        }
    }
    public void SaveProvinces()
    {
        string jsonData = JsonHelper.ToJson(ProvinceController.instance.provinces.Select(e => e.ProvinceInfo).ToArray(), true);

        File.WriteAllText("Assets/ProvinceMap/Resources/provInfo.json", jsonData);
    }

    public void AddProvincesToCountries()
    {
        string[] data = File.ReadAllLines("Assets/ProvinceMap/Resources/countryProvs.txt");

        for (int i = 0; i < data.Length; i++)
        {
            string[] elements = data[i].Split('#');
            Country ci = ProvinceController.instance.GetCountry(elements[0]);
            for (int j = 1; j < elements.Length; j++)
            {
                Province prov = ProvinceController.instance.GetProvince(int.Parse(elements[j]));
                prov.ProvinceInfo.ownerID = ci.info.countryID;
                prov.ProvinceInfo.ownerTag = ci.info.tag;
                prov.ProvinceInfo.owner = ci;

                ci.AddProvince(prov);

            }

        }
    }

    public void BuildText()
    {
        foreach (Province prov in ProvinceController.instance.provinces)
        {

            GameObject textGO = new GameObject();
            textGO.name = "ProvinceText";

            Transform tTransform = textGO.transform;
            tTransform.SetParent(labelHolder);
            tTransform.localScale = Vector3.one;
            Vector3 center = prov.ProvinceInfo.center;
            tTransform.position = new Vector3(center.x, 0.35f, center.y);
            tTransform.localRotation = Quaternion.Euler(new Vector3(90, 0, 0));

            TextMeshPro text = textGO.AddComponent<TextMeshPro>();
            text.text = prov.ProvinceInfo.provinceName;

            text.color = (prov.ProvinceInfo.owner != null) ? prov.ProvinceInfo.owner.info.countryColour : Color.white;

        }
    }

    public void BuildBorders(Province prov)
    {
        foreach (ImageCell cell in spliter.cells)
        {
            if (cell.cell_Color == prov.ProvinceInfo.provinceColour)
            {
                prov.border = cell.borderPoints;
                prov.points = cell.points;
                //BuildProvinceBorder(prov.gameObject.transform, cell.borderPoints.ToArray());
                prov.ProvinceInfo.center = cell.median_point;
                break;
            }
        }
    }

    public void LoadCountries()
    {
        string jsonData = File.ReadAllText("Assets/ProvinceMap/Resources/countries.json");
        CountryInfo[] countries = JsonHelper.FromJson<CountryInfo>(jsonData);
        ProvinceController.instance.AddCountries(countries);

    }

    public void SaveCountries()
    {
        string jsonData = JsonHelper.ToJson(ProvinceController.instance.countries.Select(e => e.info).ToArray(), true);
        File.WriteAllText("Assets/ProvinceMap/Resources/countries.json", jsonData);
        SaveOwnedProvinces();
    }

    public void SaveOwnedProvinces()
    {
        string data = "";
        foreach (Country country in ProvinceController.instance.countries)
        {
            if (country.provinces.Count != 0)
            {
                data += country.info.tag + "#";
                for (int i = 0; i < country.provinces.Count; i++)
                {
                    Province prov = country.provinces[i];
                    data += prov.ProvinceInfo.provinceID;
                    if (i != country.provinces.Count - 1)
                        data += "#";
                }
                data += "\n";
            }
        }
        Debug.Log(data);
    }

    public void ScaleObject(Renderer renderer, float width, float length)
    {

        float sizeX = renderer.bounds.size.x;
        float sizeZ = renderer.bounds.size.z;

        Vector3 rescale = renderer.transform.localScale;

        rescale.x = width * rescale.x / sizeX;
        rescale.z = length * rescale.z / sizeZ;

        renderer.transform.localScale = rescale;

    }

    /*
    public void OrderPositions()
    {
        foreach (ImageVoronoi_Cell cell in spliter.cells)
        {
            Vector3[] points = cell.points.ToArray();
            Array.Sort(points, new ClockwiseComparer(cell.median_point));
            cell.points = new List<Vector3>(points);

        }
    }
    */
    public void GenerateHeightmap() {

    }

    public void BuildHeightmap(Province prov)
    {
        Vector2 lowerBoundry = new Vector2(prov.border.Min(e => e.x), prov.border.Min(e => e.y));
        Vector2 upperBoundry = new Vector2(prov.border.Max(e => e.x), prov.border.Max(e => e.y));

        //Texture2D provHeightmap = new Texture2D((int)(upperBoundry.x - lowerBoundry.x), (int)(upperBoundry.y - lowerBoundry.y));
        //provHeightmap.filterMode = FilterMode.Point;
        //float[,] heights = new float[provHeightmap.width, provHeightmap.height];

        //for (int i = 0; i < provHeightmap.width; i++)
        //{
        //    for (int k = 0; k < provHeightmap.height; k++)
        //    {
        //        heights[i, k] = Mathf.PerlinNoise(((float)i / (float)provHeightmap.width) * 1, ((float)k / (float)provHeightmap.width) * 1) / 10.0f;
        //    }
        ////AssetDatabase.CreateAsset(provHeightmap, "Assets/ProvinceMap/TerrainData/" + prov.name + "_height.png");
        //}

        //terrain.terrainData.SetHeights(0, 0, heights);
    }

    List<Vector2> ConvertVectors(List<Vector3> vectors)
    {
        List<Vector2> v2 = new List<Vector2>();
        foreach (Vector3 v in vectors)
        {
            v2.Add(new Vector2(v.x, v.y));
        }
        return v2;
    }
}


