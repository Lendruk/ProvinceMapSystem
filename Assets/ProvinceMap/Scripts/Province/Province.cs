using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Province : MonoBehaviour
{

    
    public ProvinceInfo provinceInfo;

    [ReadOnly]
    public Vector3 provinceCenter;

	[System.NonSerialized]
	public List<Vector3> border;
	[System.NonSerialized]
	public List<Vector2> points;

	[System.NonSerialized]
	public GameObject mesh;

    public ProvinceInfo.TerrainType terrainType;


    // Use this for initialization
    void Start () {
        provinceCenter = provinceInfo.center;
        UpdateAtributes();
    }
    public void UpdateAtributes()
    {
        terrainType = provinceInfo.terrain;
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
            textM.text = provinceInfo.provinceName;
        }
        if(provinceInfo == null)
            return;
        
        
        
    }
    void UpdatePopulation()
    {
        provinceInfo.UpdatePopulation();
        
    }
    public int GetID()
    {
        return provinceInfo.provinceID;
    }
    public bool IsTransversable()
    {
        return provinceInfo.isTransversable;
    }
    public string GetName()
    {
        return provinceInfo.provinceName;
    }

    public void InitializeProvince()
    {
        provinceInfo.provinceName = "Default";
        Text text = GetComponentInChildren<Text>();
        provinceInfo = new ProvinceInfo();
        text.fontSize = 16;
        text.text = provinceInfo.provinceName;
        text.transform.position = new Vector3(text.gameObject.transform.position.x, text.gameObject.transform.position.y, -3);

    }
    /*
    public void ApplyWeatherEffect()
    {
        if (provinceInfo.weather == null)
            return;
        if (provinceInfo.weather.weatherEffect != null)
        {
            GameObject effect = Instantiate(provinceInfo.weather.weatherEffect, this.transform);
            effect.transform.position = new Vector3(effect.transform.position.x, effect.transform.position.y, effect.transform.position.z + 3);
            effect.tag = "prov" + provinceInfo.provinceID + "WE";
        }
    }
    public void RemoveWeatherEffect()
    {
        GameObject effect = GameObject.FindGameObjectWithTag("prov" + provinceInfo.provinceID + "WE");
        Destroy(effect);
        provinceInfo.weather = WeatherList.GetWeather(WeatherEnum.CLEAR);
    }
    */

}
