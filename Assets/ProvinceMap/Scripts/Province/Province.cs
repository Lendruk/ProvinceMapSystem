using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class Province
{
    [ReadOnly]
    public string name;
    public ProvinceInfo ProvinceInfo;

	[System.NonSerialized]
	public List<Vector3> border;
	[System.NonSerialized]
	public List<Vector2> points;

    public Province(ProvinceInfo info)
    {
        this.ProvinceInfo = info;
        name = ProvinceInfo.provinceName;
    }

    void UpdatePopulation()
    {
        ProvinceInfo.UpdatePopulation();
        
    }
    public int GetID()
    {
        return ProvinceInfo.provinceID;
    }
    public bool IsTransversable()
    {
        return ProvinceInfo.isTransversable;
    }
    public string GetName()
    {
        return ProvinceInfo.provinceName;
    }

    /*
    public void ApplyWeatherEffect()
    {
        if (ProvinceInfo.weather == null)
            return;
        if (ProvinceInfo.weather.weatherEffect != null)
        {
            GameObject effect = Instantiate(ProvinceInfo.weather.weatherEffect, this.transform);
            effect.transform.position = new Vector3(effect.transform.position.x, effect.transform.position.y, effect.transform.position.z + 3);
            effect.tag = "prov" + ProvinceInfo.provinceID + "WE";
        }
    }
    public void RemoveWeatherEffect()
    {
        GameObject effect = GameObject.FindGameObjectWithTag("prov" + ProvinceInfo.provinceID + "WE");
        Destroy(effect);
        ProvinceInfo.weather = WeatherList.GetWeather(WeatherEnum.CLEAR);
    }
    */

}
