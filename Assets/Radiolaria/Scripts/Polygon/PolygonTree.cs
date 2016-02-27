using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System;

[Serializable]
public class PolygonTree {

	public static void ShapeCopy(ref PolygonTree src, ref PolygonTree dst){

		dst.exterior.vertices = new List<Vector2>(src.exterior.vertices);
		if(dst.interiors.Count > 0)
			dst.interiors.Clear ();
		foreach(Polygon inter in src.interiors){
			Polygon interior = new Polygon();
			interior.vertices = new List<Vector2>(inter.vertices);
			dst.interiors.Add(interior);
		}
	}


	[SerializeField]
	private Polygon _exterior = new Polygon();
	public Polygon exterior{
		set{ _exterior = value; }
		get{
			return _exterior;
		}
	}

	[SerializeField]
	private List<Polygon> _interiors = new List<Polygon>();
	public List<Polygon> interiors{
//		set; get;
		set{ _interiors = value;} 
		get{

//			if(_interiors == null)
//				_interiors = new List<Polygon>();
			return _interiors;
		}
	}

//	protected Polygon InstantiatePolygon() {
//		GameObject polygonObj = Instantiate( polygonPrefab ) as GameObject;
//		polygonObj.transform.parent = transform;
//		polygonObj.transform.localPosition = Vector2.zero;
//		polygonObj.transform.localRotation = Quaternion.identity;
//
//		Polygon polygon = polygonObj.GetComponent<Polygon>();
//		polygon.parent = this;
//
////		return polygonObj.GetComponent<Polygon>();
//		return polygon;
//
//	}


	#region Interior Polygon
	
	/// <summary>
	/// Sets the exterior. 设置外轮廓。
	/// </summary>
	/// <param name="dimension">Dimension. </param>
	public virtual void SetExterior(float dimension) {
//		exterior = new Polygon(dimension);
		if(_exterior == null)
		{
			_exterior = new Polygon();
		}
		_exterior.SetVertices(dimension);
	}

	public virtual void SetExterior(float width, float height) {
		//		exterior = new Polygon(dimension);
		if(_exterior == null)
		{
			_exterior = new Polygon();
		}
		_exterior.SetVertices(width, height);
	}

	public virtual void SetExterior(Vector2[] array) {
//		_exterior = new Polygon(array);
		if(_exterior == null)
		{
			_exterior = new Polygon();
		}
		_exterior.SetVertices(array);
	}
	
	/// <summary>
	/// Adds the interior. 添加内轮廓（正方形孔洞）。
	/// </summary>
	/// <param name="dimension">Dimension.正方形孔洞尺寸。</param> 
	public virtual Polygon AddInterior(float dimension){

		Polygon inter = new Polygon();//InstantiatePolygon();
		
		interiors.Add(inter);

		inter.SetVertices(dimension);

		return inter;
		
	}
	
	public virtual Polygon AddInterior(Vector2[] vertices){
		Polygon inter = new Polygon();//InstantiatePolygon();
		
		interiors.Add(inter);

		inter.SetVertices(vertices);

		return inter;
		
	}



//	public virtual void OnUpdate(){
//		if(polygonMesh.polygonTree != this)
//			polygonMesh.polygonTree = this;
//		polygonMesh.OnUpdate();
//	}

//	void Start () {
//		polygonMesh.polygonTree = this;
//	}

	#endregion

}
