using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public abstract class AbstractMeshRenderer : MonoBehaviour {

    

    protected List<Vector3> vertices = new List<Vector3>();
    protected List<Vector2> uv = new List<Vector2>();
    protected List<int> triangles = new List<int>();
    protected List<Vector3> normals = new List<Vector3>();

    protected abstract void HandleOnUpdate();

    public virtual void Clear()
    {
        vertices.Clear();
        normals.Clear();
        uv.Clear();
        triangles.Clear();
    }
    
    public void UpdateMesh()
    {
        HandleOnUpdate();

        var _mesh = GetComponent<MeshFilter>().mesh;
        if (!_mesh)
        {
            _mesh = new UnityEngine.Mesh();
            GetComponent<MeshFilter>().mesh = _mesh;
        }

        _mesh.Clear();
        _mesh.vertices = vertices.ToArray();
        if (normals.Count == 0)
            _mesh.RecalculateNormals();
        else
        _mesh.normals = normals.ToArray();
        _mesh.uv = uv.ToArray();
        _mesh.triangles = triangles.ToArray();

    }
}
