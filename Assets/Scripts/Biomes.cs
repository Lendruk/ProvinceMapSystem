using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Biomes {

    public static List<Biome> biomes = new List<Biome>
    {
        new Biome
        {
            biomeColour = Color.black,
            biomeName = "Error Biome"
        },
        new Biome
        {
            biomeColour = Color.green,
            biomeName = "Grasslands"
        },
        new Biome
        {
            biomeColour = Color.blue,
            biomeName = "Ocean"
        }
    };
    public static Biome GetBiome(string name)
    {
        foreach(Biome b in Biomes.biomes)
        {
            if (b.biomeName == name)
                return b;
        }
    return Biomes.biomes[0];
    }
}
