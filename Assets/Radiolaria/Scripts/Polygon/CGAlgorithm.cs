using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class CGAlgorithm{
/*===========================================================================================
* http://geomalgorithms.com/a03-_inclusion.html#wn_PnPoly()

Determining the inclusion of a point P in a 2D planar polygon is a geometric problem 
that results in interesting algorithms. Two commonly used methods are:
===========================================================================================
I. The Crossing Number (cn) method
- which counts the number of times a ray starting from the point P crosses the polygon boundary edges. 
The point is outside when this "crossing number" is even; 
otherwise, when it is odd, the point is inside. 
This method is sometimes referred to as the "even-odd" test.

Edge Crossing Rules:
	1.an upward edge includes its starting endpoint, and excludes its final endpoint;
	2.a downward edge excludes its starting endpoint, and includes its final endpoint;
	3.horizontal edges are excluded
	4.the edge-ray intersection point must be strictly right of the point P.
--------------------------------------------------------------------------------

Pseudo-Code: Crossing # Inclusion

Code for this algorithm is well-known, and the edge crossing rules are easily expressed. 
For a polygon represented as an array V[n+1] of vertex points with V[n]=V[0], popular 
implementation logic ([Franklin, 2000], [O'Rourke, 1998]) is as follows:

--------------------------------------------------------------------------------
typedef struct {int x, y;} Point;

cn_PnPoly( Point P, Point V[], int n )
{
    int    cn = 0;    // the  crossing number counter

    // loop through all edges of the polygon
    for (each edge E[i]:V[i]V[i+1] of the polygon) {
        if (E[i] crosses upward ala Rule #1
         || E[i] crosses downward ala  Rule #2) {
            if (P.x <  x_intersect of E[i] with y=P.y)   // Rule #4
                 ++cn;   // a valid crossing to the right of P.x
        }
    }
    return (cn&1);    // 0 if even (out), and 1 if  odd (in)

}
--------------------------------------------------------------------------------
Note that the tests for upward and downward crossing satisfying Rules #1 and #2 
also exclude horizontal edges (Rule #3). All-in-all, a lot of work is done by 
just a few tests which makes this an elegant algorithm.
===========================================================================================

II. The Winding Number (wn) method
- which counts the number of times the polygon winds around the point P. 
The point is outside only when this "winding number" wn = 0; 
otherwise, the point is inside.
--------------------------------------------------------------------------------

Pseudo-Code: Winding Number Inclusion

This results in the following wn algorithm which is an adaptation of the cn algorithm 
and uses the same edge crossing rules as before to handle special cases.
--------------------------------------------------------------------------------

typedef struct {int x, y;} Point;

wn_PnPoly( Point P, Point V[], int n )
{
    int    wn = 0;    // the  winding number counter

    // loop through all edges of the polygon
    for (each edge E[i]:V[i]V[i+1] of the polygon) {
        if (E[i] crosses upward ala Rule #1)  {
            if (P is  strictly left of E[i])    // Rule #4
                 ++wn;   // a valid up intersect right of P.x
        }
        else
        if (E[i] crosses downward ala Rule  #2) {
            if (P is  strictly right of E[i])   // Rule #4
                 --wn;   // a valid down intersect right of P.x
        }
    }
    return wn;    // =0 <=> P is outside the polygon

}
--------------------------------------------------------------------------------

Clearly, this winding number algorithm has the same efficiency 
as the analogous crossing number algorithm. Thus, since it is more accurate in general, 
the winding number algorithm should always be the preferred method to determine inclusion 
of a point in an arbitrary polygon.
===========================================================================================
* */
	
/* http://en.wikipedia.org/wiki/Even-odd_rule
	 # x, y -- x and y coordinates of point
	 # a list of tuples [(x, y), (x, y), ...]
	 def isPointInPath(x, y, poly):
        num = len(poly)
        i = 0
        j = num - 1
        c = False
        for i in range(num):
                if  ((poly[i][1] > y) != (poly[j][1] > y)) and \
                        (x < (poly[j][0] - poly[i][0]) * (y - poly[i][1]) / (poly[j][1] - poly[i][1]) + poly[i][0]):
                    c = not c
                j = i
        return c
* */
	
	//isLeft(): tests if a point is Left|On|Right of an infinite line.
	//    Input:  three points P0, P1, and P2
	//    Return: >0 for P2 left of the line through P0 and P1
	//            =0 for P2  on the line
	//            <0 for P2  right of the line
	//    See: Algorithm 1 "Area of Triangles and Polygons"
	//    http://geomalgorithms.com/a01-_area.html
		

	
	// cn_PnPoly(): crossing number test for a point in a polygon
	//      Input:   P = a point,
	//               V[] = vertex points of a polygon V[n+1] with V[n]=V[0]
	//      Return:  0 = outside, 1 = inside
	// This code is patterned after [Franklin, 2000]
	
	public static int CN_PnPoly (Vector2 P, Vector2[] V) {
		
		int n = V.Length;
		int i , j;
		int cn = 0;    // the  crossing number counter

	    // loop through all edges of the polygon
	    for (i=0, j = n - 1; i<n; j = i++) {    // edge from V[i]  to V[i+1]
	       if (((V[j].y <= P.y) && (V[i].y > P.y))     // an upward crossing
	        || ((V[j].y > P.y) && (V[i].y <=  P.y))) { // a downward crossing
	            // compute  the actual edge-ray intersect x-coordinate
	            float vt = (float)(P.y  - V[j].y) / (V[i].y - V[j].y);
	            if (P.x <  V[j].x + vt * (V[i].x - V[j].x)) // P.x < intersect
	                 ++cn;   // a valid crossing of y=P.y right of P.x
				j = i;
	        }
	    }
	    return (cn&1);    // 0 if even (out); and 1 if odd (in).
	}
	
	// WN_PnPoly(): winding number test for a point in a polygon
	//      Input:   P = a point,
	//               V[] = vertex points of a polygon V[n+1] with V[n]=V[0]
	//      Return:  wn = the winding number (=0 only when P is outside)
	
	public static int WN_PnPoly (Vector2 P, Vector2[] V) {
		int n = V.Length;
		int i, j ;
		int wn = 0;    						// the  winding number counter

    	// loop through all edges of the polygon
	    for (i=0, j = n - 1; i<n; j = i++) {   			// edge from V[i] to  V[i+1]
	        if (V[j].y <= P.y) {          	// start y <= P.y
	            if (V[i].y  > P.y)      	// an upward crossing
	                 if (IsLeft( V[j], V[i], P) > 0)  // P left of  edge
	                     ++wn;            	// have  a valid up intersect
	        } else {                        // start y > P.y (no test needed)
	            if (V[i].y  <= P.y)    	// a downward crossing
	                 if (IsLeft( V[j], V[i], P) < 0)  // P right of  edge
	                     --wn;            	// have  a valid down intersect
	        }
			j = i;
	    }
	    return wn;							// = 0, out; != 0, in.
	}
	
	public static bool WN_PnPoly (Vector2 P, List<Vector2> V) {
		int n = V.Count;
		int i, j;
		int wn = 0;    						// the  winding number counter

    	// loop through all edges of the polygon
	    for (i=0, j = n - 1; i<n; j = i++) {   			// edge from V[i] to  V[i+1]
	        if (V[j].y <= P.y) {          	// start y <= P.y
	            if (V[i].y  > P.y)      	// an upward crossing
	                 if (IsLeft( V[j], V[i], P) > 0)  // P left of  edge
	                     ++wn;            	// have  a valid up intersect
	        } else {                        // start y > P.y (no test needed)
	            if (V[i].y  <= P.y)    	// a downward crossing
	                 if (IsLeft( V[j], V[i], P) < 0)  // P right of  edge
	                     --wn;            	// have  a valid down intersect
	        }
			j = i;
	    }
	    return wn != 0;							// = 0, out; != 0, in.
	}
	
	//	inline int
	public static float IsLeft( Vector2 P0, Vector2 P1, Vector2 P2 )
	{
	    return ( (P1.x - P0.x) * (P2.y - P0.y)
	            - (P2.x -  P0.x) * (P1.y - P0.y) );
	}
	/* ===========================================================================================
	 * Intersections of Lines and Planes	
	 * http://geomalgorithms.com/a05-_intersect-1.html
	 * */
	//===========================================================================================
	// Line and Segment Intersections
	//
	// Assume that classes are already given for the objects:
	//    Point and Vector with
	//        coordinates {float x, y, z;}
	//        operators for:
	//            == to test  equality
	//            != to test  inequality
	//            Point   = Point ± Vector
	//            Vector =  Point - Point
	//            Vector =  Scalar * Vector    (scalar product)
	//            Vector =  Vector * Vector    (3D cross product)
	//    Line and Ray and Segment with defining  points {Point P0, P1;}
	//        (a Line is infinite, Rays and  Segments start at P0)
	//        (a Ray extends beyond P1, but a  Segment ends at P1)
	//    Plane with a point and a normal {Point V0; Vector  n;}
	//===========================================================================================
	
	//#define SMALL_NUM   0.00000001 // anything that avoids division overflow
	private const float SMALL_NUM = 0.00000001F;
	private const float VECTEX_ROUND = 0.000001F; 
	
	// dot product (3D) which allows vector operations in arguments
	//#define dot(u,v)   ((u).x * (v).x + (u).y * (v).y + (u).z * (v).z)
	// 點積運算
	// 點積的結果為向量u在向量v上垂直投影的某種量。
	// 同一向量的Dot可以理解为其刚量（长度）平方。
	// 所以，也可以運用dot來計算距離。但是沒有多大好處。
	private static float Dot(Vector3 u, Vector3 v) {
		return (u.x * v.x + u.y * v.y + u.z * v.z);
	}
	
	public static float Dot(Vector2 u, Vector2 v) {
		return (u.x * v.x + u.y * v.y);
	}
	
	// 向量p01與向量p02進行點積，判斷∠102之大小。
	// > 0 時，兩向量夾角小於 90˚ and >= 0˚；
	// = 0 時，夾角等於 90˚ ，两向量垂直；
	// < 0 時，夾角 > 90˚ 且 <= 180˚ 。	
	public static float Dot( Vector2 p0, Vector2 p1, Vector2 p2 ) {
		//return Dot(p1-p0, p2-p0);
		return (p1.x-p0.x) * (p2.x-p0.x) + (p1.y-p0.y) * (p2.y-p0.y);
	}
	
	public static float Dot(Vector2 u) {
		return (u.x*u.x + u.y*u.y);
	}
	
	// 叉積運算，回傳純量（除去方向）
	// 叉積的結果為平行四邊形的面積量。
	// 叉積为0时，可以理解为两向量平行。
	// 另，点到直线垂直距离也是运用叉积计算。叉積求出 v1 v2 組成的平行四邊形面積，
	// 然後除以 v2 的長度，便是垂直距離。叉積可能會有負值，請記得取絕對值，
	// 才不會得到負的距離。
	//（点与线段距离因点位置区域，最小距离可能是点至线段端点距离）
	// #define perp(u,v)  ((u).x * (v).y - (u).y * (v).x)  // perp product  (2D)
 	public static float Perp(Vector2 u, Vector2 v) {
		return (u.x * v.y - u.y * v.x);
	}
	
	// 向量p01與向量p02進行叉積，判斷p01到p02的旋轉方向。
	// > 0 時，兩向量前後順序為逆時針順序（在 180˚ 之內）；
	// = 0 時，兩向量平行，也就是指夾角等於 0˚ 或 180˚ ；
	// < 0 時，兩向量前後順序為順時針順序（在 180˚ 之內）。
	public static float Perp( Vector2 p0, Vector2 p1, Vector2 p2 ) {
		//return Perp(p1-p0, p2-p0);
		return (p1.x-p0.x) * (p2.y-p0.y) - (p1.y-p0.y) * (p2.x-p0.x);
	}

	public static Vector2 Perp(Vector2 p1, Vector2 p2, float width)
	{
		//(p2 - p1) 方向左手垂线
		Vector2 v1, v2;
		
		v1.x = p2.y; v1.y = p1.x;
		v2.x = p1.y; v2.y = p2.x;
		Vector2 perpendicular = v1 - v2;
		float normalizedDistance = ( 1.0f / Mathf.Sqrt ((perpendicular.x * perpendicular.x) + (perpendicular.y * perpendicular.y)) );
		perpendicular *= normalizedDistance * width;
		return perpendicular;
	}

	// intersect2D_2Segments(): find the 2D intersection of 2 finite segments
	//    Input:  two finite segments S1 and S2
	//    Output: *I0 = intersect point (when it exists)
	//            *I1 =  endpoint of intersect segment [I0,I1] (when it exists)
	//    Return: 0=disjoint (no intersect)
	//            1=intersect  in unique point I0
	//            2=overlap  in segment from I0 to I1
	public static int Intersect2D_2Segments( Segment2D S1, Segment2D S2, Vector2 I0, Vector2 I1 ) {
		Vector2   u = S1.P1 - S1.P0;
	    Vector2   v = S2.P1 - S2.P0;
	    Vector2   w = S1.P0 - S2.P0;
	    float     D = Perp(u,v);
	
	    // test if  they are parallel (includes either being a point)
	    if (Mathf.Abs(D) < SMALL_NUM) {      // S1 and S2 are parallel
	        if (Perp(u,w) != 0 || Perp(v,w) != 0)  {
	            return 0;                    // they are NOT collinear
	        }
	        // they are collinear or degenerate
	        // check if they are degenerate  points
	        float du = Dot(u,u);
	        float dv = Dot(v,v);
	        if (du==0 && dv==0) {            // both segments are points
	            if (S1.P0 !=  S2.P0)         // they are distinct  points
	                 return 0;
	            I0 = S1.P0;                 // they are the same point
	            return 1;
	        }
	        if (du==0) {                     // S1 is a single point
	            if  (InSegment(S1.P0, S2) == 0)  // but is not in S2
	                 return 0;
	            I0 = S1.P0;
	            return 1;
	        }
	        if (dv==0) {                     // S2 a single point
	            if  (InSegment(S2.P0, S1) == 0)  // but is not in S1
	                 return 0;
	            I0 = S2.P0;
	            return 1;
	        }
	        // they are collinear segments - get  overlap (or not)
	        float t0, t1;                    // endpoints of S1 in eqn for S2
	        Vector2 w2 = S1.P1 - S2.P0;
	        if (v.x != 0) {
	                 t0 = w.x / v.x;
	                 t1 = w2.x / v.x;
	        } else {
	                 t0 = w.y / v.y;
	                 t1 = w2.y / v.y;
	        }
	        if (t0 > t1) {                   // must have t0 smaller than t1
	                 float t=t0; t0=t1; t1=t;    // swap if not
	        }
	        if (t0 > 1 || t1 < 0) {
	            return 0;      // NO overlap
	        }
	        t0 = t0<0 ? 0 : t0;               // clip to min 0
	        t1 = t1>1 ? 1 : t1;               // clip to max 1
	        if (t0 == t1) {                   // intersect is a point
	            I0 = S2.P0 +  t0 * v;
	            return 1;
	        }
	
	        // they overlap in a valid subsegment
	        I0 = S2.P0 + t0 * v;
	        I1 = S2.P0 + t1 * v;
	        return 2;
	    }//if (Mathf.Abs(D) < SMALL_NUM) {...
	
	    // the segments are skew and may intersect in a point
	    // get the intersect parameter for S1
	    float     sI = Perp(v,w) / D;
	    if (sI < 0 || sI > 1)                // no intersect with S1
	        return 0;
	
	    // get the intersect parameter for S2
	    float     tI = Perp(u,w) / D;
	    if (tI < 0 || tI > 1)                // no intersect with S2
	        return 0;
	
	    I0 = S1.P0 + sI * u;                // compute S1 intersect point
	    return 1;
	}
	
  //public static int Intersect2D_2Segments( Segment2D S1, Segment2D S2, Vector2 I0, Vector2 I1 ) {
	public static int Intersect2D_2Segments( Vector2 P0, Vector2 P1, Vector2 P2, Vector2 P3, Vector2 I0, Vector2 I1 ) {
		Vector2   u = P1 - P0;
	    Vector2   v = P3 - P2;
	    Vector2   w = P0 - P2;
	    float     D = Perp(u,v);
	
	    // test if  they are parallel (includes either being a point)
	    if (Mathf.Abs(D) < SMALL_NUM) {      // S1 and S2 are parallel
	        if (Perp(u,w) != 0 || Perp(v,w) != 0)  {
	            return 0;                    // they are NOT collinear
	        }
	        // they are collinear or degenerate
	        // check if they are degenerate  points
	        float du = Dot(u,u);
	        float dv = Dot(v,v);
	        if (du==0 && dv==0) {            // both segments are points
	            if (P0 !=  P2)         // they are distinct  points
	                 return 0;
	            I0 = P0;                 // they are the same point
	            return 1;
	        }
	        if (du==0) {                     // S1 is a single point
	            if  (!InSegment(P0, P2, P3))  // but is not in S2
	                 return 0;
	            I0 = P0;
	            return 1;
	        }
	        if (dv==0) {                     // S2 a single point
	            if  (!InSegment(P2, P0, P1))  // but is not in S1
	                 return 0;
	            I0 = P2;
	            return 1;
	        }
			// 两线段共线 - 求重叠（ or not ）
	        // they are collinear segments - get  overlap (or not)
	        float t0, t1;                    // endpoints of S1 in eqn for S2
	        Vector2 w2 = P1 - P2;
	        if (v.x != 0) {
	                 t0 = w.x / v.x;
	                 t1 = w2.x / v.x;
	        } else {
	                 t0 = w.y / v.y;
	                 t1 = w2.y / v.y;
	        }
	        if (t0 > t1) {                   // must have t0 smaller than t1
	                 float t=t0; t0=t1; t1=t;    // swap if not
	        }
	        if (t0 > 1 || t1 < 0) {
	            return 0;      // NO overlap
	        }
			
			
	        t0 = t0<0 ? 0 : t0;               // clip to min 0
	        t1 = t1>1 ? 1 : t1;               // clip to max 1
	        if (t0 == t1) {                   // intersect is a point
	            I0 = P2 +  t0 * v;
	            return 1;
	        }
	
	        // they overlap in a valid subsegment
	        I0 = P2 + t0 * v;
	        I1 = P2 + t1 * v;
	        return 2;
	    }//if (Mathf.Abs(D) < SMALL_NUM) {...
	
	    // the segments are skew and may intersect in a point
	    // get the intersect parameter for S1
	    float     sI = Perp(v,w) / D;
	    if (sI < 0 || sI > 1)                // no intersect with S1
	        return 0;
	
	    // get the intersect parameter for S2
	    float     tI = Perp(u,w) / D;
	    if (tI < 0 || tI > 1)                // no intersect with S2
	        return 0;
	
	    I0 = P0 + sI * u;                // compute S1 intersect point
	    return 1;
	}

	/// <summary>
	/// Get Intersects factor --- (u, v, inbound).
	/// </summary>
	/// <returns>The factor.</returns>
	/// <param name="P0">P0.</param>
	/// <param name="P1">P1.</param>
	/// <param name="P2">P2.</param>
	/// <param name="P3">P3.</param>
	/// <param name="Factor">Factor.</param>
	public static int IntersectFactor(Vector2 P0, Vector2 P1, Vector2 P2, Vector2 P3, out InterFactor Factor){

		Factor = InterFactor.none;

		Vector2   u = P1 - P0;
		Vector2   v = P3 - P2;
		Vector2   w = P0 - P2;
		float     D = Perp(u,v);

		// test if  they are parallel (includes either being a point)
		if (Mathf.Abs(D) > SMALL_NUM) {

			// the segments are skew and may intersect in a point
			// get the intersect parameter for P01
			float     sI = Perp(v,w) / D;
			if (sI < 0 || sI > 1)                // no intersect with S1
				return 0;
			
			// get the intersect parameter for P23
			float     tI = Perp(u,w) / D;
			if (tI < 0 || tI > 1)                // no intersect with S2
				return 0;
			
			//I0 = P0 + sI * u;                // compute S1 intersect point
			Vector2 w2 = P1 - P2;				//因为Perp(w,v)有可能等于0，所以求P2是否在P2P3另一侧。
			Factor = new InterFactor(){t = sI, u = tI, inbound = ((Perp(w,v) < 0) || (Perp(w2,v) > 0)) };

			return 1;

		}//if (Mathf.Abs(D) > SMALL_NUM) {...

		return 0;
	}
	
	// These 2 segments are skew and may intersect in one point.
	// just delete the part of --- if (Mathf.Abs(D) < SMALL_NUM) {...} 
	public static bool Intersect2D_2SkewSegments( Vector2 P0, Vector2 P1, Vector2 P2, Vector2 P3, 
		int IndexOfP2, int IndexofP3, ref Intersection Inter ) {
//		Inter = new Intersection();
//		I0 = Vector2.zero;
		Vector2   u = P1 - P0;
	    Vector2   v = P3 - P2;
	    Vector2   w = P0 - P2;
	    float     D = Perp(u, v);
	
	    // the segments are skew and may intersect in a point
	    // get the intersect parameter for P0-P1
	    float     uI = Perp(v, w) / D;
	    if (uI < -VECTEX_ROUND || uI > 1 + VECTEX_ROUND)                // no intersect with S1
	        return false;
	
	    // get the intersect parameter for P2-P3		
	    float     vI = Perp(u, w) / D;
	    if (vI < -VECTEX_ROUND || vI > 1 + VECTEX_ROUND)                // no intersect with S2
	        return false;
		
		// if vI == 0, P2 is intersect point;
		if(vI < VECTEX_ROUND){
			Inter.Point0 = P2;
			Inter.Type = PointType.ON_VERTEX;
			Inter.VertIndex = IndexOfP2;
			return true;
		}
		// if vI == 1, P3 is intersect point;
		if(vI > 1 - VECTEX_ROUND){
			Inter.Point0 = P3;
			Inter.Type = PointType.ON_VERTEX;
			Inter.VertIndex = IndexofP3;
			return true;
		}
		// if uI == 0, P0 is intersect point;
		if(uI < VECTEX_ROUND){
			Inter.Point0 = P0;
			Inter.Type = PointType.ON_EDGE;
			Inter.EdgeStartIndex = IndexOfP2;
			Inter.EdgeEndIndex = IndexofP3;
			return true;
		}
		// if uI == 1, P1 is intersect point;
		if(uI > 1 - VECTEX_ROUND){
			Inter.Point0 = P1;
			Inter.Type = PointType.ON_EDGE;
			Inter.EdgeStartIndex = IndexOfP2;
			Inter.EdgeEndIndex = IndexofP3;
			return true;
		}
		
	    Inter.Point0 = P0 + uI * u;                // compute S1 intersect point
	    Inter.Type = PointType.ON_EDGE;
		Inter.EdgeStartIndex = IndexOfP2;
		Inter.EdgeEndIndex = IndexofP3;
		return true;
	}
	
	public static bool Intersect2D_2SkewSegments( Vector2 P0, Vector2 P1, Vector2 P2, Vector2 P3 , ref Vector2 SkewPoint) {
//		Inter = new Intersection();
//		I0 = Vector2.zero;
//		SkewPoint = P0;
		Vector2   u = P1 - P0;
	    Vector2   v = P3 - P2;
	    Vector2   w = P0 - P2;
	    float     D = Perp(u, v);
	
	    // the segments are skew and may intersect in a point
	    // get the intersect parameter for P0-P1
	    float     uI = Perp(v, w) / D;
	    if (uI < -VECTEX_ROUND || uI > 1 + VECTEX_ROUND)                // no intersect with S1
	        return false;
	
	    // get the intersect parameter for P2-P3		
	    float     vI = Perp(u, w) / D;
	    if (vI < -VECTEX_ROUND || vI > 1 + VECTEX_ROUND)                // no intersect with S2
	        return false;
		
		// if vI == 0, P2 is intersect point;
		if(vI < VECTEX_ROUND){
			SkewPoint = P2;
			return true;
		}
		// if vI == 1, P3 is intersect point;
		if(vI > 1 - VECTEX_ROUND){
			SkewPoint = P3;
			return true;
		}
		// if uI == 0, P0 is intersect point;
		if(uI < VECTEX_ROUND){
			SkewPoint = P0;
			return true;
		}
		// if uI == 1, P1 is intersect point;
		if(uI > 1 - VECTEX_ROUND){
			SkewPoint = P1;
			return true;
		}
		
	    SkewPoint = P0 + uI * u;                // compute S1 intersect point
		return true;
	}
	
	// inSegment(): determine if a point is inside a segment
	//    Input:  a point P, and a collinear segment S
	//    Return: 1 = P is inside S
	//            0 = P is  not inside S
	// 仅在已知P与S共线的情况下，判断点P是否在线段S上。
	private static int InSegment( Vector2 P, Segment2D S) {
		// 	return p.x >= min(p1.x, p2.x)
        //		&& p.x <= max(p1.x, p2.x)
        //		&& p.y >= min(p1.y, p2.y)
        //		&& p.y <= max(p1.y, p2.y);
		
	    if (S.P0.x != S.P1.x) {    // S is not  vertical
	        if (S.P0.x <= P.x && P.x <= S.P1.x)
	            return 1;
	        if (S.P0.x >= P.x && P.x >= S.P1.x)
	            return 1;
	    }
	    else {    // S is vertical, so test y  coordinate
	        if (S.P0.y <= P.y && P.y <= S.P1.y)
	            return 1;
	        if (S.P0.y >= P.y && P.y >= S.P1.y)
	            return 1;
	    }
	    return 0;
	}
	
	// 仅在已知P与S共线的情况下，判断点P是否在线段S上。
	private static bool InSegment( Vector2 p, Vector2 p1, Vector2 p2) {
		 	return p.x >= Mathf.Min(p1.x, p2.x)
        		&& p.x <= Mathf.Max(p1.x, p2.x)
        		&& p.y >= Mathf.Min(p1.y, p2.y)
        		&& p.y <= Mathf.Max(p1.y, p2.y);
		
//	    if (P1.x != P2.x) {    // S is not  vertical
//	        if (P1.x <= P.x && P.x <= P2.x)
//	            return 1;
//	        if (P1.x >= P.x && P.x >= P2.x)
//	            return 1;
//	    }
//	    else {    // S is vertical, so test y  coordinate
//	        if (P1.y <= P.y && P.y <= P2.y)
//	            return 1;
//	        if (P1.y >= P.y && P.y >= P2.y)
//	            return 1;
//	    }
//	    return 0;
	}
	
	// 判断P0P1是否会被V内的线段遮挡（与之相交）。
	// 已知条件为：其中一个点一定在多边形内部。
	// 仅返回距离P0最近的相交点。
		
	public static bool SegmentSkewPoly( Vector2 P0, Vector2 P1, List<Vector2> V, ref Intersection Inter) {
//		Inter = new Intersection();
		Intersection SubInter = new Intersection();
		bool isIntersect = false;
			
		int n = V.Count;
		int i , j ;
		
//		Vector2 I1 = Vector2.zero;
		float dis = Vector2.SqrMagnitude(P1 - P0);
//		Vector2 I0;
		
		for(i = 0, j = n - 1; i < n; j = i++) {
			if(Intersect2D_2SkewSegments(P0, P1, V[j], V[i], j, i, ref SubInter)) {
				float thisDis = Vector2.SqrMagnitude(P0 - SubInter.Point0);
				if(dis >= thisDis && thisDis > 0){
					dis = thisDis;
					Inter = SubInter;
					isIntersect = true;
				}				
			}
		}
		
		return isIntersect;		
	}
	
	// 
	// 求点P1在线段P23上的投影位置.
	// 如果P1投影在线段外，返回最近之线段端点。否则，返回投影点位置。
	public static bool Project2D_PointOnSegment( Vector2 P1, Vector2 P2, Vector2 P3, int start, int end, ref Intersection Inter) {
		
		Vector2 u = P1 - P2;
		Vector2 v = P3 - P2;
		float t = Dot (u, v) / Dot (v);
		if(t <= 0){
			Inter.Point0 = P2;
			Inter.Type = PointType.ON_VERTEX;
			Inter.VertIndex = start;
			return true;
		}
		else if ( t >= 1){
			Inter.Point0 = P3;
			Inter.Type = PointType.ON_VERTEX;
			Inter.VertIndex = end;
			return true;
		}
		else {
			Inter.Point0 = P2 + v*t;
			Inter.Type = PointType.ON_EDGE;
			Inter.EdgeStartIndex = start;
			Inter.EdgeEndIndex = end;
			return true;
		}
		//return false;
	}
	
	//if P0 is on poly V's edge, and P1 is outside of V, find project on the edge.
//	public static bool ProjectOnPolyEdge (Vector2 P0, Vector2 P1, int start, int end, List<Vector2> V, ref Intersection Inter) {
////		Inter = new Intersection();
////		Intersection SubInter;
//		return Project2D_PointOnSegment(P1, V[start], V[end], start, end, ref Inter);
////		return false;
//	}
	
//	//if P0 is on poly V's one vertex, and P1 is outside of V, find project on two edge deside the vertex.
//	public static bool ProjectOnPolyVertex(Vector2 P0, Vector2 P1, int index, List<Vector2> V, ref Intersection Inter) {
////		Inter = new Intersection();
//		
//		int n = V.Count;
//		int pre = index == 0 ? n - 1 : index - 1;
//		int next = index == n - 1 ? 0 : index + 1;
//		
//		Vector2 uv = V[pre] + V[next];
//		
//		if(IsLeft(P0, uv, P1) < 0) {
//			return Project2D_PointOnSegment(P1, P0, V[pre],  index, pre,  ref Inter);
//		}else {
//			return Project2D_PointOnSegment(P1, P0, V[next], index, next, ref Inter);
//		}
//		
////		return false;
//	}
	// 判断点P位于线段S上。也就是， 判断线段S与点P相交 。
	// 先用叉積判斷點與線段是否共線，再用點積判斷點是否位於線段端點之間。
	// Intersect2D_PnSegments(): determine if a point is inside a segment
	//    Input:  a point P, and a segment S
	//    Return: 1 = P is inside S
	//            0 = P is  not inside S
	public static bool Intersect2D_PnSegment( Vector2 P, Segment2D S ) {
//		return Perp(S.P0, S.P1, P) == 0 && Dot(P, S.P0, S.P1) <= 0;
		return ( Mathf.Abs(Perp(S.P0, S.P1, P)) < SMALL_NUM )
			&& ( Dot(P, S.P0, S.P1) <= 0 );
		
	}
	
	//is same as Mathf.Sqrt(Dot(v, v));
	public static float Length(Vector2 v) {
		return Mathf.Sqrt(v.x*v.x + v.y*v.y);
	}
	
	//计算P0点至（无限长）直线P1P2的最短距离（垂线长度）
	//叉積的結果為平行四邊形的面積量, 面積除以底边长度得高。
	public static float Distance2D_PointToLine(Vector2 P0, Vector2 P1, Vector2 P2) {
		
		Vector2 v = P0 - P1, u = P2 - P1;
		
		return Mathf.Abs(Perp(u , v)/Length(u));
	}
	
	//计算P1至线段P2P3的最短距离的!平方!
	//如果P1在P2P3上的投影在线段外，则为P0与最近端点的距离；否则为垂线长度。
	public static float Distance2D_PointToSegment(Vector2 P1, Vector2 P2, Vector2 P3){
		
		Vector2 u = P1 - P2;
		Vector2 v = P3 - P2;
//		Vector2 w;
		
		float t = Dot (u, v) / Dot (v);
		if(t <= VECTEX_ROUND){
			return Dot (u);
			
		}
		else if ( t >= 1 - VECTEX_ROUND){
//			w = P1 - P3;
			return Dot ( P1 - P3);
		}
		else {
//			P0 = P2 + v*t;
//			w = u - v*t;
			return Dot (u - v*t);
		}
	}
	
	//这个方法没有用。
	//用Project2D_PointOnSegment（）方法求出投影点位置后，再用Mathf.Sqrt(Dot（P0-ProjectInter）)方法求点至线段距离。
	public static float Distance2D_PointToSegment(Vector2 P1, int start, int end, Vector2 P2, Vector2 P3, out Intersection Inter){
		
		Inter = new Intersection();
		
		Vector2 u = P1 - P2;
		Vector2 v = P3 - P2;
//		Vector2 w;
		
		float t = Dot (u, v) / Dot (v);
		if(t <= VECTEX_ROUND){
			Inter.Type = PointType.ON_VERTEX;
			Inter.VertIndex = start;
			Inter.Point0 = P2;
			return Dot (u);
			
		}
		else if ( t >= 1 - VECTEX_ROUND){
//			w = P1 - P3;
			Inter.Type = PointType.ON_VERTEX;
			Inter.VertIndex = end;
			Inter.Point0 = P3;
			return Dot ( P1 - P3);
		}
		else {
//			P0 = P2 + v*t;
//			w = u - v*t;
			Inter.Type = PointType.ON_EDGE;
			Inter.EdgeStartIndex = start;
			Inter.EdgeEndIndex = end;
			Inter.Point0 = P2 + v*t;
			return Dot (u - v*t);
		}
	}
	
	//有用！
	//求Position在多边形上的投影
	//方法：截取多边形的每个边为目标线段，并调用Project2D_PointOnSegment方法。
	//注：目标线段长度必须大于指定长度
	//
	public static void Project2D_PointToPoly(Vector2 Position, float Length, List<Vector2> V, out Intersection Inter){
		
		Inter = new Intersection(){Type = PointType.ON_VERTEX, Point0 = V[0], VertIndex = 0};
		float dis = Dot(Position - V[0]);
		
		int n = V.Count;
		int i , j ;

		Intersection temp = new Intersection();
		
		
		for(i = 0, j = n - 1; i < n; j = i++) {
			Project2D_PointOnSegment(Position, V[j], V[i], j, i, ref temp);
			float thisDis = Dot ( Position - temp.Point0);			
			float segmentL = Mathf.Sqrt(Dot (V[i] - V[j]));
			if(dis > thisDis && segmentL >= Length) {
				Inter = temp;
				dis = thisDis;
			}
		}
	}
	
	// WHY: 移动某个端点时，线段如何沿着另一条线段移动。
	// 即，通过其中一个端点（不断变化）的投影附加在已知线段上 P2P3 的可能
	//
	//如果P2P3的长度小于 L： 
	//	。。。 这个情况已经通过Project2D_PointToPoly方法剔除了。
	//如果P2P3的长度等于 L：
	//	。。。
	//如果P2P3的长度大于L：
	//	在P2P3头尾各截取Length/2后，重新定义端点，再判断Position的投影位置。
	
	
	public static void AttachToSegmentByEnds(Vector2 ProjectPos, float Length, int Index_0 , int Index_1, Vector2 P2, Vector2 P3, ref Intersection Leader, ref Intersection Follower) {
		Vector2 v = P3 - P2;
		float t = Length / Mathf.Sqrt(Dot (v));
//		float t = Dot (Follower.Point0 - Leader.Point0) / Dot (v);
		
		float DisProToP2 = Mathf.Sqrt(Dot (ProjectPos - P2));
		float DisProToP3 = Mathf.Sqrt(Dot (P3 - ProjectPos));
		
		if(t > 1){
//			return;
		}else if(t == 1){
			if(DisProToP2 < Length){ 
				Leader.Type = PointType.ON_VERTEX;
				Leader.VertIndex = Index_0;
				Leader.Point0 = P2;
				Follower.Type = PointType.ON_VERTEX;
				Follower.VertIndex = Index_1;
				Follower.Point0 = P3;
			}
			else{
				Leader.Type = PointType.ON_VERTEX;
				Leader.VertIndex = Index_1;
				Leader.Point0 = P3;
				Follower.Type = PointType.ON_VERTEX;
				Follower.VertIndex = Index_0;
				Follower.Point0 = P2;
			}
			
//			return;
		}else{
			
			Vector2 Project = v * t;//Project可以理解为在P2P3方向上长度为Length的向量//Mathf.Sqrt(Dot (v)));//v.normalized;//
			
			Vector2 u = Follower.Point0 - Leader.Point0;
			float w = Dot (u, v);
			if(w > 0){//
				if(DisProToP3 <= Length){
					Follower.Type = PointType.ON_VERTEX;
					Follower.VertIndex = Index_1;
					Follower.Point0 = P3;
					Leader.Type = PointType.ON_EDGE;
					Leader.EdgeStartIndex = Index_0;
					Leader.EdgeEndIndex = Index_1;
					Leader.Point0 = P3 - Project;
				}else{
					Leader.Type = PointType.ON_EDGE;
					Leader.EdgeStartIndex = Index_0;
					Leader.EdgeEndIndex = Index_1;
					Leader.Point0 = ProjectPos;
					Follower.Type = PointType.ON_EDGE;
					Follower.EdgeStartIndex = Index_0;
					Follower.EdgeEndIndex = Index_1;
					Follower.Point0 = ProjectPos + Project;
				}
			}else{
				if(DisProToP2 <= Length){
					Follower.Type = PointType.ON_VERTEX;
					Follower.VertIndex = Index_1;
					Follower.Point0 = P2;
					Leader.Type = PointType.ON_EDGE;
					Leader.EdgeStartIndex = Index_0;
					Leader.EdgeEndIndex = Index_1;
					Leader.Point0 = P2 + Project;
				}else{
					Leader.Type = PointType.ON_EDGE;
					Leader.EdgeStartIndex = Index_0;
					Leader.EdgeEndIndex = Index_1;
					Leader.Point0 = ProjectPos;
					Follower.Type = PointType.ON_EDGE;
					Follower.EdgeStartIndex = Index_0;
					Follower.EdgeEndIndex = Index_1;
					Follower.Point0 = ProjectPos - Project;
				}
			}			
		}
	}
	
		
	public static void AttachToPolyByPoints(Vector2 Position, float Length, ref Intersection Leader, ref Intersection Follower, List<Vector2> V) {
		Intersection Inter;
		Project2D_PointToPoly(Position, Length, V, out Inter);
		int n = V.Count;
		
		if(Inter.Type == PointType.ON_VERTEX) {
			int pre = PreIndex(Inter.VertIndex, n);
			int next = NextIndex(Inter.VertIndex, n);
			
			Vector2 uv = V[pre] + V[next];
			
			Leader = Inter;
			
			if(IsLeft(Position, uv, Inter.Point0) < 0 ^ !WN_PnPoly(Position, V))
			{
				AttachToSegmentByEnds(Inter.Point0, Length, pre, Inter.VertIndex, V[pre], Inter.Point0, ref Leader, ref Follower);
				
			}
			else
			{
				AttachToSegmentByEnds(Inter.Point0, Length, Inter.VertIndex, next, Inter.Point0, V[next], ref Leader, ref Follower);
			}
		}
		else if(Inter.Type == PointType.ON_EDGE) {
			AttachToSegmentByEnds(Inter.Point0, Length, Inter.EdgeStartIndex, Inter.EdgeEndIndex, V[Inter.EdgeStartIndex], V[Inter.EdgeEndIndex], ref Leader, ref Follower);

		}
		
	}
	
	//WHY: 移动中心点时，线段如何沿着另一条线段移动。
	//计算固定长度 L 的线段，通过其中心点的投影附加在已知线段上 P2P3 的可能
	//如果P2P3的长度小于 L： 
	//	。。。 这个情况虽然已经通过Project2D_PointToPoly方法剔除了，
	//		但在if(IsLeft(Position, uv, Inter.Point0) < 0 ^ !WN_PnPoly(Position, V))判断时，会再次发生。
	//如果P2P3的长度等于 L：
	//	。。。
	//如果P2P3的长度大于L：
	//	在P2P3头尾各截取Length/2后，重新定义端点，再判断Position的投影位置。
	
	public static void AttachToSegmentByCenter(Vector2 Position, float Length, int Index_0 , int Index_1, Vector2 P2, Vector2 P3, ref Intersection Start, ref Intersection End) {

		Vector2 v = P3 - P2;
		float t = Length / Mathf.Sqrt(Dot (v));
		
		if(t > 1){
//			return;
		}else if(t == 1){
			Start.Type = PointType.ON_VERTEX;
			Start.VertIndex = Index_0;
			Start.Point0 = P2;
			End.Type = PointType.ON_VERTEX;
			End.VertIndex = Index_1;
			End.Point0 = P3;
//			return;
		}else{
			
			Vector2 Project = v * t;//Mathf.Sqrt(Dot (v)));//v.normalized;//		
			Vector2 halfProject = Project * 0.5F;
			
			Vector2 newP2 = P2 + halfProject;
			Vector2 newP3 = P3 - halfProject;
			
			Vector2 w = newP3 - newP2;
				
			t = Dot (Position - newP2, w) / Dot (w);
			
			if(t <= 0){
				//Inter.Point0 = P2;
				Start.Type = PointType.ON_VERTEX;
				Start.VertIndex = Index_0;
				Start.Point0 = P2;
				End.Type = PointType.ON_EDGE;
				End.EdgeStartIndex = Index_0;
				End.EdgeEndIndex = Index_1;
				End.Point0 = P2 + Project;
				
			}
			else if ( t >= 1){
				//Inter.Point0 = P3;
				End.Type = PointType.ON_VERTEX;
				End.VertIndex = Index_1;
				End.Point0 = P3;
				Start.Type = PointType.ON_EDGE;
				Start.EdgeStartIndex = Index_0;
				Start.EdgeEndIndex = Index_1;
				Start.Point0 = P3 - Project;
			}
			else {
				//P0 = P2 + v*t;
				//Inter.Point0 = P2 + v*t;
				Vector2 projectCenter = newP2 + w*t;
				End.Type = PointType.ON_EDGE;
				End.EdgeStartIndex = Index_0;
				End.EdgeEndIndex = Index_1;
				End.Point0 = projectCenter + halfProject ;
				Start.Type = PointType.ON_EDGE;
				Start.EdgeStartIndex = Index_0;
				Start.EdgeEndIndex = Index_1;
				Start.Point0 = projectCenter - halfProject;
				
			}
		}
	}

	
	public static void AttachToPolyByBlock(Vector2 Position, float Length, ref Intersection Start, ref Intersection End, List<Vector2> V) {

		Intersection Inter;
		Project2D_PointToPoly(Position, Length, V, out Inter);
				
		int n = V.Count;
		
		if(Inter.Type == PointType.ON_VERTEX) {
//			int n = V.Count;
			int pre = PreIndex(Inter.VertIndex, n);
			int next = NextIndex(Inter.VertIndex, n);
			
			Vector2 uv = V[pre] + V[next];
			
			if(IsLeft(Position, uv, Inter.Point0) < 0 ^ !WN_PnPoly(Position, V))
			{
				AttachToSegmentByCenter(Inter.Point0, Length, pre, Inter.VertIndex, V[pre], Inter.Point0, ref Start, ref End);
			}
			else
			{
				AttachToSegmentByCenter(Inter.Point0, Length, Inter.VertIndex, next, Inter.Point0, V[next], ref Start, ref End);
			}
		}else if(Inter.Type == PointType.ON_EDGE) {
			AttachToSegmentByCenter(Inter.Point0, Length, Inter.EdgeStartIndex, Inter.EdgeEndIndex, V[Inter.EdgeStartIndex], V[Inter.EdgeEndIndex], ref Start, ref End);
		}
		
	}
	
	public static void TranslateSegment( ref Vector2 P0, ref Vector2 P1, float Spacing) {
		//static function OrthoNormalize(normal: Vector3, tangent: Vector3, binormal: Vector3): void;
		Vector3 nromal = Vector3.back;//or forward
		Vector3 tangent = P1 - P0;
		Vector3 binormal = Vector3.zero;
		Vector3.OrthoNormalize(ref nromal, ref tangent, ref binormal);
//		Vector2.
		Vector2 space = binormal * Spacing;
		P0 += space;
		P1 += space;
		
	}
	
	public static List<Vector2> ScalePoly(List<Vector2> Poly, float Spacing){
		
		int n = Poly.Count;
		
		List<Vector2> scaledPoly = new List<Vector2>();
		
		Vector2 P0, P1, P2, P3;
		Vector2 skewPoint = Vector2.zero;
		
		for(int i = 0; i < n; i++) {
			P0 = Poly[PreIndex(i, n)];
			P1 = P2 = Poly[i];
			P3 = Poly[NextIndex(i, n)];
			TranslateSegment(ref P0, ref P1, Spacing);
			TranslateSegment(ref P2, ref P3, Spacing);
			if(Intersect2D_2SkewSegments(P0, P1, P2, P3, ref skewPoint)){
				scaledPoly.Add(skewPoint);
			}else{
				scaledPoly.Add(P1);
				scaledPoly.Add(P2);
			}
		}
		
		return scaledPoly;
	}
	
	public static int PreIndex(int index, int size){
		return index == 0 ? size - 1 : index - 1;
	}
	
	public static int NextIndex(int index, int size) {
		return index == size - 1 ? 0 : index + 1;
	}
}

//
//public enum IntersectType {
//	SKEW 			= 0,	//两线段相交，且仅交于一点。
//	JOINT 			= 1,	//两线段首位相连，且不平行。
//	END_TO_END		= 2,	//两线段首位相连，且平行（共线）。
//	OVERLAP			= 3,	//两线段部分或全部重叠（共线）。
//	FIRST_IS_POINT	= 4,	//第一线段为点，且在第二线段上。
//	SECOND_IS_POINT	= 5,	//第二线段为点，且在第一线段上。
//	BOTH_ARE_POINT	= 6,	//两线段均为点，且重合（共点）。
//	NONE			= 7		//两线段不相交。
//	
//}

public struct Segment2D {
	public Vector2 P0;
	public Vector2 P1;
	
	public Segment2D (Vector2 p0, Vector2 p1) {
		P0 = p0;
		P1 = p1;
	}
}

public struct Segment3D {
	public Vector3 P0;
	public Vector3 P1;
	
	public Segment3D (Vector3 p0, Vector3 p1) {
		P0 = p0;
		P1 = p1;
	}
}
