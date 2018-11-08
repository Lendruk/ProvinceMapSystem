using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UDelaunay;

public class VoronoiDiagram : MonoBehaviour {

    [SerializeField]
    private int pointCount;


    public int mapWidth;
    public int mapHeight;
    public int relaxations;

    public bool generateCenters;

    public Transform parent;

    //Voronoi Elements
    private List<Vector2> map_points;
    private List<LineSegment> map_edges;
    private List<LineSegment> map_spanningTree;
    private List<LineSegment> map_delaunayTriangulation; // To be used later.
    private List<uint> colours;

    private Voronoi voronoi;


    //Visual Elements
    private List<GameObject> spheres;
    public Material borderMaterial;
    public Material cellMaterial;

    

   
    public List<MapCell> Initialize(int seed)
    {
        RandomizePoints(seed);
        GenerateDiagram();
        return DrawDiagram();
    }

    void RandomizePoints(int seed)
    {
        if (seed == 0)
            seed = Random.Range(1,int.MaxValue);
        map_points = new List<Vector2>();
        colours = new List<uint>();
        
        Random.InitState(seed);
        for (int i = 0; i < pointCount; i++)
        {
            colours.Add(0);  
            map_points.Add(new Vector2(Random.Range(0, mapWidth), Random.Range(0, mapHeight)));
        }
    }

    void GenerateDiagram()
    {   
        voronoi = new Voronoi(map_points, new Rect(0,0,mapWidth,mapHeight),relaxations);
        
        map_edges = voronoi.VoronoiDiagram();
        if(relaxations > 0)
            map_points = voronoi.SiteCoordinates();

        //map_spanningTree = voronoi.SpanningTree(KruskalType.MINIMUM);

        map_delaunayTriangulation = voronoi.DelaunayTriangulation();
    }
    List<MapCell> DrawDiagram()
    {
        if (generateCenters)
        {
            GameObject centerHolder = new GameObject("Voronoi Site Holder");
            centerHolder.transform.SetParent(parent);
            // Centers
            for (int i = 0; i < map_points.Count; i++)
            {

                Vector2 pos = map_points[i];
                GameObject sph = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sph.name = "center (" + pos.x + "," + 0 + "," + pos.y + ")";
                sph.transform.position = new Vector3(pos.x, 0, pos.y);
                sph.transform.SetParent(centerHolder.transform);
            }
        }

        GameObject edgeHolder = new GameObject("Edge Holder");
        edgeHolder.transform.SetParent(parent);
        // Voronoi Edges
        for (int i = 0; i < map_edges.Count; i++)
        {
            Vector2 left = (Vector2)map_edges[i].P0;
            Vector2 right = (Vector2)map_edges[i].P1;
            
            GameObject edge = new GameObject("edge"+i);
            edge.transform.SetParent(edgeHolder.transform);
            LineRenderer line = edge.AddComponent<LineRenderer>();
            line.positionCount = 2;
            line.SetPositions(new Vector3[] { new Vector3(left.x,0,left.y), new Vector3(right.x,0,right.y) });
            line.startColor = Color.black;
            line.endColor = Color.black;
            line.material = borderMaterial;
        }


        //Map Borders;
        GameObject borders = new GameObject("Borders");
        borders.name = "Map Borders";
        LineRenderer lineBorder = borders.AddComponent<LineRenderer>();
        lineBorder.positionCount = 4;
        lineBorder.material = borderMaterial;
        lineBorder.SetPositions(new Vector3[] { new Vector3(0, 0, 0), new Vector3(mapWidth, 0, 0), new Vector3(mapWidth, 0, mapHeight),new Vector3(0,0,mapHeight) });
        lineBorder.loop = true;


        return DrawMeshes();
    }  
    
    bool CompareEdge(Vector2 e1,Vector2 e2)
    {
        if (e1.x > e2.x)
        {
            return true;
        }
        else if (e1.x == e2.x)
        {
            if (e1.y > e2.y)
            {
                return true;
            }
        }
        return false;
    }
    bool ContainsVertex(List<Vector2> lst,Vector2 v)
    {
        foreach(Vector2 vector in lst)
        {
            if (vector.x == v.x && vector.y == v.y)
                return true;
        }
        return false;
    }

