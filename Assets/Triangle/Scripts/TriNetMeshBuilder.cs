using UnityEngine;
using System.Collections.Generic;
using TriangleNet;
using TriangleNet.Geometry;
using TriangleNet.Tools;
using TriangleNet.IO;
using System.Linq;

[RequireComponent(typeof(MeshFilter))]
public class TriNetMeshBuilder : MonoBehaviour
{
    InputGeometry Geometry
    {
        get
        {
            InputGeometry _geometry = new InputGeometry();

            var polys = GetComponentsInChildren<PolyMesh>();

            if(polys == null || polys.Length == 0)
            {
                Debug.LogError("There are not polyMesh!");
                throw new System.Exception("There are not a polyMesh in children!");
            }

            foreach (var poly in polys.Where(p => p.tag == "Outer"))
            {

                var points = poly.GetEdgePoints().Select(p => new Point(p.x, p.y));
                _geometry.AddPoly(points);

            }

            foreach (var poly in polys.Where(p => p.tag == "Inner"))
            {

                var points = poly.GetEdgePoints().Select(p => new Point(p.x, p.y));
                _geometry.AddPoly(points);

            }

            foreach (var poly in polys.Where(p => p.tag == "Hole"))
            {

                var points = poly.GetEdgePoints().Select(p => new Point(p.x, p.y));
                _geometry.AddPolyAsHole(points);

            }

            return _geometry;
        }
    }

    TriangleNet.Mesh triMesh;
    
    public UnityEngine.Mesh UniMesh
    {
        get
        {
            var _mesh = GetComponent<MeshFilter>().sharedMesh;
            if (!_mesh)
            {
                _mesh = new UnityEngine.Mesh();
                GetComponent<MeshFilter>().mesh = _mesh;
            }
            return _mesh;
        }
    }
    
    public void BuildMeshFromGeometry()
    {
        triMesh = new TriangleNet.Mesh();

        triMesh.behavior.Quality = true;
        triMesh.behavior.MinAngle = 1f;
        

        triMesh.Triangulate(Geometry);

        //         var statistic = new Statistic();
        //         statistic.Update(triMesh, 1);

        // Refine by setting a custom maximum area constraint.
        //triMesh.Refine(statistic.LargestArea / 4);
        //triMesh.Smooth();
        
        triMesh.Renumber();

        UniMesh.vertices = TriangleConverter.ConverteVertices(triMesh.Vertices);
        UniMesh.RecalculateNormals();
        UniMesh.triangles = TriangleConverter.ConverteTriangles(triMesh.Triangles);
        UniMesh.uv = TriangleConverter.ConverteUVs(triMesh);


        Debug.Log(string.Format("width {0:0.00}, height {1:0.00}; min {2:0.00}, {3:0.00}; max {4:0.00}, {5:0.00}",
            triMesh.Bounds.Width, triMesh.Bounds.Height,
            triMesh.Bounds.Xmin, triMesh.Bounds.Ymin,
            triMesh.Bounds.Xmax, triMesh.Bounds.Ymax));

    }

    public void UpdateMeshWith(Vector3[] _vertices)
    {
        UniMesh.vertices = _vertices;
    }

    public void UpdateMeshWith(TriMeshData data)
    {
        
        UniMesh.vertices = data.vertices.ToVector3Array();
        UniMesh.RecalculateNormals();
        UniMesh.triangles = data.triangles;
        UniMesh.uv = data.uv.ToVector2Array();
    }

    /*public void SaveMesh(int[] ids)
    {
        TriMeshData data = new TriMeshData(UniMesh);
        data.ids = ids;
        string dataJson = JsonFx.Json.JsonWriter.Serialize(data);
        string path = System.IO.Path.Combine(Application.dataPath, meshDataPath);
        _InternalDataCodec.SaveString(dataJson, path);
    }*/

}
