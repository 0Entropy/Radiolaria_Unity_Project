using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Shape3DSide : MonoBehaviour {

	private PolygonTree _shape2D;
	public PolygonTree shape2D
	{
		set
		{
			_shape2D = value;
			OnUpdateMesh();
		}
	}
	
//	public Face face;
	private float _thickness = 0.02F;
	public float thickness{
		set
		{
			_thickness = value * 0.5F ;
			if(_shape2D != null)
			{
				OnUpdateMesh();
			}
		}
	}
	
	public void OnUpdateMesh(){
		
		List<Vector3> verts = new List<Vector3>();
		List<Vector2> uvs = new List<Vector2>();
		List<int> tris = new List<int>();
//				
//		float halfThickness = _thickness * 0.5F;
		
		CalculateMeshData(_shape2D.exterior.vertices, _thickness, ref verts, ref uvs, ref tris);
		
		foreach(Polygon inPolygon in _shape2D.interiors){

			if(ForceEarCut.AreVerticesClockwise(inPolygon.vertices, 0, inPolygon.vertices.Count))
				inPolygon.vertices.Reverse ();

			CalculateMeshData(inPolygon.vertices, _thickness, ref verts, ref uvs, ref tris);
		}
		
		Mesh _mesh = GetComponent<MeshFilter>().sharedMesh;
		if (_mesh == null)
		{
			_mesh = new Mesh();
			_mesh.name = "Side_Mesh";
			GetComponent<MeshFilter>().mesh = _mesh;
		}
		_mesh.Clear();
		_mesh.vertices = verts.ToArray();
		_mesh.uv = uvs.ToArray();
		_mesh.triangles = tris.ToArray();
		_mesh.RecalculateNormals();
		;
	}
	
	public static void CalculateMeshData(List<Vector2> points, float offset, ref List<Vector3> vertices, ref List<Vector2> uv, ref List<int> triangles){
		
		int count = points.Count;
		int offsetIndex = vertices.Count;
		Vector3 offsetPos = new Vector3(0, 0, offset);
		float shapeDepth = offset * 2.0F;//Mathf.Abs(offset) * 2.0F;
		
		for (int i = 0; i < count; i++)
		{
			//  + offsetPos		1'b-----------------------0'a
			//  |				|							|
			//  i+1 <--- i		|							|
			//  |				|							|
			//  - offsetPos		2'c-----------------------3'd
			//
			
			Vector3 p0 = (Vector3)points[i];
			Vector3 p1 = (Vector3)points[(i + 1)%count];
			
			vertices.Add (p0 + offsetPos);
			vertices.Add (p1 + offsetPos);
			vertices.Add (p1 - offsetPos);
			vertices.Add (p0 - offsetPos);
			
			float dis = Vector3.Distance(p0, p1);
			float f = dis / shapeDepth;
			
			uv.Add(new Vector2(0, 0 ));
			uv.Add(new Vector2(0, f ));
			uv.Add(new Vector2(1, f ));
			uv.Add(new Vector2(1, 0 ));
		}
		
		//Build triangles array
		for (int a = 0, n = count * 4; a < n; a += 4)
		{
			var b = (a + 1) % n;
			var c = (a + 2) % n;
			var d = (a + 3) % n;
			triangles.Add(offsetIndex + a);
			triangles.Add(offsetIndex + b);
			triangles.Add(offsetIndex + c);
			triangles.Add(offsetIndex + a);
			triangles.Add(offsetIndex + c);
			triangles.Add(offsetIndex + d);
		}
	}

}
