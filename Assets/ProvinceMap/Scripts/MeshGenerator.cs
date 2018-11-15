using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public struct VertexWithIndex {
     public Vector2 vertex;
     public int index;
 
     public VertexWithIndex(Vector2 vertex, int index) {
         this.vertex = vertex;
         this.index = index;
     }
 
     public bool Is(VertexWithIndex other) {
         if (other.vertex == this.vertex && other.index == this.index) {
             return true;
         }
 
         return false;
     }
 }
public static class MeshGenerator{
    

    public static void BuildMesh(List<Vector3> points,Transform parent)
    {
        // Use the triangulator to get indices for creating triangles
        //Triangulator tr = new Triangulator(points);
        Vector2[] v2arr = points.Select(v=>new Vector2(v.x, v.y)).ToArray();

        int[] indices = TriangulateHull(v2arr);

        // Create the Vector3 vertices
        Vector3[] vertices = new Vector3[points.Count];
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = new Vector3(points[i].x, points[i].y, 0);
        }

        // Create the mesh
        Mesh msh = new Mesh();
        msh.vertices = vertices;
        msh.triangles = indices;
        msh.RecalculateNormals();
        msh.RecalculateBounds();

        Vector2[] uvs = new Vector2[vertices.Length];
 
         float rightX = (vertices.Max(a=>a.x) - vertices.Min(a=>a.x)) / (vertices.Max(a=>a.y) - vertices.Min(a=>a.y));
         float l_width = msh.bounds.size.x;
         float l_height = msh.bounds.size.z;
         for(int i = 0; i < uvs.Length; i++) {
             float distFromMinX = vertices[i].x - msh.bounds.min.x;
             float distFromMinY = vertices[i].y - msh.bounds.min.y;
 
             uvs[i] = new Vector2(distFromMinX / l_width, distFromMinY / l_height);
         }
         msh.uv = uvs;

        // Set up game object with mesh;
        GameObject newGO = new GameObject();
        newGO.transform.SetParent(parent);
        newGO.AddComponent(typeof(MeshRenderer));
        MeshFilter filter = newGO.AddComponent(typeof(MeshFilter)) as MeshFilter;
        filter.mesh = msh;
    }
    
    public static int[] TriangulateHull(Vector2[] inputVerts)
     {
         if (inputVerts.Length < 3) {
             if (inputVerts.Length == 2) {
                 return new int[] { 0, 1, };
             }
 
             return null;
         }
 
         List<VertexWithIndex> vertices = new List<VertexWithIndex>();
         for (int i = 0; i < inputVerts.Length; i++)
         {
             vertices.Add(new VertexWithIndex(inputVerts[i], i));
         }        
 
         List<int> tris = new List<int>();
         
         VertexWithIndex randomSelected = vertices.OrderBy(a=>a.vertex.y+a.vertex.x).First(); // Heuristic to pick the lowest x+y value results in all angles being in [0, 90]
         List<VertexWithIndex> restOfVertices = vertices.Where(v=>!v.Is(randomSelected)).ToList();
 
         Stack<VertexWithIndex> V = new Stack<VertexWithIndex>(restOfVertices.OrderBy(x=>x.vertex.PositiveAngleTo(randomSelected.vertex))); //The issue is here, it isn't ordering them correctly use the debugger for this method
         VertexWithIndex secondVertexInTriangle = V.Pop();
 
         //So what this algorithm does is triangulates by connecting a vertex with all other vertices.
         //Then 
         //It only works for convex hulls.
         while (V.Count > 0) {
             int[] newTriangle = new int[3]; //EnsureCorrectOrdering(randomSelected, secondVertexInTriangle, V.Peek());
             tris.Add(randomSelected.index);
             tris.Add(secondVertexInTriangle.index);
             tris.Add(V.Peek().index);
 
             secondVertexInTriangle = V.Pop();
         }
         
 
         return tris.ToArray();
     }
 
     // Given three vertices, return their indices in clockwise order
     static int[] EnsureCorrectOrdering(VertexWithIndex[] triangle) {
         return EnsureCorrectOrdering(triangle[0], triangle[1], triangle[2]);
     }
 
     static int[] EnsureCorrectOrdering(VertexWithIndex a, VertexWithIndex b, VertexWithIndex c) {
         int[] indices = new int[3];
         Vector3 AC = c.vertex - a.vertex;
         AC = new Vector3(AC.x, AC.z, AC.y);
         Vector3 AB = b.vertex - a.vertex;
         AB = new Vector3(AB.x, AB.z, AB.y);
 
 
         if (Vector3.Cross(AC, AB).sqrMagnitude > 0) {
             indices[0] = a.index;
             indices[1] = b.index;
             indices[2] = c.index;
         } else {
             indices[0] = a.index;
             indices[1] = c.index;
             indices[2] = b.index;
         }
 
         return indices;
     }    
 
     public static float PositiveAngleTo(this Vector2 this_, Vector2 to) {
         Vector2 direction = to - this_;
         float angle = Mathf.Atan2(direction.y,  direction.x) * Mathf.Rad2Deg;
         if (angle < 0f) angle += 360f;
         return angle;
     }


	public static GameObject BuildProvinceMeshes(List<Vector2> points,Transform parent,Material mat,Terrain terrain,float offset){
		//List<Vector2> DVerts = new List<Vector2>();
		//foreach (Vector3 vert in points) {
		//	DVerts.Add (new Vector2(vert.x, vert.y));
		//}

		// Use the triangulator to get indices for creating triangles
		Triangulator tr = new Triangulator(points.ToArray());
		int[] indices = tr.Triangulate();

		// Create the Vector3 vertices
		Vector3[] vertices = new Vector3[points.Count];
		for (int i=0; i<vertices.Length; i++) {
			vertices[i] = new Vector3(points[i].x,terrain.SampleHeight(new Vector3(points[i].x,0, points[i].y))+offset , points[i].y);
		}

		// Create the mesh
		Mesh msh = new Mesh();
		msh.vertices = vertices;
		msh.triangles = indices;
		msh.RecalculateNormals();
		msh.RecalculateBounds();

		// Set up game object with mesh;
		GameObject go = new GameObject();
		MeshRenderer rend = (MeshRenderer) go.AddComponent(typeof(MeshRenderer));
		rend.material = mat;
		MeshFilter filter = go.AddComponent(typeof(MeshFilter)) as MeshFilter;
		filter.mesh = msh;
		go.transform.SetParent (parent);
		//go.transform.rotation = Quaternion.Euler (90, 0, 0);
		return go;
	}
}

