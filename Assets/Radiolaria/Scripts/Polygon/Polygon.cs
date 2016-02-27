using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System;

public class Polygon {
    
	//private List<Vector2> vertices = new List<Vector2>();
	public List<Vector2> vertices {
        set; get;
	}

	public void SetVertices(float width, float height){
		float w = width * 0.5F;
		float h = height * 0.5F;
		SetVertices(
			new Vector2[]{ 
			new Vector2(w, h), 
			new Vector2(-w, h), 
			new Vector2(-w, -h),
			new Vector2(w, -h)
		});
	}

	public void SetVertices(float dimension) {

		SetVertices(
			new Vector2[] 
			{
			new Vector2(dimension, dimension), 
			new Vector2(-dimension, dimension), 
			new Vector2(-dimension, -dimension),
			new Vector2(dimension, -dimension)
			}
		);		
	}
	
	public void SetVertices(Vector2[] array) {

		vertices = new List<Vector2>(array);

//		CheckClockwise();
		if(!ForceEarCut.AreVerticesClockwise(vertices, 0, vertices.Count))
			vertices.Reverse ();

//		OnUpdate();
	}

//	public void CheckClockwise(){
//		if(!ForceEarCut.AreVerticesClockwise(vertices, 0, vertices.Count))
//			vertices.Reverse ();
//	}
//
//	public int verticesCount
//	{
//		get {
//			if(vertices == null)
//				return 0;
//			return vertices.Count;
//		}
//	}

	public int PreIndex(int index){
		if(vertices.Count == 0){
			Debug.LogError("Cannot return PreIndex, cos count == 0");
			return -1;
		}
		if(index > vertices.Count){
			Debug.LogError("Cannot return PreIndex, cos index > count");
			return -1;
		}
		return index == 0 ? vertices.Count - 1 : index - 1;
	}
	
	public int NextIndex(int index){
		if(vertices.Count == 0){
			Debug.LogError("Cannot return NextIndex, cos count == 0");
			return -1;
		}
		if(index > vertices.Count){
			Debug.LogError("Cannot return NextIndex, cos index > count");
			return -1;
		}
		return index == vertices.Count - 1 ? 0 : index + 1;
	}

	public Vector2 PrePosition(int index) {
		return vertices[PreIndex(index)];
	}

	public Vector2 NextPosition(int index) {
		return vertices[NextIndex (index)];
	}

	public Vector2 NextPosition(Vector2 vect){
		int index = vertices.LastIndexOf(vect);
		return NextPosition(index);
	}

	public Vector2 PrePosition(Vector2 vect){
		int index = vertices.IndexOf(vect);
		return PrePosition(index);
	}

}
