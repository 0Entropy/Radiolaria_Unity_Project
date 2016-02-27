using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public class Triangle2DRenderer : AbstractMeshRenderer
{
    public TriangleNet.Mesh TriangleMesh { set; get; }
    protected override void HandleOnUpdate()
    {
        Clear();

        vertices = TriangleMesh.Vertices.Select(p => new Vector3((float)p.X, (float)p.Y, 0)).ToList();
        
        for (int i = 0; i < vertices.Count; i++)
            normals.Add(Vector3.back);
        
        double w = TriangleMesh.Bounds.Width;
        double h = TriangleMesh.Bounds.Height;
        double min_x = TriangleMesh.Bounds.Xmin;
        double min_y = TriangleMesh.Bounds.Ymin;

        int n = TriangleMesh.Vertices.Count;
        
        foreach (var pt in TriangleMesh.Vertices.Select(p => new Vector2((float)(p.X / w - min_x), (float)(p.Y / h - min_y))))
        {
            uv.Add(pt);
        }
        
        foreach (var tri in TriangleMesh.Triangles)
        {
            triangles.Add(tri.P0);
            triangles.Add(tri.P2);
            triangles.Add(tri.P1);
        }
    }
}
