using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProvinceUIController : MonoBehaviour {
    //public PieGraph populationPieGraph;
    //public PieGraph culturePieGraph;
    public Text populationText;
    public Text provinceName;

    //Religion
    public Text religionText;
    public Image religionImage;
    //public PieGraph religionPieGraph;

    public GameObject provUI;
    public Province currentProvince;


    public void GiveInformation(Province province)
    {
        if (province == currentProvince) return;


        ProvinceInfo pop = province.ProvinceInfo;
        float[] temp = { pop.ruralPopulation, pop.urbanPopulation, pop.aristocraticPopulation };


        currentProvince = province;

        //populationPieGraph.GiveValues( temp);
        populationText.text = "Population Stats: " +"\n"+ pop.totalPopulation + " Total\n";
        provinceName.text = currentProvince.GetName();


       //culturePieGraph.MakeCultureGraph(province.ProvinceInfo);

        religionText.text += "\n"+pop.GetMajorityReligion().religionName;

        Debug.Log("total pop"+pop.totalPopulation);
        
    }
    public void ActivateUI()
    {
        provUI.SetActive(true);
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
