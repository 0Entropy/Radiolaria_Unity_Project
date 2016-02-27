using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[Flags]
public enum Join{ JOIN_BEVEL, JOIN_MITER, JOIN_ROUND }

public class Shape3DOutline : MonoBehaviour {



	public Join join;
	public float _strokeWidth;

	private PolygonTree _shape2D;
	public PolygonTree shape2D
	{
		set{
			_shape2D = value;
			OnUpdateMesh();
		}
	}

	private Facing _facing = Facing.FACE_BACK;
	public Facing facing{
		set
		{
			_facing = value;
		}
	}


	void OnDrawGizmos() {
		Gizmos.color = Color.yellow;
//		Gizmos.DrawSphere(transform.position, 1);
		if(_shape2D!=null){
			OnCalculateRect();
			Gizmos.DrawWireCube(transform.position + (Vector3)_rect.center, new Vector3(_rect.width, _rect.height));
//			Gizmos.
//			Gizmos.DrawSphere(transform.position +  
			Gizmos.DrawIcon(transform.position + (Vector3)_rect.center, "w, h : "+ _rect.width +", " + _rect.height);
		}
	}
	
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private Rect _rect;
	public Rect rect{
		get
		{
			if(_rect == null){
				_rect = new Rect();
				OnCalculateRect();
			}
			return _rect;
		}
	}
	
	public void OnCalculateRect()
	{
		float minX = 0, maxX = 0, minY = 0, maxY = 0;
		foreach(Vector2 v in _shape2D.exterior.vertices)
		{
			if(minX > v.x){
				minX = v.x;
			}
			if(maxX < v.x)
			{
				maxX = v.x;
			}
			if(minY > v.y)
			{
				minY = v.y;
			}
			if(maxY < v.y)
			{
				maxY = v.y;
			}
		}
		//						left, top, right, bottom
		_rect = Rect.MinMaxRect(minX, minY, maxX, maxY);
		//		_dimension = new Vector2(maxX - minX, maxY - minY);
	}
//	private MeshFilter _meshFilter;

	public void OnUpdateMesh(){

		List<Vector3> verts = new List<Vector3>();
		List<Vector2> uvs = new List<Vector2>();
		List<int> tris = new List<int>();

		if(join == Join.JOIN_BEVEL)
		{
			if(!ForceEarCut.AreVerticesClockwise(_shape2D.exterior.vertices, 0, _shape2D.exterior.vertices.Count))
			{
				_shape2D.exterior.vertices.Reverse ();
			}
			CalculateBevelOutline(_shape2D.exterior.vertices, _strokeWidth, ref verts, ref uvs, ref tris);

			foreach(Polygon inter in _shape2D.interiors)
			{
				if(ForceEarCut.AreVerticesClockwise(inter.vertices, 0, inter.vertices.Count))
				{
					inter.vertices.Reverse ();
				}
				CalculateBevelOutline(inter.vertices,  _strokeWidth, ref verts, ref uvs, ref tris);
			}
		}
		else if (join == Join.JOIN_MITER)
		{
			if(!ForceEarCut.AreVerticesClockwise(_shape2D.exterior.vertices, 0, _shape2D.exterior.vertices.Count))
			{
				_shape2D.exterior.vertices.Reverse ();
			}
			CalculateMeshData(_shape2D.exterior.vertices, _strokeWidth, ref verts, ref uvs, ref tris);
			
			foreach(Polygon inter in _shape2D.interiors)
			{
				if(ForceEarCut.AreVerticesClockwise(inter.vertices, 0, inter.vertices.Count))
				{
					inter.vertices.Reverse ();
				}
				CalculateMeshData(inter.vertices,  _strokeWidth, ref verts, ref uvs, ref tris);
			}
		}

		Mesh _mesh = GetComponent<MeshFilter>().sharedMesh;
		if (_mesh == null)
		{
			_mesh = new Mesh();
			_mesh.name = "Outline_Mesh";
			GetComponent<MeshFilter>().mesh = _mesh;
		}
		_mesh.Clear();
		_mesh.vertices = verts.ToArray();
		_mesh.uv = uvs.ToArray();
		_mesh.triangles = tris.ToArray();
		_mesh.RecalculateNormals();
		_mesh.Optimize();

		OnCalculateRect();
	}

