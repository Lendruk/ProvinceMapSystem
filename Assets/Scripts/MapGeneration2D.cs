using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UDelaunay;

public class MapGeneration2D : MonoBehaviour
{

    public int mapWidth;
    public int mapHeight;
    public int relaxations;
    public int pointCount;
    public int seed;
    [Range(0,1)]
    public float waterThreshold;
    public Transform parent;
    public GameObject plane;

    private List<Vector2> map_points;
    private List<LineSegment> map_edges;
    private List<uint> colours;
    private List<MapCell> cells;


    private Voronoi voronoi;
    private Texture2D texture;

    public Color borderColour;

    public static int NOISE_SCALE = 5;
    public static int OCTAVES = 4;
    public static float PERSISTANCE = .5f;
    public static int LACUNARITY = 2;


    private void Start()
    {
        plane.transform.localScale = new Vector3(mapWidth/10.0f, 1f, mapHeight/10.0f);
        plane.transform.position = new Vector3(mapWidth / 2f, 0, mapHeight / 2f);
        GenerateMap();
    }

    void GenerateMap()
    {
        RandomizePoints(seed);
        GenerateDiagram();
        Draw();
    }
    void GenerateDiagram()
    {
        
        voronoi = new Voronoi(map_points, new Rect(0, 0, mapWidth, mapHeight), relaxations);
        
        map_edges = voronoi.VoronoiDiagram();
        if (relaxations > 0)
        {
            map_points = voronoi.SiteCoordinates();
            
        }
            

        //map_spanningTree = voronoi.SpanningTree(KruskalType.MINIMUM);

        //map_delaunayTriangulation = voronoi.DelaunayTriangulation();
    }
    void Draw()
    {
        texture = new Texture2D(mapWidth, mapHeight);
        cells = new List<MapCell>(); 
        Debug.Log(map_points.Count);
        for (int i = 0; i < map_points.Count; i++)
        {
            Vector2 site = map_points[i];
            MapCell cell = new MapCell
            {
                Position = site,
                edges = new List<MapEdge>(),
                neighbours = voronoi.NeighborSitesForSite(site),
                ID = i,      
            };
            //cell.isBorder = IsBorder(voronoi.VoronoiBoundaryForSite(site));
            cell.isBorder = HullContainsSite(site);
            


            foreach (LineSegment segment in voronoi.VoronoiBoundaryForSite(site))
            {
                cell.edges.Add(new MapEdge(segment.P0, segment.P1));
            }
            

            // Paint edges
            for (int j = 0; j < cell.edges.Count; j++)
            {
                MapEdge edge = cell.edges[j];
                List<Vector2> points = new List<Vector2>
                {
                    edge.p0,
                    edge.p1
                };
                Vector2 p0 = points[0];
                Vector2 p1 = points[1];

                int distance = (int)Mathf.Abs(Vector2.Distance(p0, p1));
                for (int k = 0; k <=distance ; k++)
                {
                    Vector2 temp = p1 - p0;
                    temp.Normalize();
                    Vector2 pos = k * temp + p0;
                    
                    int x;
                    int y;

                    if ((int)pos.x >= mapWidth)
                        x = ((int)pos.x) - 1;
                    else
                        x = (int)pos.x;

                    if ((int)pos.y >= mapHeight)
                        y = ((int)pos.y) - 1;
                    else
                        y = (int)pos.y;

                    texture.SetPixel(x,y, borderColour);
                }
            }

            //Fill Center
            //Temporary fill to have land and water

            int tempx = (int)site.x; int tempy = (int)site.y;
            if (cell.isBorder)
            {
                cell.biome = Biomes.GetBiome("Ocean");
                texture.FloodFillBorder(tempx, tempy, cell.biome.biomeColour, borderColour);
                continue;
            }
            continue;

            float[,] noiseLayer = Noise.GenerateNoiseMap(mapWidth, mapHeight, seed, NOISE_SCALE, OCTAVES, PERSISTANCE, LACUNARITY, new Vector2(0, 0));
            
            if (noiseLayer[tempx, tempy] > waterThreshold)
            {
                cell.biome = Biomes.GetBiome("Grasslands");
                texture.FloodFillBorder(tempx, tempy, cell.biome.biomeColour, borderColour);
            }

            else
            {
                cell.biome = Biomes.GetBiome("Ocean");
                texture.FloodFillBorder(tempx, tempy, cell.biome.biomeColour, borderColour);
            }
                
        }
        texture.Apply();
        plane.GetComponent<MeshRenderer>().sharedMaterial.mainTexture = texture;
    }
    void RandomizePoints(int seed)
    {
        if (seed == 0)
            seed = Random.Range(1, int.MaxValue);
        map_points = new List<Vector2>();
        //colours = new List<uint>();

        Random.InitState(seed);
        for (int i = 0; i < pointCount; i++)
        {
           // colours.Add(0);
            map_points.Add(new Vector2(Random.Range(0, mapWidth), Random.Range(0, mapHeight)));
        }
    }
    bool HullContainsSite(Vector2 p)
    {
        foreach(Edge e in voronoi.HullEdges())
        {
            if (e.LeftSite.Coordinate == p || e.RightSite.Coordinate == p)
                return true;
        }
        return false;
    }

    bool IsBorder(List<LineSegment> edges)
    {
        Vector2[] points = new Vector2[edges.Count * 2];
        for (int i = 0; i < edges.Count; i++)
        {
            points[i] = edges[i].P0;
            points[i + 1] = edges[i].P1;
            i++;
        }
        int count = 0;
        for (int j = 0; j < points.Length; j++)
        {
            count = 0;
           
            Vector2 cur = points[j];
            for (int i = 0; i < points.Length; i++)
            {
               
                if (i == j)
                    continue;
                if (cur.x == points[i].x && cur.y == points[i].y)
                {
                    count++;
                    continue;
                }
               
            }
            if (count == 0)
                return true;     
        }
        return false;
    }

    bool ContainsSite(Vector2 site)
    {
        foreach(MapCell cell in cells)
        {
            
            if (cell.Position == site)
                return true;
        }
        return false;
    }
}
  
