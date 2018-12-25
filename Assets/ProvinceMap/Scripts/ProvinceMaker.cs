
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using TMPro;

public class ProvinceMaker : MonoBehaviour
{
    public static ProvinceMaker instance;

    public ImageSplitter spliter;
    public Camera cam;
    public Texture2D mapBmp;
    public Transform labelHolder;
    //public Terrain terrain;
	//public TerrainGrid grid;

    [Header("Map Config")]

    public Color borderColour;
	public Material provinceMaterial;
    public float borderWidth;
    public float sphereRadius;
    public float borderYoffset = 0f;
	public float provinceMeshOffset = 0f;
	public float waterLevel;
    [Range(0.0f, 2.0f)]
    public float mapScale = 1.0f;

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
        //OrderPositions();
        //BuildBasicInfo();
        BuildProvinceInfo();
        //BuildProvinceTerrain();
        LoadCountries();
        AddProvincesToCountries();

        BuildText();
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
        planeT.position = new Vector3(mapBmp.width / 2, 0, mapBmp.height / 2);

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
            Province province = go.AddComponent<Province>();
            province.ProvinceInfo = prov;

            BuildBorders(province);
            go.transform.SetParent(this.transform);
            ProvinceController.instance.AddProvince(province);
        }
    }
    public void BuildProvinceInfo()
    {
        string jsonData = File.ReadAllText("Assets/ProvinceMap/Resources/provInfo.json");
        ProvinceInfo[] info = JsonHelper.FromJson<ProvinceInfo>(jsonData);

        foreach (ProvinceInfo i in info)
        {
            GameObject go = new GameObject();
            go.name = "provinceID: " + i.provinceID;
            Province province = go.AddComponent<Province>();
            province.ProvinceInfo = i;
            i.UpdatePopulation();

            

            BuildBorders(province);
            go.transform.SetParent(this.transform);
            ProvinceController.instance.AddProvince(province);
        }
    }
    public void BuildProvinceTerrain()
    {
        string[] data = File.ReadAllLines("Assets/ProvinceMap/Resources/provTerrain.txt");

        foreach (string entry in data)
        {
            string[] elements = entry.Split('#');
            Province prov = ProvinceController.instance.GetProvince(int.Parse(elements[0]));
            prov.ProvinceInfo.terrain = (ProvinceInfo.TerrainType)Enum.Parse(typeof(ProvinceInfo.TerrainType), elements[1]);
            
        }
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
            GameObject go = prov.gameObject;
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

            text.color = (prov.ProvinceInfo.owner != null) ?  prov.ProvinceInfo.owner.info.countryColour : Color.white;  

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