	public void CalculateMeshData(List<Vector2> vertices, float strokeWidth, ref List<Vector3> verts, ref List<Vector2> uvs, ref List<int> tris)
	{
		int n = vertices.Count;
		
		int initIndex = verts.Count;
		
		int i = 0;
		int vertNum = initIndex;
		
		float pFactor = 0, nFactor = 0;
		Vector2 pVer = vertices[n-1];
		Vector2 cVer = vertices[0];
		Vector2 nVer = vertices[1];
		float pDis = Vector2.Distance(pVer, cVer);
		float nDis;// = Vector2.Distance(cVer, nVer);
		
		//		int sign = ForceEarCut.ComputeSpannedAreaSign(pVer, cVer, nVer);
		Vector2 firstSkewPoint = pVer;
		
		while(i<n+1){
			
			nDis = Vector2.Distance(cVer, nVer);
			if(nDis < strokeWidth){
				pDis = Vector2.Distance(pVer, nVer);
				i++;
				//				cVer = nVer;
				nVer = vertices[i%n];
				
				continue;
			}
			
			pFactor = pDis / strokeWidth;
			nFactor = nDis / strokeWidth;
			
			Vector2 perPC = CGAlgorithm.Perp(pVer, cVer, strokeWidth);
			Vector2 perCN = CGAlgorithm.Perp(cVer, nVer, strokeWidth);
			
//			int sign = ForceEarCut.ComputeSpannedAreaSign(pVer, cVer, nVer);
			//---------------------------------------------------------------------------
				
				
			Vector2 skewPoint = cVer + perPC;
			//CGAlgorithm.Intersect2D_2SkewSegments(pVer + perPC, cVer + perPC , cVer + perCN, nVer + perCN, ref skewPoint) ;
			Vector2 pcDir = (cVer - pVer).normalized * strokeWidth * 5;
			Vector2 cnDir = (cVer - nVer).normalized * strokeWidth * 5;
			if( CGAlgorithm.Intersect2D_2SkewSegments(
				pVer + perPC, cVer + perPC + pcDir, 
				cVer + perCN + cnDir, nVer + perCN , 
				ref skewPoint) )
			{

				if(vertNum == initIndex ) {
					firstSkewPoint = skewPoint;
				}else{
					verts[vertNum - 1] = skewPoint;
				}
				verts.Add (pVer);
				verts.Add (skewPoint);
				verts.Add (cVer);
				if(i == n){
					verts.Add (firstSkewPoint);
//					verts.Add (nVer + perCN);
				}else{
					verts.Add (nVer);//(nVer + perCN);
				}
				
				uvs.Add(new Vector2( 0, 0 ));
				uvs.Add(new Vector2( 1, pFactor ));
				uvs.Add(new Vector2( 0, pFactor ));
				
				uvs.Add(new Vector2( 1, pFactor + nFactor ));

				if(_facing == Facing.FACE_FORWARD)
				{
					tris.Add (vertNum );
					tris.Add (vertNum + 2);
					tris.Add (vertNum + 1);
					tris.Add (vertNum + 2);
					tris.Add (vertNum + 3);
					tris.Add (vertNum + 1);
				}
				else if(_facing == Facing.FACE_BACK)
				{
					tris.Add (vertNum );
					tris.Add (vertNum + 1);
					tris.Add (vertNum + 2);
					tris.Add (vertNum + 2);
					tris.Add (vertNum + 1);
					tris.Add (vertNum + 3);
				}
				
				vertNum += 4;
					
			}else{
				Debug.LogError("Outline .... no intersect???");
			}
			i++;
			pVer = cVer;
			cVer = nVer;
			nVer = vertices[i%n];
			pDis = nDis;
			
		}
		
		
	}

