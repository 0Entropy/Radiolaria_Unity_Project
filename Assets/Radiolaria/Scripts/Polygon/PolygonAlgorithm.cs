using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public class PolygonAlgorithm{

	//A:	(=|=|
	//B:	  |=|=)
	//OR:	(=|=|=)
	//AND:	  |=| 
	//NOT:	(=|
	//XOR:	(=| |=)

	private static Vector2 RIGHT_DISTANCE = new Vector2(5.0F, 0.0F);



	public enum OPERATION{NONE, OR, AND, NOT, XOR};

    public static List<Vector2> Merge(List<List<Vector2>> allPolys)
    {
        List<Vector2> merge = allPolys[0];
        allPolys.RemoveAt(0);

        int maxCount = 100;

        while (allPolys.Count > 0)
        {

            var removableList = new List<List<Vector2>>();

            foreach (var currect in allPolys)
            {
                List<Vector2> result;
                if (Operate(merge, currect, out result, OPERATION.OR))
                {
                    merge = result;
                    removableList.Add(currect);
                }
            }

            foreach (var r in removableList)
                allPolys.Remove(r);

            if (--maxCount < 0)
                break;
        }

        return merge;
    }
    
	public static bool Operate(List<Vector2> subjPoly, List<Vector2> operPoly, out List<Vector2> result, OPERATION operation = OPERATION.OR)
    {

        result = new List<Vector2>();

		int sn = subjPoly.Count;
		int on = operPoly.Count;
		
		if(!ForceEarCut.AreVerticesClockwise(subjPoly, 0, sn)){
			subjPoly.Reverse();
		}


        //当union == UNION.NOT时，operPoly 应该是逆时针方向。
        if (operation == OPERATION.NOT){
			if(ForceEarCut.AreVerticesClockwise(operPoly, 0, on)){
				operPoly.Reverse();
			}
		}else{
			if(!ForceEarCut.AreVerticesClockwise(operPoly, 0, on)){
				operPoly.Reverse();
			}
		}
		
		List<VertexNode> entering = new List<VertexNode>();
		
		List<VertexNode> subjList = new List<VertexNode>();
		List<VertexNode> operList = new List<VertexNode>();
		
		VertexNode.FillNodeList(subjList, subjPoly);
		VertexNode.FillNodeList(operList, operPoly);
		
		int oi, oj, si, sj;
		
		InterFactor factor;// = InterFactor.none;
		
		for( si = 0, sj = sn - 1; si < sn; sj = si++){
			for(oi = 0, oj = on - 1; oi < on; oj = oi++){
				
				int nInter = CGAlgorithm.IntersectFactor(subjPoly[sj], subjPoly[si], operPoly[oj], operPoly[oi], out factor);
				if(nInter == 1 ){
					VertexNode subjNode = new VertexNode(subjList, sj, subjPoly, factor.t, factor.inbound);
					VertexNode operNode = new VertexNode(operList, oj, operPoly, factor.u, factor.inbound);
					subjNode.Other = operNode;
					subjList.Add (subjNode);
					operList.Add (operNode);
					if(operation == OPERATION.NOT){
						if(!factor.inbound){
							entering.Add (operNode);
						}
					}else if(operation == OPERATION.OR){
						if(factor.inbound){
							entering.Add (operNode);
						}
					}else if(operation == OPERATION.AND){
						if(factor.inbound){
							entering.Add (subjNode);
						}
					}
				}
				
			}
		}
        
		if(entering.Count == 0){

            return false;

		}
		
		subjList.Sort();
		operList.Sort();
		
		while(entering.Count > 0){

			VertexNode nextNode = entering[0];
			entering.RemoveAt(0);
			
			VertexNode startNode = nextNode;
			
			result.Add (nextNode.Vlaue);
			nextNode = nextNode.Next;
			while(nextNode != startNode && nextNode.Other != startNode){
				result.Add(nextNode.Vlaue);
				if(nextNode.Kind == VerType.Intersection){
					if(nextNode.inbound){
						if(entering.Contains(nextNode))
							entering.Remove(nextNode);
						else if(entering.Contains(nextNode.Other))
							entering.Remove(nextNode.Other);
					}
					nextNode = nextNode.Other.Next;
				}else{
					nextNode = nextNode.Next;
				}
			}
            
		}
		
		
		return true;
	}
	
	#region combine
	public static List<Vector2> Combine(PolygonTree tree)
	{
		List<Vector2> exVertices = new List<Vector2>(tree.exterior.vertices);
		if(!ForceEarCut.AreVerticesClockwise(exVertices, 0, exVertices.Count))
			exVertices.Reverse ();

		if(tree.interiors == null || tree.interiors.Count == 0){
			return exVertices;
		}

		List<List<Vector2>> inPolygons = new List<List<Vector2>>(tree.interiors.Count);
		foreach(Polygon inPoly in tree.interiors)
		{
			List<Vector2> verts = new List<Vector2>(inPoly.vertices);
			if(!ForceEarCut.AreVerticesClockwise(verts, 0, verts.Count))
				verts.Reverse ();
			inPolygons.Add(verts);
		}
		return Combine(exVertices, inPolygons);
	}



	public static List<Vector2> Combine(List<Vector2> exVertices, List<List<Vector2>> inPolygons){
		//		inP.Clear();
		//		exP.Clear();
		
		List<Vector2> inP = new List<Vector2>();
		List<Vector2> exP = new List<Vector2>();
		
		//List<Vector2> exVertices = new List<Vector2>(tree.exterior.vertices);//new List<Vector2>(poly.exteriorRing.vertices);
		
//		List<Polygon> inPolygons = new List<Polygon>(tree.interiors);
//		List<Vector2> inVertices = new List<Vector2>();
//		foreach(Polygon poly in inPolygons){
//			inVertices.AddRange(poly.vertices);
//		}
		
		
		while(inPolygons.Count>0){
			
			
			Vector2 inVertex = inPolygons[0][0];
			int index = 0;
			for(int i = 0; i < inPolygons.Count; i++){
				if(GetRightMostVertex(inPolygons[i], ref inVertex)){
					index = i;
				}
			}
			
			Intersection inter = Intersection.none;
			
			if(CGAlgorithm.SegmentSkewPoly(inVertex, inVertex + RIGHT_DISTANCE, exVertices, ref inter)){
				
				if(inter.Type == PointType.ON_EDGE){
					
					exVertices.Insert(inter.EdgeEndIndex, inter.Point0);
					
				}
				Combine(ref exVertices, ref inP, ref exP, inPolygons[index], inter.Point0, inVertex);
			}
			
			else
			{
				Debug.LogError("The " + index +" interior polygon cannot combine to exterior polygon !");
				
			}
			
			inPolygons.RemoveAt(index);
			
			
		}

		if(!ForceEarCut.AreVerticesClockwise(exVertices, 0, exVertices.Count))
			exVertices.Reverse ();

		return exVertices;
	}
	
	private static void Combine(ref List<Vector2> exRing, ref List<Vector2> inP, ref List<Vector2> exP,  List<Vector2> inRing, Vector2 exVertex, Vector2 inVertex){
		
		inP.Add (inVertex);
		exP.Add (exVertex);
		
		List<Vector2> inTemp = new List<Vector2>(inRing);
		inTemp.Reverse ();
		
		int inCount = inTemp.Count;
		int exCount = exRing.Count;
		
		int inIndex = inTemp.IndexOf(inVertex);
		int exIndex = exRing.IndexOf(exVertex);
		
		//		List<Vector2> result = new List<Vector2>();
		//		result.AddRange(exRing.GetRange(0, exIndex + 1));
		//		
		//		result.AddRange(inTemp.GetRange(inIndex, inCount - inIndex));
		//		result.AddRange(inTemp.GetRange(0, inIndex + 1));
		//		
		//		result.AddRange(exRing.GetRange(exIndex, exCount - exIndex));
		
		exRing.InsertRange(exIndex + 1, inTemp.GetRange(inIndex, inCount - inIndex));
		exRing.InsertRange(exIndex + 1 + inCount - inIndex, inTemp.GetRange(0, inIndex + 1));
		exRing.Insert(exIndex + 2 + inCount, exVertex);
		
		//		return result;
	}
	
	private static bool GetRightMostVertex(List<Vector2> vertices, ref Vector2 vertex){
		
		bool result = false;
		foreach(Vector2 ver in vertices){
			if( vertex.x < ver.x ){
				vertex = ver;
				result = true;
			}
		}
		
		return result;
	}
	#endregion
}

