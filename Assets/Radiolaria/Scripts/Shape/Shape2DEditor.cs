using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Shape2DEditor : MonoBehaviour {

//	public PolygonTree polygonTree;
//	
//	public Point vertexPrefab;
//	public Block edgePrefab;
//	
//	public List<Point> points = new List<Point>();
//	public List<Block> blocks = new List<Block>();

//	public PolygonTree origin;
	private List<PolygonEditor> editors = new List<PolygonEditor>();

	public PolygonEditor editorPrefab;

//	private PolygonTree _shape2D;
	public PolygonTree shape2D{
		set{
			PolygonEditor exteriorEditor = editorPrefab.Spawn(transform.position, transform.rotation);
			exteriorEditor.transform.parent = transform;
			exteriorEditor.polygon = value.exterior;
			exteriorEditor.SpawnPoints();
			editors.Add (exteriorEditor);
			
			foreach(Polygon inter in value.interiors){
				PolygonEditor interiorEditor = editorPrefab.Spawn(transform.position, transform.rotation);
				interiorEditor.transform.parent = transform;
				interiorEditor.polygon = inter;
				interiorEditor.SpawnPoints();
				editors.Add (interiorEditor);
			}
		}
	}
//	
//	public void DoEdit(Slice slice){
////		editors =
////		origin = pt;
//		PolygonEditor exteriorEditor = editorPrefab.Spawn();
//		exteriorEditor.transform.parent = slice.transform;
//		exteriorEditor.polygon = slice.originShape.exterior;
//		exteriorEditor.SpawnPoints();
//		editors.Add (exteriorEditor);
//
//		foreach(Polygon inter in slice.originShape.interiors){
//			PolygonEditor interiorEditor = editorPrefab.Spawn();
//			interiorEditor.transform.parent = slice.transform;
//			interiorEditor.polygon = inter;
//			interiorEditor.SpawnPoints();
//			editors.Add (interiorEditor);
//		}
//	}

	public void OnDestory(){
		foreach(PolygonEditor editor in editors){
			editor.RecyclePoints();
//			editor.Recycle();
		}
		editors.Clear();
	}
	
	// Use this for initialization
	void Start () {
//		vertexPrefab.CreatePool();
//		edgePrefab.CreatePool();
		editorPrefab.CreatePool();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