    List<MapCell> DrawMeshes()
    {
        List<MapCell> cells = new List<MapCell>();
        GameObject cellHolder = new GameObject("CellHolder");
        cellHolder.transform.SetParent(parent);
        for (int i1 = 0; i1 < map_points.Count; i1++)
        {
            Vector2 site = map_points[i1];
            List<LineSegment> lineSegments = voronoi.VoronoiBoundaryForSite(site);
           // lineSegments = OrderEdges(lineSegments);
            List<Vector2> vertices2D = new List<Vector2>();
            //Debug.Log("Number of edges: " + lineSegments.Count);
            for (int i = 0; i < lineSegments.Count; i++)
            {
                LineSegment l = lineSegments[i];
                //Debug.Log(l.P0);
                //Debug.Log(l.P1);
                /*
                if (CompareEdge(l.P0, l.P1) && !vertices2D.Contains(l.P0))
                {
                    
                    vertices2D.Add(l.P0);
                    GameObject.CreatePrimitive(PrimitiveType.Sphere).transform.position = new Vector3(l.P0.x, 0, l.P0.y);
                }
                else if (!vertices2D.Contains(l.P1)){
                    vertices2D.Add(l.P1);
                    GameObject.CreatePrimitive(PrimitiveType.Sphere).transform.position = new Vector3(l.P1.x, 0, l.P1.y);
                }
                */
                if (!ContainsVertex(vertices2D,l.P0))
                {
                    vertices2D.Add(l.P0);
                    //GameObject.CreatePrimitive(PrimitiveType.Sphere).transform.position = new Vector3(l.P0.x, 0, l.P0.y);
                }
                    
                if (!ContainsVertex(vertices2D,l.P1))
                {
                    vertices2D.Add(l.P1);
                    //GameObject.CreatePrimitive(PrimitiveType.Sphere).transform.position = new Vector3(l.P1.x, 0, l.P1.y);
                }      
            }
            
            vertices2D.Sort(new ClockwiseComparer(site));
                
            // Use the triangulator to get indices for creating triangles
            Triangulator tr = new Triangulator(vertices2D.ToArray());
            int[] indices = tr.Triangulate();

            // Create the Vector3 vertices
            Vector3[] vertices = new Vector3[vertices2D.Count];
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] = new Vector3(vertices2D[i].x,0 , vertices2D[i].y);
            }
            // Create the mesh
            Mesh msh = new Mesh();
            msh.vertices = vertices;
            msh.triangles = indices;

            msh.RecalculateBounds();
            msh.RecalculateNormals();
            

            // Create Voronoi Cell


            GameObject go = new GameObject("Cell "+i1);

            // Set up game object with mesh;
            go.AddComponent<MeshRenderer>().material = cellMaterial;
            MeshFilter filter = go.AddComponent(typeof(MeshFilter)) as MeshFilter;
            filter.mesh = msh;


            MapCell cell = new MapCell
            {
                ID = i1,
                voronoiCell = go,
                position = site
            };
            go.transform.SetParent(cellHolder.transform);
            cells.Add(cell);
            

        }
        return cells;
    }
    List<LineSegment> OrderEdges(List<LineSegment> segments)
    {
        List<LineSegment> ordered = new List<LineSegment>();

        while (segments.Count > 0)
        {
            LineSegment min = segments[0];

            for (int i = 0; i < segments.Count; i++)
            {
                if (CompareEdge(min.P0, segments[i].P0))
                    min = segments[i];
                if (CompareEdge(min.P0, segments[i].P1))
                    min = segments[i];
                if (CompareEdge(min.P1, segments[i].P0))
                    min = segments[i];
                if (CompareEdge(min.P1, segments[i].P1))
                    min = segments[i];
            }
            ordered.Add(min);
            segments.Remove(min);
        }
        return ordered;
    }
}