public enum VerType{
	None,
	Polygon, 
	Intersection
}


public struct InterFactor {
	public float t;
	public float u;
	public bool inbound;

	public static InterFactor none = new InterFactor(){t = 0.0F, u = 0.0F, inbound = false};
}

public class VertexNode : IComparable<VertexNode>{

	private List<VertexNode> parentList;

	public VerType Kind;

	public float t;
	public bool inbound;

	private VertexNode _other;
	public VertexNode Other{
		set{ 
			if(_other != value)
				_other = value;
			if(value.Other != this)
				value.Other = this;
		}
		get {
			return _other;
		}
	}

	public VertexNode Next{
		get { 
			int i = parentList.IndexOf(this);
			return parentList[(i + 1) % parentList.Count];
		}
	}

	public int originIndex;
	public List<Vector2> originVertices;

	private Vector2 _value;
	public Vector2 Vlaue{
		set
		{
			_value = value;
		}
		get 
		{
			switch(Kind){
			case VerType.None:
				return _value;
				break;
			case VerType.Polygon:
				return originVertices[originIndex];
				break;
			case VerType.Intersection:
				int nextIndex = (originIndex + 1) % originVertices.Count;
				Vector2 v = originVertices[nextIndex] - originVertices[originIndex];
				return originVertices[originIndex] + v * t;
				break;
			default:
				return Vector2.zero;
				break;
			}
		}
	}

	public static void FillNodeList(List<VertexNode> nodeList, List<Vector2> verts) {
		foreach(Vector2 v in verts){
			nodeList.Add (new VertexNode(nodeList, verts.IndexOf(v), verts));
		}
	}

	public VertexNode(List<VertexNode> parent, int index, List<Vector2> verts){
		parentList = parent;

		Kind = VerType.Polygon;

		t = 0.0F;
		inbound = false;
		_other = null;

		originIndex = index;
		originVertices = verts;
	}

	public VertexNode(List<VertexNode> parent, int index, List<Vector2> verts, float factor, bool isEnter){
		parentList = parent;

		Kind = VerType.Intersection;

		t = factor;
		inbound = isEnter;
		_other = null;

		originIndex = index;
		originVertices = verts;
	}

	public int CompareTo(VertexNode other) {
		return (t + originIndex).CompareTo(other.t + other.originIndex);
	}
}
