using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CountryInfo{

    public int countryID;

    public string countryName;

    public Color countryColour;

    public List<Province> provinces;

    public List<CultureEnum> acceptedCultures;

    public ReligionEnum stateReligion;


    
}
