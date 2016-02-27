namespace Geometry
{

    using UnityEngine;
    using System.Collections.Generic;

    public class Utils
    {
        private const float VECTEX_ROUND = 0.000001F;
        // cn_PnPoly(): crossing number test for a point in a polygon
        //      Input:   P = a point,
        //               V[] = vertex points of a polygon V[n+1] with V[n]=V[0]
        //      Return:  0 = outside, 1 = inside
        // This code is patterned after [Franklin, 2000]

        public static int CN_PnPoly(Vector2 P, Vector2[] V)
        {

            int n = V.Length;
            int i, j;
            int cn = 0;    // the  crossing number counter

            // loop through all edges of the polygon
            for (i = 0, j = n - 1; i < n; j = i++)
            {    // edge from V[i]  to V[i+1]
                if (((V[j].y <= P.y) && (V[i].y > P.y))     // an upward crossing
                 || ((V[j].y > P.y) && (V[i].y <= P.y)))
                { // a downward crossing
                  // compute  the actual edge-ray intersect x-coordinate
                    float vt = (float)(P.y - V[j].y) / (V[i].y - V[j].y);
                    if (P.x < V[j].x + vt * (V[i].x - V[j].x)) // P.x < intersect
                        ++cn;   // a valid crossing of y=P.y right of P.x
                    j = i;
                }
            }
            return (cn & 1);    // 0 if even (out); and 1 if odd (in).
        }

        public static int WN_PnPoly(Vector2 P, Vector2[] V)
        {
            int n = V.Length;
            int i, j;
            int wn = 0;                         // the  winding number counter

            // loop through all edges of the polygon
            for (i = 0, j = n - 1; i < n; j = i++)
            {               // edge from V[i] to  V[i+1]
                if (V[j].y <= P.y)
                {           // start y <= P.y
                    if (V[i].y > P.y)       // an upward crossing
                        if (IsLeft(V[j], V[i], P) > 0)  // P left of  edge
                            ++wn;               // have  a valid up intersect
                }
                else
                {                        // start y > P.y (no test needed)
                    if (V[i].y <= P.y)      // a downward crossing
                        if (IsLeft(V[j], V[i], P) < 0)  // P right of  edge
                            --wn;               // have  a valid down intersect
                }
                j = i;
            }
            return wn;                          // = 0, out; != 0, in.
        }

        public static bool WN_PnPoly(Vector2 P, List<Vector2> V)
        {
            int n = V.Count;
            int i, j;
            int wn = 0;                         // the  winding number counter

            // loop through all edges of the polygon
            for (i = 0, j = n - 1; i < n; j = i++)
            {               // edge from V[i] to  V[i+1]
                if (V[j].y <= P.y)
                {           // start y <= P.y
                    if (V[i].y > P.y)       // an upward crossing
                        if (IsLeft(V[j], V[i], P) > 0)  // P left of  edge
                            ++wn;               // have  a valid up intersect
                }
                else
                {                        // start y > P.y (no test needed)
                    if (V[i].y <= P.y)      // a downward crossing
                        if (IsLeft(V[j], V[i], P) < 0)  // P right of  edge
                            --wn;               // have  a valid down intersect
                }
                j = i;
            }
            return wn != 0;                         // = 0, out; != 0, in.
        }

        //	inline int
        public static float IsLeft(Vector2 P0, Vector2 P1, Vector2 P2)
        {
            return ((P1.x - P0.x) * (P2.y - P0.y)
                    - (P2.x - P0.x) * (P1.y - P0.y));
        }

        // 叉積運算，回傳純量（除去方向）
        // 叉積的結果為平行四邊形的面積量。
        // 叉積为0时，可以理解为两向量平行。
        // 另，点到直线垂直距离也是运用叉积计算。叉積求出 v1 v2 組成的平行四邊形面積，
        // 然後除以 v2 的長度，便是垂直距離。叉積可能會有負值，請記得取絕對值，
        // 才不會得到負的距離。
        //（点与线段距离因点位置区域，最小距离可能是点至线段端点距离）
        // #define perp(u,v)  ((u).x * (v).y - (u).y * (v).x)  // perp product  (2D)
        public static float Perp(Vector2 u, Vector2 v)
        {
            return (u.x * v.y - u.y * v.x);
        }

        public static bool Intersect2D_2SkewSegments(Vector2 P0, Vector2 P1, Vector2 P2, Vector2 P3, ref Vector2 SkewPoint)
        {
            //		Inter = new Intersection();
            //		I0 = Vector2.zero;
            //		SkewPoint = P0;
            Vector2 u = P1 - P0;
            Vector2 v = P3 - P2;
            Vector2 w = P0 - P2;
            float D = Perp(u, v);

            if (D < VECTEX_ROUND)
                D = VECTEX_ROUND;
            //Debug.Log("Intersect2D_2Sk... D = " + D);

            // the segments are skew and may intersect in a point
            // get the intersect parameter for P0-P1
            float uI = Perp(v, w) / D;
            if (uI < -VECTEX_ROUND || uI > 1 + VECTEX_ROUND)                // no intersect with S1
                return false;

            // get the intersect parameter for P2-P3		
            float vI = Perp(u, w) / D;
            if (vI < -VECTEX_ROUND || vI > 1 + VECTEX_ROUND)                // no intersect with S2
                return false;

            // if vI == 0, P2 is intersect point;
            if (vI < VECTEX_ROUND)
            {
                SkewPoint = P2;
                return true;
            }
            // if vI == 1, P3 is intersect point;
            if (vI > 1 - VECTEX_ROUND)
            {
                SkewPoint = P3;
                return true;
            }
            // if uI == 0, P0 is intersect point;
            if (uI < VECTEX_ROUND)
            {
                SkewPoint = P0;
                return true;
            }
            // if uI == 1, P1 is intersect point;
            if (uI > 1 - VECTEX_ROUND)
            {
                SkewPoint = P1;
                return true;
            }

            SkewPoint = P0 + uI * u;                // compute S1 intersect point
            return true;
        }

        public static void TranslateSegment(ref Vector2 P0, ref Vector2 P1, float Spacing)
        {
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

        public static List<Vector2> ScalePoly(List<Vector2> Poly, float Spacing)
        {

            int n = Poly.Count;

            List<Vector2> scaledPoly = new List<Vector2>();

            Vector2 P0, P1, P2, P3;
            Vector2 skewPoint = Vector2.zero;

            for (int i = 0; i < n; i++)
            {
                P0 = Poly[PreIndex(i, n)];
                P1 = P2 = Poly[i];
                P3 = Poly[NextIndex(i, n)];

                TranslateSegment(ref P0, ref P1, Spacing);
                TranslateSegment(ref P2, ref P3, Spacing);

                if (Intersect2D_2SkewSegments(P0, P1, P2, P3, ref skewPoint))
                {
                    scaledPoly.Add(skewPoint);
                }
                else
                {
                    scaledPoly.Add(P1);
                    scaledPoly.Add(P2);
                }
            }

            return scaledPoly;
        }
        
        public static int PreIndex(int index, int size)
        {
            return index == 0 ? size - 1 : index - 1;
        }

        public static int NextIndex(int index, int size)
        {
            return index == size - 1 ? 0 : index + 1;
        }
    }
}
