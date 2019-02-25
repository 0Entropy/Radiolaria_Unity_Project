using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Geometry;
using TriangleNet.Geometry;

public class Context_2D_3D : MonoBehaviour
{
    public Camera Camera2D;

    public Camera Camera3D;

    public Border_2D Border2D;

    public Mesh3DRenderer Mesh3D;

    public Mesh3DRenderer Mesh2D;

    List<Point2D> points;

    public Shape2D shape { set; get; }
    
    public float width { private set; get; }
    public float height { private set; get; }
    int cols = 9;//15;
    int rows;

    float tinkness = 0.05f;

    Matrix4x4 ScreenToContextMat;

    const float SIN60 = 0.866f;//1.732f;//

    //public List<Point> BorderRect;
    List<Vector2> BorderVertices;

    ForceField mForceField;

    float AspectRatio;

    void OnEnable()
    {
        ButtonManager_2D.OnCheckRadio += ButtonManager_2D_OnCheckRadio;
        ButtonManager_2D.OnCheckToggle += ButtonManager_2D_OnCheckToggle;
        ButtonManager_2D.OnCheckButton += ButtonManager_2D_OnCheckButton;
        _InputManager.OnClick += _InputManager_OnClick;

        ButtonManager_3D.OnCheckRadio += ButtonManager_3D_OnCheckRadio;

        UIController.OnNavigateTo += UIController_OnNavigateTo;
    }

    private void UIController_OnNavigateTo(UIState state)
    {
        //
        if(state == UIState.VIEW_3D)
        {
            var bounds = Mesh3D.GetComponent<MeshFilter>().sharedMesh.bounds;
            var scalar = -Mathf.Max(bounds.size.x, bounds.size.y) * AspectRatio * 1.05f;
            Camera3D.transform.position = new Vector3(bounds.center.x, bounds.center.y, scalar);

            Mesh3D.GetComponent<_RotateHandler>().StartRotate();

            Camera3D.gameObject.SetActive(true);
            Camera2D.gameObject.SetActive(false);

            Debug.Log("Context Navigate to 3D");
        }

        if(state == UIState.VIEW_2D)
        {

            Mesh3D.GetComponent<_RotateHandler>().StopRotate();

            Camera2D.gameObject.SetActive(true);
            Camera3D.gameObject.SetActive(false);
            
            Debug.Log("Context Navigate to 2D");
        }
    }

    private void ButtonManager_3D_OnCheckRadio(string name)
    {
        Debug.Log(name);
        Mesh3D.GetComponent<MeshRenderer>().material.mainTexture =
                    Resources.Load<Texture2D>("MaterialTexture/" + name);
/*

        switch (name)
        {
            case "StainlessSteel":
                //Debug.Log("StainlessSteel");
                Mesh3D.GetComponent<MeshRenderer>().material.mainTexture =
                    Resources.Load<Texture2D>("MaterialTexture/" + name);
                break;
            case "Gold":
                Debug.Log("Gold");
                break;
            case "Wood":
                Debug.Log("Wood");
                break;
            case "Black":
                Debug.Log("Black");
                break;
            default:
                break;
        }*/
    }

    private void _InputManager_OnClick(Vector3 position)
    {
        /*if (Input.mousePosition.y < 128)//Button Panel's width = 128 pixels.
            return;*/
            

        //Vector2 inputPosition = ScreenToContextMat.MultiplyPoint3x4(Input.mousePosition);

        //",,Separator,,,,,,Force_Toggle,Separator,Border_Toggle,Separator,Refresh_Basic";

        switch (mouseType)
        {
            case "CellDivision":
                shape.OnDivide(position);
                break;
            case "CellUnion":
                shape.OnUnite(position);
                break;
            case "ForceAttractive":
                mForceField.AttachAttractiveForceAt(position);
                break;
            case "ForceRepulsive":
                mForceField.AttachAttractiveForceAt(position, true);
                break;
            case "ForceClockwise":
                mForceField.AttachTorqueForceAt(position);
                break;
            case "ForceAnticlockwise":
                mForceField.AttachTorqueForceAt(position, true);
                break;
            case "ForceClear":
                Debug.Log("ForceClear");
                break;
            default:
                break;
        }
    }

