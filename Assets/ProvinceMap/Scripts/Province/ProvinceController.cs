using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProvinceController : MonoBehaviour {
    public static ProvinceController instance;

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
            if (provinces[i].provinceInfo.provinceColour == clr)
                return provinces[i];
        }
        return null;
    }

    public ProvinceInfo[] getInfo()
    {
        ProvinceInfo[] info = new ProvinceInfo[provinces.Count];
        for (int i = 0; i < provinces.Count; i++)
        {
            info[i] = provinces[i].provinceInfo;
        }
        return info;
    }
}
