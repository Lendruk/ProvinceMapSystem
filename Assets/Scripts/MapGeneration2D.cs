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
                //points.Sort(new ClockwiseComparer(site));
              
                if(Mathf.Abs(points[1].x -points[0].x) < 1)
                {
                    if (points[1].y > points[0].y)
                    {
                        for (int yi = (int)points[0].y; yi < (int)points[1].y; yi++)
                        {
                            texture.SetPixel((int)points[0].x, yi, borderColour);
                        }
                        continue;
                    }
                    else
                    {
                        for (int yi = (int)points[0].y; yi > (int)points[1].y; yi--)
                        {
                            texture.SetPixel((int)points[0].x, yi, borderColour);
                        }
                        continue;
                    }

                }
                else if(Mathf.Abs(points[1].y - points[0].y )< 1)
                {
                    if (points[0].x > points[1].x)
                    {
                        for (int xi = (int)points[0].x; xi >= (int)points[1].x; xi--)
                        {
                            texture.SetPixel(xi, (int)points[0].y, borderColour);
                        }
                        continue;
                    }
                    else
                    {
                       
                            for (int xi = (int)points[0].x; xi <= (int)points[1].x; xi++)
                            {
                                texture.SetPixel(xi, (int)points[0].y, borderColour);
                            }
                            continue;
                        }
                    
                }
                  float   m = (points[1].y - points[0].y) / (points[1].x - points[0].x);
                  float  b = points[0].y - (m * points[0].x);

                if (m > 1 || m < -1)
                {
                    if (points[1].y > points[0].y)
                    {
                        for (int yi = (int)points[0].y; yi < (int)points[1].y; yi++)
                        {
                            if ((yi / m - b / m) < 0)
                                continue;
                            if ((yi / m - b / m) >= mapHeight )
                                continue;

                            texture.SetPixel((int)(yi / m - b / m), yi, borderColour);
                        }
                    }
                    else
                    {
                        for (int yi = (int)points[0].y; yi > (int)points[1].y; yi--)
                        {
                            if ((yi / m - b / m) < 0)
                                continue;
                            if ((yi / m - b / m) >= mapHeight)
                                continue;

                            texture.SetPixel((int)(yi / m - b / m), yi, borderColour);
                        }
                    }
                }
                else
                {
                    if (points[1].x > points[0].x)
                    {
                        for (int xi = (int)points[0].x; xi <= (int)points[1].x; xi++)
                        {
                            if ((m * xi) + b < 0)
                                continue;
                            if ((m * xi) + b >= mapWidth)
                                continue;

                            texture.SetPixel(xi, (int)(m * xi + b), borderColour);
                        }
                    }
                    else
                    {
                        for (int xi = (int)points[0].x; xi >= (int)points[1].x; xi--)
                        {
                            if ((m * xi) + b < 0)
                                continue;
                            if ((m * xi) + b >= mapWidth)
                                continue;

                            texture.SetPixel(xi, (int)(m * xi + b), borderColour);
                        }
                    }
                }
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
}
  
