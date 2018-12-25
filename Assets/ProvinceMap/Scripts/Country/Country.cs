using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Country {

    public CountryInfo info;

    public List<Province> provinces = new List<Province>();

    public int totalPopulation;

    public int ruralPopulation;

    public int urbanPopulation;

    public int aristocraticPopulation;


    public void AddProvince(Province province)
    {
        provinces.Add(province);
        ruralPopulation += province.ProvinceInfo.ruralPopulation;
        urbanPopulation += province.ProvinceInfo.urbanPopulation;
        aristocraticPopulation += province.ProvinceInfo.aristocraticPopulation;
        totalPopulation = 0;
        totalPopulation += ruralPopulation + urbanPopulation + aristocraticPopulation;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
