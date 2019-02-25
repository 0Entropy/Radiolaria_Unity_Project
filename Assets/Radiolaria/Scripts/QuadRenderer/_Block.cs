using UnityEngine;
using System.Collections;
using Geometry;
using System;

public class _Block : _Rectangle {

    public Edge2D Edge { set; get; }
    public Border_2D Border { set; get; }

    void OnEnable()
    {
        _InputManager.OnFocus += _InputManager_OnFocus;
    }
    void OnDisable()
    {
        _InputManager.OnFocus -= _InputManager_OnFocus;
    }

    private void _InputManager_OnFocus(GameObject obj, Vector2 pos)
    {
        if (gameObject != obj)
            return;
        //Debug.Log("Focus On Block!");
        Border.InsertPointAt(this, pos);
    }

    protected override void HandleOnInitial()
    {
        base.HandleOnInitial();
        IsUpdateAtRuntime = true;
        
    }

    protected override void HandleOnUpdateVertices()
    {
        if(Edge != null)
            SetEndsPosition(Edge.Points[0].LocalPosition, Edge.Points[1].LocalPosition);

        base.HandleOnUpdateVertices();

        
    }

    protected override void HandleOnUpdateCollider()
    {
        if (!GetComponent<Collider>())
        {
            gameObject.AddComponent<BoxCollider>();
        }
        Bounds bounds = gameObject.GetComponent<MeshFilter>().sharedMesh.bounds;
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        boxCollider.size = new Vector3(bounds.size.x * 2.0f, bounds.size.y, 0.1f);
        boxCollider.center = bounds.center;
    }

    /*private _Point _end;
	public _Point End{
		set { 
			_end = value;
			_end.Left = this;
			_end.Previous.Right = this;
		}
		get { return _end; }
	}

	//private PointBuilder mPointBuilder;

	public void ResetLocalPosition () {
		if(_end != null && _end.Previous != null){
            SetEndsPosition(_end.transform.localPosition, _end.Previous.transform.localPosition);
		}
	}*/

    //	private float fireRate = 0.5F;
    //	private float nextFire;
    //	private Vector3 oldInputPos;
    //
    //	void OnMouseDown() {
    //		nextFire = Time.time + fireRate;
    //		oldInputPos = Input.mousePosition;
    //		SendMessageUpwards("SelectChild", transform);
    //	}
    //
    //	void OnMouseUp() {
    //		SendMessageUpwards("ReleaseChild");
    //	}
    //
    //	void OnMouseDrag() {
    //		if(Time.time > nextFire && oldInputPos == Input.mousePosition){
    //			SendMessageUpwards("ReleaseChild");
    //			SendMessageUpwards("InsertPointAt", transform);
    //			nextFire = float.MaxValue;
    //		}
    //	}
}