    void OnDisable()
    {
        ButtonManager_2D.OnCheckRadio -= ButtonManager_2D_OnCheckRadio;
        ButtonManager_2D.OnCheckToggle -= ButtonManager_2D_OnCheckToggle;
        ButtonManager_2D.OnCheckButton -= ButtonManager_2D_OnCheckButton;
        _InputManager.OnClick -= _InputManager_OnClick;

        ButtonManager_3D.OnCheckRadio -= ButtonManager_3D_OnCheckRadio;

        UIController.OnNavigateTo -= UIController_OnNavigateTo;
    }

    private void ButtonManager_2D_OnCheckButton(string name)
    {
        if(name == "Refresh")
        {
            HandleOnRefersh();
        }
    }

    private void ButtonManager_2D_OnCheckToggle(string name, bool isOn)
    {
        //
        switch (name)
        {
            case "Force":
                Debug.Log("Force toggle to " + isOn);
                break;
            case "Border":
                Border2D.gameObject.SetActive(isOn);
                break;
        }
    }

    string mouseType = string.Empty;
    private void ButtonManager_2D_OnCheckRadio(string name)
    {
        mouseType = name;
        //Debug.Log("ButtonManager_OnCheckRadio(string) : Muse Type : " + mouseType);
    }

    // Update is called once per frame
    void Update()
    {
        CalcTrianglePolygon();
        
    }

    void HandleOnRefersh()
    {
        Mesh3D.GetComponent<MeshRenderer>().material.mainTexture =
                    Resources.Load<Texture2D>("MaterialTexture/Wood");

        mouseType = string.Empty;

        mForceField.Clear();

        if (!Border2D.gameObject.activeSelf)
            Border2D.gameObject.SetActive(true);
        Border2D.Refersh();


        CalcPoints();

        ClacShape2D();

        foreach (var f in shape.Faces)
            f.AddRigidbodyAndSpring(1.0f);
    }
    

    void Start()
    {

        ClacDimension();

        CalcPoints();
        
        ClacShape2D();
        
        foreach (var f in shape.Faces)
            f.AddRigidbodyAndSpring(1.0f);
        
    }

    public void CalcTrianglePolygon()
    {
        Mesh3D.Clear();
        Mesh2D.Clear();
        

        Vector2[] border_polygon = Border2D.Vertices.ToArray();

        foreach (var f in shape.Faces)
        {
            bool isAllInBorder = true;

            List<Vector2> poly = f.ActualPoints.Select(p => (Vector2)(p.Position)).ToList();

            //var outer2D = Utils.ScalePoly(poly, 0.05f);
            Mesh2D.OuterPolygon.Add(poly);////(outer2D);

            List<Vector2> inner2D = Utils.ScalePoly(poly, -tinkness);

            Mesh2D.InnerHole.Add(inner2D);

            foreach (var pos in poly)
            {
                if (Utils.WN_PnPoly(pos, border_polygon) == 0)
                {
                    isAllInBorder = false;
                    break;
                }
            }

            if (isAllInBorder)
            {
                //List<Vector2> poly3D = f.ActualPoints.Select(p => (Vector2)(p.Position)).ToList();

                var outer3D = Utils.ScalePoly(poly, tinkness);
                //poly = f.ActualPoints.Select(p => (Vector2)(p.Position)).ToList();
                Mesh3D.OuterPolygon.Add(outer3D);// (outer3D);

                var inner3D = Utils.ScalePoly(poly, -tinkness);
                Mesh3D.InnerHole.Add(inner3D);
            }
        }

        Mesh3D.UpdateMesh();
        Mesh2D.UpdateMesh();

    }
    
