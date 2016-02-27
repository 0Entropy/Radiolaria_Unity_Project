using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Geometry;

public class Border_2D : MonoBehaviour {

    public int InitialVerticesCount = 7;
    public float InitialRadius = 3.0f;

    public _Vertex vertexPrefab;
    public _Block blockPrefab;

    List<_Vertex> vertexs = new List<_Vertex>();
    List<_Block> blocks = new List<_Block>();

    Shape2D Shape;
    //Face2D Face;
    List<Vector2> InitialVertices;
    
    Lazy<List<Vector2>> _lazyAllVertices;
    public List<Vector2> Vertices { get { return _lazyAllVertices.Value; } }
    private List<Vector2> GetAllVertices()
    {
        return Shape.Faces[0].ActualPoints.Select(p => (Vector2)(p.Position)).ToList();
        //return Shape.AllPoints.Select(p => (Vector2)(p.Position)).ToList();
        //return points.Select(p => (Vector2)(p.Position)).ToList();
    }

    /*public void UpdateVertices()
    {
        _lazyAllVertices.isValeChanged = true;
    }*/

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder("Border Vertices : \n");
        foreach(var p in Vertices)
        {
            sb.Append(string.Format("[{0:0.00},{1:0.00}],", p.x, p.y));
        }
        sb.Remove(sb.Length - 1, 1);
        return sb.ToString();
    }

    void Start()
    {
        vertexPrefab.CreatePool();
        blockPrefab.CreatePool();
        
        _lazyAllVertices = new Lazy<List<Vector2>>(GetAllVertices);

        Shape = new Shape2D();
        CalcInitialVertices();

        /*Polygon poly = new Polygon();
        poly.SetVertices(2);*/

        //polygon = poly;
        InitialPoints();

        _InputManager.Instance.Init();
    }

    void CalcInitialVertices()
    {
        //float radius = 2.0f;
        //int count = 7;
        InitialVertices = new List<Vector2>();
        float angle = Mathf.PI * 2.0f / InitialVerticesCount;
        for (int i=0; i< InitialVerticesCount; i++)
        {

            float x = Mathf.Sin(angle * i) * InitialRadius;
            float y = Mathf.Cos(angle * i) * InitialRadius;
            InitialVertices.Add( new Vector2(x, y));
        }
    }

    public void InitialPoints()
    {
       
        foreach (var pos in InitialVertices)
        {
            _Vertex vertex = vertexPrefab.Spawn();
            vertex.transform.SetParent(transform);
            vertex.transform.localPosition = pos;
            vertex.Border = this;
            
            vertexs.Add(vertex);
        }

        Face2D face = Shape.AddPoints(vertexs.Select(v => v.Point).ToArray());
        face.Name = "BorderFace";

        foreach(var e in face.BasicEdges)
        {
            _Block block = blockPrefab.Spawn();
            block.transform.SetParent(transform);
            block.Border = this;

            block.Edge = e;
            blocks.Add(block);
        }

        _lazyAllVertices.isValeChanged = true;
    }

    public void HandleOnClear()
    {
        foreach(var v in vertexs)
        {
            v.Recycle();
        }
        vertexs.Clear();

        foreach(var b in blocks)
        {
            b.Recycle();
        }
        blocks.Clear();

        Shape.Clear();

        _lazyAllVertices.isValeChanged = true;
        
    }

    public void Refersh()
    {
        HandleOnClear();
        InitialPoints();
    }

    public void TransferPoint(_Vertex vertex)
    {
        if(!_lazyAllVertices.isValeChanged)
            _lazyAllVertices.isValeChanged = true;
    }
    
    public void InsertPointAt(_Block block, Vector3 position)
    {
        //_Block block = blockObj.GetComponent<_Block>();
        Edge2D edge = block.Edge;

        block.Recycle();
        blocks.Remove(block);

        _Vertex vertex = vertexPrefab.Spawn();
        vertex.transform.SetParent(transform);
        vertex.transform.position = position;
        vertex.Border = this;

        edge.MiddlePoint = vertex.Point;

        Edge2D e0 = new Edge2D(edge.Points[0], vertex.Point);
        Edge2D e1 = new Edge2D(vertex.Point, edge.Points[1]);

        _Block b0 = blockPrefab.Spawn();
        _Block b1 = blockPrefab.Spawn();
        b0.transform.SetParent(transform);
        b1.transform.SetParent(transform);
        b0.Border = this;
        b1.Border = this;
        b0.Edge = e0;
        b1.Edge = e1;

        blocks.Add(b0);
        blocks.Add(b1);

        vertexs.Add(vertex);

        _lazyAllVertices.isValeChanged = true;

        //Debug.Log("Border Vertices Count : " + Vertices.Count);
        
    }

    #region Transfer,Insert and Remove _Point
    /// <summary>
    /// Directions the of 序列号index处NextPoint至此_Point的方向. 
    /// </summary>
    /// <returns>The 方向矢量 of.</returns>
    /// <param name="index">Index.</param>


    /*public int PreIndex(int index)
    {

        return _polygon.PreIndex(index);
    }

    public int NextIndex(int index)
    {

        return _polygon.NextIndex(index);
    }

    public _Point NextPoint(int index)
    {
        return points[NextIndex(index)];
    }

    public _Point PrePoint(int index)
    {
        return points[PreIndex(index)];
    }

    public _Point NextPoint(_Point point)
    {
        int index = points.IndexOf(point);
        return NextPoint(index);
    }*/

    

    /*public void RemovePointAt(Transform point)
    {

        _Point originPoint = point.GetComponent<_Point>();
        int index = points.IndexOf(originPoint);
        _Block originBlock = blocks[index];

        blocks[NextIndex(index)].SetEndsPosition(_vertices[PreIndex(index)], _vertices[NextIndex(index)]);

        points.Remove(originPoint);
        blocks.Remove(originBlock);
        _vertices.RemoveAt(index);

        originBlock.Recycle();
        originPoint.Recycle();

        if (_vertices.Count < 1)
        {
            RecyclePoints();
        }
    }*/

    //	public void OnReverse(){
    //		vertices.Reverse();
    //		points.Reverse();
    //		blocks.Reverse();
    //		for(int i=0; i<vertices.Count; i++){
    //			blocks[i].SetEndsPosition(vertices[PreIndex(i)], vertices[i]);
    //			blocks[NextIndex(i)].SetEndsPosition(vertices[i], vertices[NextIndex(i)]);
    //		}
    //	}

    #endregion


    #region Edit


    // Update is called once per frame
    void Update()
    {

    }

    #endregion
}
