using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Geometry;

public class _Vertex : _Square
{
    public Border_2D Border { set; get; }

    private Point2D _point;
    public Point2D Point {
        get
        {
            if(_point == null)
            {
                _point = new Point2D(transform.position, "Point");
                _point.AddGameObject(gameObject, 1.0f, false);
                
            }
            return _point;
        }
    }
    
    void OnEnable()
    {
        _InputManager.OnDrag += _InputManager_OnDrag;
    }

    void OnDisable()
    {
        _InputManager.OnDrag -= _InputManager_OnDrag;
    }

    private void _InputManager_OnDrag(GameObject obj, Vector2 position)
    {
        if (gameObject != obj)
            return;

        transform.position = position;

        /*if(Border != null)
            Border.UpdateVertices();*/
        Border.TransferPoint(this);
    }

    protected override void HandleOnUpdateCollider()
    {
        if (!GetComponent<Collider>())
        {
            gameObject.AddComponent<BoxCollider>();
        }
        Bounds bounds = gameObject.GetComponent<MeshFilter>().sharedMesh.bounds;
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        boxCollider.size = new Vector3(bounds.size.x * 2.0f, bounds.size.y * 2.0f, 0.2f);
        boxCollider.center = bounds.center;
    }
    /*public _Block Left { set; get; }

    public _Block Right { set; get; }

    protected override void HandleOnUpdateVertices()
    {
        base.HandleOnUpdateVertices();
        if (Left != null)
            Left.ResetLocalPosition();
        if (Right != null)
            Right.ResetLocalPosition();
    }*/
    /*private _Point _Previous;
	public _Point Previous{
		set { _Previous = value;}
		get { return _Previous; }
	}

	public Vector3 Direction{
		get{ return (_Previous.transform.position - transform.position).normalized; }
	}

	private _Block _left, _right;
	public _Block Left {
		set {_left = value;}
		get {return _left; }
	}
	
	public _Block Right {
		set {_right = value;}
		get {return _right; }
	}
    
	public void ResetLocalPosition() {
		if(_left != null)
			_left.ResetLocalPosition();
		if(_right != null)
			_right.ResetLocalPosition();
	}*/

    ///<summary> 将与此Point相关的Slot，Hold等点位置信息储存在属性中
    /// 
    /// </summary>
    /// 
    //	public SortedList<float, List<Vector3>> ExtraPoints = new SortedList<float, List<Vector3>>();
    //	public void AddExtraPoints() {}

    //	public SortedList<Slot, List<Vector3>> SlotPoints = new SortedList<Slot, List<Vector3>>();
    //	public void AddSlotPoints() {}

    //public List<Slot> Slots = new List<Slot>();

    //	void OnMouseDown() {
    //		SendMessageUpwards("SelectChild", transform);
    //	}
    //
    //	void OnMouseUp() {
    ////		SendMessageUpwards("SetSelectedChild", null);
    //		if(Input.GetMouseButtonUp(0)){
    //			Debug.LogError("Mouse : 0" );
    //			SendMessageUpwards("ReleaseChild");
    //
    //		}
    //		if(Input.GetMouseButton(1)){
    //			Debug.LogError("Mouse : 1" );
    //			SendMessageUpwards("RemovePointAt", transform);
    //		}
    //	}
    //
    //	void OnMouseDrag() {
    //		SendMessageUpwards("TranslatePoint", transform);
    //	}
}
