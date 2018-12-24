using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProvinceController : MonoBehaviour {
    public static ProvinceController instance;
    public Transform provBorders;

    public List<Province> provinces = new List<Province>();

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }
    public void AddProvince(Province provinceObject)
    {
        provinces.Add(provinceObject);

    }
    public Province GetProvince(int id)
    {
        for (int i = 0; i < provinces.Count; i++)
        {
            if (provinces[i].GetID() == id)
            {
                return provinces[i];
            }
        }
        Debug.LogError("Invalid id sent");
        return null;
    }
    public Province GetProvinceByColour(Color clr)
    {
        for (int i = 0; i < provinces.Count; i++)
        {
            if (provinces[i].ProvinceInfo.provinceColour == clr)
                return provinces[i];
        }
        return null;
    }

    public ProvinceInfo[] getInfo()
    {
        ProvinceInfo[] info = new ProvinceInfo[provinces.Count];
        for (int i = 0; i < provinces.Count; i++)
        {
            info[i] = provinces[i].ProvinceInfo;
        }
        return info;
    }
    public void DeleteBorders()
    {
       foreach(Transform child in provBorders.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void BuildProvinceBorder(Vector3[] points, Color borderColour)
    {
        GameObject go = new GameObject("Border");
        go.transform.SetParent(provBorders);
        LineRenderer line = go.AddComponent<LineRenderer>();
        line.material = new Material(Shader.Find("Diffuse"));
        line.startColor = borderColour;
        line.endColor = borderColour;
        line.startWidth = 1;
        line.endWidth = 1;
        line.positionCount = points.Length;
        line.loop = true;
        for (int i = 0; i < points.Length; i++)
        {

            //float y = terrain.SampleHeight(new Vector3(points[i].x, 0, points[i].y)) + terrain.GetPosition().y;
            //if (y < Mathf.Abs(waterLevel))
            //    y = Mathf.Abs(waterLevel);
            line.SetPosition(i, new Vector3(points[i].x, 0, points[i].y));
        }

    }
}
