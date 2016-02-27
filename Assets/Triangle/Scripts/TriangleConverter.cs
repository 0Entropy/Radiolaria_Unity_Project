using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using TriangleNet;
using TriangleNet.Data;
using TriangleNet.Geometry;
using TriangleNet.Tools;

public class TriangleConverter {

     public TriangleNet.Mesh TriangleMesh { set; get; }

    
// 
//     public Vector3[] vertices { set; get; }
//     public Vector2[] uv { set; get; }
//     public int[] triangles { set; get; }

    public static Vector3[] ConverteVertices(ICollection<Vertex> vertices)
    {
        Vector3[] result = new Vector3[vertices.Count];
        int i = 0;
        foreach (var pt in vertices.Select(p => new Vector3((float)p.X, (float)p.Y, 0)))
        {
            result[i] = pt;
            i++;
        }
        return result;
    }

    public static int[] ConverteTriangles(ICollection<Triangle> triangles)
    {
        var result = new List<int>(3 * triangles.Count);
        foreach (var tri in triangles)
        {
            result.Add(tri.P0);
            result.Add(tri.P2);
            result.Add(tri.P1);

        }
        return result.ToArray();
    }

    public static Vector2[] ConverteUVs(TriangleNet.Mesh mesh)
    {
        double w = mesh.Bounds.Width;
        double h = mesh.Bounds.Height;
        double min_x = mesh.Bounds.Xmin;
        double min_y = mesh.Bounds.Ymin;

        int n = mesh.Vertices.Count;

        Vector2[] result = new Vector2[n];
        int i = 0;
        foreach (var pt in mesh.Vertices.Select(p => new Vector2((float)(p.X / w - min_x), (float)(p.Y / h - min_y))))
        {
            result[i] = pt;
            i++;
        }
        return result;
    }

}