    void FixedUpdate()
    {
        foreach (var point in shape.AllPoints)
        {
            if (point.UnityObject.GetComponent<Rigidbody>())
            {
                //point.UnityObject.rigidbody.AddForce(mForceField.Lookup(point.Position));
                //AttachTorqueForceAt
                point.UnityObject.GetComponent<Rigidbody>().AddForce(mForceField.Lookup(point.Position));

            }
        }
    }

    


    void ClacDimension()
    {
        width = 1.5f * (cols - 1) + 1.0f;
        AspectRatio = (float)Screen.height / (float)Screen.width;
        float actualHeight = width * AspectRatio;
        height = actualHeight + SIN60 * 2.0f;
        rows = (int)(height / SIN60);
        if (rows % 2 == 0)
        {
            rows++;
        }
        
        Camera2D.orthographicSize = actualHeight * 0.5f;
        
        ScreenToContextMat = Matrix4x4.TRS(new Vector3(-width, -actualHeight, 0) * 0.5f, Quaternion.identity, Vector3.one * width / Screen.width);
        float w2 = width * 0.5f;
        float h2 = height * 0.5f;
        //BorderRect = new List<Point>() { new Point(-w2, -h2), new Point(w2, -h2), new Point(w2, h2), new Point(-w2, h2) };
        BorderVertices = new List<Vector2>() { new Vector2(-w2, -h2), new Vector2(w2, -h2), new Vector2(w2, h2), new Vector2(-w2, h2) };

        mForceField = new ForceField(width, actualHeight,2f);

        Debug.Log(mForceField);
        
    }

    void CalcPoints()
    {
        points = new List<Point2D>();

        //joints = new List<Point2D>();
        float x = -width * 0.5f - 1.5f;
        float y;
        //Debug.Log("x");
        for (int i = 0; i <= cols; i++)
        {

            x += i % 2 == 0 ? 1.0f : 2.0f;
            y = height * 0.5f + SIN60; //- 0.866f;
            //Debug.Log(x);

            for (int j = 0; j <= rows; j++)
            {
                y -= SIN60;
                float xx = i == 0 ? (-width * 0.5f) : i == cols ? (width * 0.5f) : x;
                
                Point2D point = new Point2D(new Vector3(xx, y, 0));
                point.Name = string.Format("({0},{1})", i, j);
                if (i == 0 || j == 0 || i == cols || j == rows)
                {
                    point.IsOnBoundary = true;

                }
                
                points.Add(point);
            }
        }
    }
    
    void ClacShape2D()
    {
        shape = new Shape2D();
        
        for (int i = 0; i < cols; i++)
        {
            int offset = i % 2 == 0 ? 1 : 0;
            for (int j = 0; j < rows / 2; j++)
            {
                Point2D p0 = GetPointFromMat(i,     2 * j + offset);
                Point2D p1 = GetPointFromMat(i,     2 * j + offset + 1);
                Point2D p2 = GetPointFromMat(i,     2 * j + offset + 2);
                Point2D p3 = GetPointFromMat(i + 1, 2 * j + offset + 2);
                Point2D p4 = GetPointFromMat(i + 1, 2 * j + offset + 1);
                Point2D p5 = GetPointFromMat(i + 1, 2 * j + offset);
                
                Face2D face = shape.AddPoints( p0, p1, p2, p3, p4, p5);
                face.Name = string.Format("{{{0},{1}}}", i, j);
            }
        }
        //Debug.Log(shape.ToString());
        
    }

    public override string ToString()
    {
        return string.Format("width : {0}, height : {1}, cols : {2}, rows : {3},\n faces : {4}, edges : {5}, points : {6}",
            width, height, cols, rows, shape.Faces.Count, shape.AllEdges.Count, shape.AllPoints.Count);
    }

    Point2D GetPointFromMat(int colNum, int rowNum)
    {
        int n = (rows + 1) * colNum + rowNum;
        //Debug.Log(n);
        return points[n];
    }
    
}
