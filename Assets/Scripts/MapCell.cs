using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCell{


    public int ID { get; set; }

    public GameObject voronoiCell;

    public Biome biome;

    public List<MapEdge> edges;

    public Vector2 Position { get; set; }

    public List<Vector2> neighbours;

    public bool isBorder;

    public void SetBiome(Biome biome)
    {
        this.biome = biome;
        voronoiCell.GetComponent<MeshRenderer>().material.color = biome.biomeColour;
    }


}
