using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using TriangleNet.Geometry;

public class Triangle3DRenderer : AbstractMeshRenderer {
    
    const float offset = 0.2f;

    float shapeDepth = offset * 2.0f;
    Vector3 offsetPos = new Vector3(0, 0, offset);

    public TriangleNet.Mesh TriangleMesh { set; get; }

    protected override void HandleOnUpdate()
    {
        Clear();

        List<Vector3> points = TriangleMesh.Vertices.Select(p => new Vector3((float)p.X, (float)p.Y, 0)).ToList();
        foreach (var segment in TriangleMesh.Segments)
        {

            Vector3 p0 = points[segment.P0];
            Vector3 p1 = points[segment.P1];

            Vector3 a = p0 + offsetPos;
            Vector3 b = p1 + offsetPos;
            Vector3 c = p1 - offsetPos;
            Vector3 d = p0 - offsetPos;

            Vector3 normal = p1 - p0;
            Vector3 tangent = Vector3.back;
            Vector3 binormal = Vector3.zero;

            Vector3.OrthoNormalize(ref normal, ref tangent, ref binormal);

            Matrix4x4 toNewSpace = new Matrix4x4();
            toNewSpace.SetRow(0, normal);
            toNewSpace.SetRow(1, tangent);
            toNewSpace.SetRow(2, binormal);
            toNewSpace[3, 3] = 1.0F;

            Vector3 ma = toNewSpace.MultiplyPoint3x4(a);
            Vector3 mb = toNewSpace.MultiplyPoint3x4(b);
            Vector3 mc = toNewSpace.MultiplyPoint3x4(c);
            Vector3 md = toNewSpace.MultiplyPoint3x4(d);

            //Debug.Log(string.Format("{0}:{1},{2}:{3},{4}:{5},{6}:{7}", a, ma, b, mb, c, mc, d, md));
            /*
                basisA = stretchAxis;
                Vector3.OrthoNormalize(ref basisA, ref basisB, ref basisC);
                Matrix4x4 toNewSpace = new Matrix4x4();
                toNewSpace.SetRow(0, basisA);
                toNewSpace.SetRow(1, basisB);
                toNewSpace.SetRow(2, basisC);
                toNewSpace[3, 3] = 1.0F;

                Matrix4x4 scale = new Matrix4x4();
                scale[0, 0] = stretchFactor;
                scale[1, 1] = 1.0F / stretchFactor;
                scale[2, 2] = 1.0F / stretchFactor;
                scale[3, 3] = 1.0F;
                Matrix4x4 fromNewSpace = toNewSpace.transpose;
                Matrix4x4 trans = toNewSpace * scale * fromNewSpace;
                int i = 0;
                while (i < origVerts.Length) {
                    newVerts[i] = trans.MultiplyPoint3x4(origVerts[i]);
                    i++;
                }
                mf.mesh.vertices = newVerts;

            */

            List<Vector2> quad2 = new List<Vector2>()
            {
                (Vector2)ma, (Vector2)mb,(Vector2)mc,(Vector2)md
            };
            List<Vector3> quad3 = new List<Vector3>() { a, b, c, d };

            if(ForceEarCut.AreVerticesClockwise(quad2, 0, 4))
            {
                quad3.Reverse();
            }



            normals.Add(binormal);
            normals.Add(binormal);
            normals.Add(binormal);
            normals.Add(binormal);

            /*vertices.Add(p0 + offsetPos);
            vertices.Add(p1 + offsetPos);
            vertices.Add(p1 - offsetPos);
            vertices.Add(p0 - offsetPos);*/

            vertices.AddRange(quad3);

            float dis = Vector3.Distance(p0, p1);
            float f = dis / shapeDepth;

            uv.Add(new Vector2(0, 0));
            uv.Add(new Vector2(0, f));
            uv.Add(new Vector2(1, f));
            uv.Add(new Vector2(1, 0));
        }

        for (int a = 0, n = points.Count * 4; a < n; a += 4)
        {
            var b = (a + 1) % n;
            var c = (a + 2) % n;
            var d = (a + 3) % n;
            triangles.Add(a);// (offsetIndex + a);
            triangles.Add(b);// (offsetIndex + b);
            triangles.Add(c);// (offsetIndex + c);
            triangles.Add(a);// (offsetIndex + a);
            triangles.Add(c);// (offsetIndex + c);
            triangles.Add(d);// (offsetIndex + d);
        }
        //CalculateSideData(TriangleMesh.Vertices.Select(p => new Vector2((float)p.X, (float)p.Y)).ToList());
    }

    public void CalculateFaceData()
    {
        if (TriangleMesh == null)
            return;

        if (vertices == null || uv == null || triangles == null)
        {

            vertices = new List<Vector3>();
            uv = new List<Vector2>(); 
            triangles = new List<int>();
        }
        /*else
        {

            vertices.Clear();
            uv.Clear();
            triangles.Clear();
        }*/

        int pointCount = TriangleMesh.Vertices.Count;
        int triangleCount = TriangleMesh.Triangles.Count;

        vertices = new List<Vector3>(pointCount * 4);   //fore : n,  back : n, side : 2 * n;

        Vector3 offsetPos = new Vector3(0, 0, offset);

        int i = 0;
        foreach (var pt in TriangleMesh.Vertices.Select(p => new Vector3((float)p.X, (float)p.Y, 0)))
        {
            vertices[i] = pt - offsetPos;
            vertices[pointCount + 1] = pt - offsetPos;
            i++;
        }
    }

    public void CalculateSideData(List<Vector2> points)
    {

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
            Vector3 p1 = (Vector3)points[(i + 1) % count];

            vertices.Add(p0 + offsetPos);
            vertices.Add(p1 + offsetPos);
            vertices.Add(p1 - offsetPos);
            vertices.Add(p0 - offsetPos);

            float dis = Vector3.Distance(p0, p1);
            float f = dis / shapeDepth;

            uv.Add(new Vector2(0, 0));
            uv.Add(new Vector2(0, f));
            uv.Add(new Vector2(1, f));
            uv.Add(new Vector2(1, 0));
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
