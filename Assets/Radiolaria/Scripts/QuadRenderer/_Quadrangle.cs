using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer),typeof(BoxCollider))]
public abstract class _Quadrangle : MonoBehaviour {

    //   
    //  up(y)				1-----width----2
    //  |					|              |
    //  |					|              |
    //  |					|            length
    //  |					|              |
    //  |					|              |
    //  .-------->right(x)	0--------------3
    //

    public bool IsUpdateAtRuntime = false;

    const int VerticesLength = 4;

    protected string Name = "Quadrangle";

    protected Vector3[] Vertices { set; get; }

    protected Vector2[] UV = {
        new Vector2(0, 0),
        new Vector2(0, 1),
        new Vector2(1, 1),
        new Vector2(1, 0)
    };

    protected int[] Triangles = { 0, 1, 2, 0, 2, 3 };

    protected Vector3[] Normals = { Vector3.back, Vector3.back, Vector3.back, Vector3.back };

    protected Vector4[] Tangents = {
        new Vector4(-1, 0, 0, -1),
        new Vector4(-1, 0, 0, -1),
        new Vector4(-1, 0, 0, -1),
        new Vector4(-1, 0, 0, -1)
    };

    /*protected Mesh UnityMesh
    {
        get
        {
            var _mesh = GetComponent<MeshFilter>().mesh;
            if (!_mesh)
            {
                _mesh = new UnityEngine.Mesh();
                GetComponent<MeshFilter>().mesh = _mesh;
            }
            return _mesh;
        }
    }*/
    
    protected void UpdateMesh()
    {
        if (Vertices.Length != VerticesLength)
            throw new System.Exception("All Quadrangle has only four vertices, no more no less!");

        var _mesh = GetComponent<MeshFilter>().mesh;
        if (!_mesh)
        {
            _mesh = new UnityEngine.Mesh();
            GetComponent<MeshFilter>().mesh = _mesh;
        }

        _mesh.Clear();
        _mesh.vertices = Vertices;
        _mesh.uv = UV;
        _mesh.triangles = Triangles;
        _mesh.normals = Normals;
        //_mesh.Optimize();

        /*Bounds bounds = UnityMesh.bounds;
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        boxCollider.size = new Vector3(bounds.size.x, bounds.size.y, 0.1f);
        boxCollider.center = bounds.center;*/
        //GetComponent<MeshCollider>().sharedMesh = UnityMesh;// GetComponent<MeshFilter>().sharedMesh;
    }

    protected void UpdateVertices()
    {
        HandleOnUpdateVertices();
        UpdateMesh();
        HandleOnUpdateCollider();
    }

    protected void OnInitial()
    {
        
        HandleOnInitial();
        UpdateVertices();
        //UnityMesh.name = Name;
        /*if (!collider)
            gameObject.AddComponent<BoxCollider>();
        GetComponent<MeshCollider>().sharedMesh = UnityMesh;*/




    }

    protected abstract void HandleOnUpdateVertices();

    protected abstract void HandleOnInitial();

    protected abstract void HandleOnUpdateCollider();

    protected void Start()
    {
        OnInitial();
    }

    protected void Update()
    {
        if (IsUpdateAtRuntime)
            UpdateVertices();
    }
}
