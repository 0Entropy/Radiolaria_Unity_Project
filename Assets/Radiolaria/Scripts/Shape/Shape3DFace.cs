using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[Flags]
public enum Facing{ 
	DOUBLE_FACE 	= 3, 
	FACE_BACK 		= 1, 
	FACE_FORWARD 	= 2 
}

public class Shape3DFace : MonoBehaviour {

	private PolygonTree _shape2D;
	public PolygonTree shape2D
	{
		set
		{
			_shape2D = value;
			OnUpdateMesh();
		}
		get{
			return _shape2D;
		}
	}

	public Facing facing;

	private float _thickness = 0.02F;
	public float thickness{
		set
		{
			_thickness = value * 0.5F;
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

		List<Vector2> points = PolygonAlgorithm.Combine(_shape2D);
		List<int> triangles = ForceEarCut.ComputeTriangles(points);

		if((facing & Facing.FACE_BACK) == Facing.FACE_BACK)
			CalculateMeshData(points, triangles, Facing.FACE_BACK, _thickness, ref verts, ref uvs, ref tris);

		if((facing & Facing.FACE_FORWARD) == Facing.FACE_FORWARD)
			CalculateMeshData(points, triangles, Facing.FACE_FORWARD, _thickness, ref verts, ref uvs, ref tris);

		Mesh _mesh = GetComponent<MeshFilter>().sharedMesh;
		if (_mesh == null)
		{
			_mesh = new Mesh();
			_mesh.name = "Face_Mesh";
			GetComponent<MeshFilter>().mesh = _mesh;
		}
		_mesh.Clear();
		_mesh.vertices = verts.ToArray();
		_mesh.uv = uvs.ToArray();
		_mesh.triangles = tris.ToArray();
		_mesh.RecalculateNormals();
		_mesh.Optimize();
	}

	public static void CalculateMeshData(List<Vector2> points, List<int> triangles, Facing facing, float thickness, ref List<Vector3> verts, ref List<Vector2> uvs, ref List<int> tris) {
		
		
		int count = points.Count;
		int offsetIndex = verts.Count;
		float thick = facing == Facing.FACE_FORWARD ? thickness : -thickness;

		for (int i = 0; i < count; i++)
		{
			Vector2 pt = points[i];
			verts.Add((Vector3)pt + Vector3.forward * thick);
			
			uvs.Add (new Vector2(pt.x, pt.y));
			
		}
		
		for(int i = 0; i < triangles.Count; i += 3) {
			if(facing == Facing.FACE_FORWARD)
			{
				tris.Add (offsetIndex + triangles[i]);
				tris.Add (offsetIndex + triangles[i+2]);
				tris.Add (offsetIndex + triangles[i+1]);
			}
			else if(facing == Facing.FACE_BACK)
			{
				tris.Add (offsetIndex + triangles[i] );
				tris.Add (offsetIndex + triangles[i+1]);
				tris.Add (offsetIndex + triangles[i+2]);
			}
		}
	}
}
