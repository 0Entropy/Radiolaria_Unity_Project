using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Shape2DEditor : MonoBehaviour {

	private List<PolygonEditor> editors = new List<PolygonEditor>();

	public PolygonEditor editorPrefab;

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

	public void OnDestory(){
		foreach(PolygonEditor editor in editors){
			editor.RecyclePoints();
		}
		editors.Clear();
	}
	
	void Start ()
    {
		editorPrefab.CreatePool();
	}
	
}
