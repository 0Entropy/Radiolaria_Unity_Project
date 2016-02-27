using UnityEngine;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using TriangleNet;
using TriangleNet.Geometry;
using Geometry;

public class Triangle_2D
{

    public List<List<Point>> OuterPolygon { set; get; }
    public List<List<Point>> InnerPolygon { set; get; }
    public List<List<Point>> InnerHole { set; get; }

    public TriangleNet.Mesh TriangleMesh { set; get; }

    InputGeometry Geometry;

    public Triangle_2D()
    {
        TriangleMesh = new TriangleNet.Mesh();
        Geometry = new InputGeometry();
        InnerHole = new List<List<Point>>();
        InnerPolygon = new List<List<Point>>();
        OuterPolygon = new List<List<Point>>();
    }

    public void Clear()
    {
        InnerHole.Clear();
        InnerPolygon.Clear();
        OuterPolygon.Clear();
        Geometry.Clear();
    }



    public void BuildMeshFromGeometry()
    {
        if (OuterPolygon != null)
            foreach (var poly in OuterPolygon)
                Geometry.AddPoly(poly);

        if (InnerPolygon != null)
            foreach (var poly in InnerPolygon)
                Geometry.AddPoly(poly);

        if (InnerHole != null)
            foreach (var hole in InnerHole)
                Geometry.AddPolyAsHole(hole);

        TriangleMesh.Triangulate(Geometry);

    }
}
