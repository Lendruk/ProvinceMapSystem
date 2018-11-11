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
    public Transform parent;
    public GameObject plane;

    private List<Vector2> map_points;
    private List<LineSegment> map_edges;
    private List<uint> colours;

    private Voronoi voronoi;
    private Texture2D texture;

    public Color borderColour;

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
            map_points = voronoi.SiteCoordinates();

        //map_spanningTree = voronoi.SpanningTree(KruskalType.MINIMUM);

        //map_delaunayTriangulation = voronoi.DelaunayTriangulation();
    }
    void Draw()
    {
        texture = new Texture2D(mapWidth, mapHeight);
      
        Debug.Log(map_points.Count);
        for (int i = 0; i < map_points.Count; i++)
        {
            Vector2 site = map_points[i];
            MapCell cell = new MapCell
            {
                position = site,
                edges = new List<MapEdge>(),
            };

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
           texture.FloodFillBorder((int)site.x, (int)site.y, Color.green, borderColour);  
        }
        texture.Apply();
        plane.GetComponent<MeshRenderer>().sharedMaterial.mainTexture = texture;
    }

    bool CompareColour(Color c1 ,Color c2)
    {
        if((int)(c1.r * 1000) == (int)(c2.r * 1000) && (int)(c1.g * 1000) == (int)(c2.g * 1000) && (int)(c1.b * 1000) == (int)(c2.b * 1000))
        {
            return true;
        }
        return false;
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
}
  
