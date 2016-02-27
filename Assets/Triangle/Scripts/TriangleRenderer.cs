﻿using UnityEngine;
using System.Collections.Generic;
/*using TriangleNet;*/
using TriangleNet.Tools;
using TriangleNet.IO;

[RequireComponent (typeof(MeshFilter), typeof(MeshRenderer))]
public class TriangleRenderer : MonoBehaviour {

    TriangleNet.Mesh TriMesh;
    
	// Use this for initialization
	void Start () {

        TriMesh = new TriangleNet.Mesh();
        /*_mesh.Triangulate("Assets/Plugins/Data/superior.poly");*/

        var geometry = FileReader.ReadPolyFile("Assets/Plugins/Data/superior.poly");
        TriMesh.behavior.Quality = true;
        TriMesh.behavior.MinAngle = 33.8f;
        TriMesh.Triangulate(geometry);
        //TriMesh.Refine(true);
        // Get mesh statistics.
        var statistic = new Statistic();
        statistic.Update(TriMesh, 1);

        // Refine by setting a custom maximum area constraint.
        TriMesh.Refine(statistic.LargestArea / 4);
        TriMesh.Smooth();

        Debug.Log(string.Format("width {0:0.00}, height {1:0.00}; min {2:0.00}, {3:0.00}; max {4:0.00}, {5:0.00}",
            TriMesh.Bounds.Width,TriMesh.Bounds.Height,
            TriMesh.Bounds.Xmin, TriMesh.Bounds.Ymin,
            TriMesh.Bounds.Xmax, TriMesh.Bounds.Ymax));

        OnRender(TriMesh);
    }
    
    public void OnRender(TriangleNet.Mesh triangleMesh)
    {

        /*data.SetMesh(mesh);
        UniMesh.vertices = data.Vertices;
        UniMesh.triangles = data.Triangles;*/

        var _mesh = GetComponent<MeshFilter>().mesh;
        if (!_mesh)
        {
            _mesh = new Mesh();
            GetComponent<MeshFilter>().mesh = _mesh;
        }

        _mesh.Clear();
        _mesh.vertices = TriangleConverter.ConverteVertices(triangleMesh.Vertices);

        List<Vector3> normals = new List<Vector3>();
        for (int i = 0; i < _mesh.vertices.Length; i++)
            normals.Add(Vector3.back);

        _mesh.normals = normals.ToArray();
        _mesh.uv = TriangleConverter.ConverteUVs(triangleMesh);
        _mesh.triangles = TriangleConverter.ConverteTriangles(triangleMesh.Triangles);
    }

    // Update is called once per frame
    void Update () {
	
	}
}
