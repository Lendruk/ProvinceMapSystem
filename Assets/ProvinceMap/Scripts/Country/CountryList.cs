using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CountryList : MonoBehaviour {
    public static CountryList instance;

    
    public List<CountryInfo> countries;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public CountryInfo GetCountry(int id)
    {
        foreach(CountryInfo ci in countries)
        {
            if (ci.countryID == id)
                return ci;
        }
        return null;
    }

}