	public void CalculateBevelOutline(List<Vector2> vertices, float strokeWidth, ref List<Vector3> verts, ref List<Vector2> uvs, ref List<int> tris)
	{
		int n = vertices.Count;

		//List<Vector2> vertices = poly.vertices;//new List<Vector2>(poly.vertices);
//		if(!isInter && ForceEarCut.AreVerticesClockwise(vertices, 0, n))
//		{
//			vertices.Reverse ();
//		}
//		if(isInter && !ForceEarCut.AreVerticesClockwise(vertices, 0, n))
//		{
//			vertices.Reverse ();
//		}
//		poly.CheckClockwise();
//
//		List<Vector3> verts = new List<Vector3>();
//		List<Vector2> uvs = new List<Vector2>();
//		List<int> tris = new List<int>();

		int initIndex = verts.Count;

		int i = 0;
		int vertNum = initIndex;

		float pFactor = 0, nFactor = 0;
		Vector2 pVer = vertices[n-1];
		Vector2 cVer = vertices[0];
		Vector2 nVer = vertices[1];
		float pDis = Vector2.Distance(pVer, cVer);
		float nDis;// = Vector2.Distance(cVer, nVer);

//		int sign = ForceEarCut.ComputeSpannedAreaSign(pVer, cVer, nVer);
		Vector2 firstSkewPoint = pVer;

		Debug.LogError( gameObject.name + " is going on to enter while ... ! ");

		while(i<n+1){

			nDis = Vector2.Distance(cVer, nVer);
			if(nDis < strokeWidth){
				pDis = Vector2.Distance(pVer, nVer);
				i++;
//				cVer = nVer;
				nVer = vertices[i%n];

				continue;
			}

			pFactor = pDis / strokeWidth;
			nFactor = nDis / strokeWidth;

			Vector2 perPC = CGAlgorithm.Perp(pVer, cVer, strokeWidth);
			Vector2 perCN = CGAlgorithm.Perp(cVer, nVer, strokeWidth);

			int sign = ForceEarCut.ComputeSpannedAreaSign(pVer, cVer, nVer);
			if(sign < 0){

				//	sign < 0 :
				//		  p
				//		   \
				//			\
				//		=== c --> n ===
				
				Vector2 skewPoint = cVer + perPC;
				//CGAlgorithm.Intersect2D_2SkewSegments(pVer + perPC, cVer + perPC , cVer + perCN, nVer + perCN, ref skewPoint) ;
				if( CGAlgorithm.Intersect2D_2SkewSegments(pVer + perPC, cVer + perPC , cVer + perCN, nVer + perCN, ref skewPoint) )
				{
					if(vertNum > initIndex)
					{
						verts[vertNum - 1] = skewPoint;
					}


					verts.Add (pVer);
					verts.Add (skewPoint);
					verts.Add (cVer);
					verts.Add (nVer + perCN);
					
					uvs.Add(new Vector2( 0, 0 ));
					uvs.Add(new Vector2( 1, pFactor ));
					uvs.Add(new Vector2( 0, pFactor ));
					uvs.Add(new Vector2( 1, pFactor + nFactor ));
					
					if(_facing == Facing.FACE_FORWARD)
					{
						tris.Add (vertNum );
						tris.Add (vertNum + 2);
						tris.Add (vertNum + 1);
						tris.Add (vertNum + 2);
						tris.Add (vertNum + 3);
						tris.Add (vertNum + 1);
					}
					else if(_facing == Facing.FACE_BACK)
					{
						tris.Add (vertNum );
						tris.Add (vertNum + 1);
						tris.Add (vertNum + 2);
						tris.Add (vertNum + 2);
						tris.Add (vertNum + 1);
						tris.Add (vertNum + 3);
					}
					
					vertNum += 4;


				}

			}else{
				
				// or sign > 0 :
				//					 p
				//		 			/
				//				   /
				//		=== n <-- c ===
				//
				

				//				}else{
				//---------------------------------------------------------------------------

				verts.Add (pVer);
				verts.Add (cVer + perPC);
				verts.Add (cVer);
				verts.Add (cVer + perCN);
				verts.Add (nVer + perCN);
				
				uvs.Add(new Vector2( 0, 0 ));
				uvs.Add(new Vector2( 1, pFactor ));
				uvs.Add(new Vector2( 0, pFactor ));
				uvs.Add(new Vector2( 1, pFactor ));
				uvs.Add(new Vector2( 1, pFactor + nFactor ));

				if(_facing == Facing.FACE_FORWARD)
				{
					tris.Add (vertNum);
					tris.Add (vertNum + 2);
					tris.Add (vertNum + 1);
					tris.Add (vertNum + 2);
					tris.Add (vertNum + 3);
					tris.Add (vertNum + 1);
					tris.Add (vertNum + 2);
					tris.Add (vertNum + 4);
					tris.Add (vertNum + 3);
				}
				else if(_facing == Facing.FACE_BACK)
				{
					tris.Add (vertNum);
					tris.Add (vertNum + 1);
					tris.Add (vertNum + 2);
					tris.Add (vertNum + 2);
					tris.Add (vertNum + 1);
					tris.Add (vertNum + 3);
					tris.Add (vertNum + 2);
					tris.Add (vertNum + 3);
					tris.Add (vertNum + 4);
				}
				vertNum += 5;

			}
			i++;
			pVer = cVer;
			cVer = nVer;
			nVer = vertices[i%n];
			pDis = nDis;

		}


	}

}
