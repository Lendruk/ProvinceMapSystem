using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Province : MonoBehaviour
{
    public ProvinceInfo ProvinceInfo;

    [ReadOnly]
    public Vector3 provinceCenter;

	[System.NonSerialized]
	public List<Vector3> border;
	[System.NonSerialized]
	public List<Vector2> points;


   // public ProvinceInfo.TerrainType terrainType;


    // Use this for initialization
    void Start () {
        provinceCenter = ProvinceInfo.center;
        
    }

	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnMouseDown()
    {
        GameObject ui = GameObject.FindGameObjectWithTag("ProvinceUICtrl");
        ProvinceUIController uictrl = ui.GetComponent<ProvinceUIController>();
        uictrl.ActivateUI();
        uictrl.GiveInformation(this);
        
    }

    private void OnValidate()
    {
        
        TextMeshPro textM = gameObject.GetComponentInChildren<TextMeshPro>();
        if(textM != null){
            textM.text = ProvinceInfo.provinceName;
        }
        if(ProvinceInfo == null)
            return;
        
        
        
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

    public void InitializeProvince()
    {
        ProvinceInfo.provinceName = "Default";
        Text text = GetComponentInChildren<Text>();
        ProvinceInfo = new ProvinceInfo();
        text.fontSize = 16;
        text.text = ProvinceInfo.provinceName;
        text.transform.position = new Vector3(text.gameObject.transform.position.x, text.gameObject.transform.position.y, -3);

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
