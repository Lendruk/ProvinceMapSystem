using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCell{


    public int ID { get; set; }

    public GameObject voronoiCell;

    public Biome biome;

    public Vector2 position { get; set; }


    public void SetBiome(Biome biome)
    {
        this.biome = biome;
        voronoiCell.GetComponent<MeshRenderer>().material.color = biome.biomeColour;
    }


}
