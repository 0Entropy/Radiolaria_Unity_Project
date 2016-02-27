using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

[Serializable]
public class TriMeshData {

    //public int[] ids { set; get; }
    public FacialBounds bounds { set; get; }

    public SerializableVector3[] vertices { set; get; }
    public int[] triangles { set; get; }
    public SerializableVector2[] uv { set; get; }

    public TriMeshData()
    {

    }

    public TriMeshData(UnityEngine.Mesh mesh)
    {
        int size = mesh.vertices.Length;

        uv = mesh.uv.ToSerializableVector2Array();//new SerializableVector2[size];
        vertices = mesh.vertices.ToSerializableVector3Array();// new SerializableVector3[size];
        /*for(int i = 0; i < size; i++)
        {
            uv[i] = (SerializableVector2)mesh.uv[i];
            vertices[i] = (SerializableVector3)mesh.vertices[i];
        }*/
        triangles = mesh.triangles;
    }

  

}

[Serializable]
public class FacialBounds
{
    public FacialBounds()
    {
        minX = minY = float.MaxValue;
        maxX = maxY = float.MinValue;
    }
    public float minX { set; get; }
    public float minY { set; get; }
    public float maxX { set; get; }
    public float maxY { set; get; }
    public float width
    {
        get
        {
            return (minX >= maxX) ? 0 : (maxX - minX);
        }
    }
    public float height
    {
        get
        {
            return (minY >= maxY) ? 0 : (maxY - minY);
        }
    }
}

[Serializable]
public class SerializableVector3
{
    /// <summary>
    /// x component
    /// </summary>
    public float x;

    /// <summary>
    /// y component
    /// </summary>
    public float y;

    /// <summary>
    /// z component
    /// </summary>
    public float z;

    public SerializableVector3() : this(0, 0, 0)
    {

    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="rX"></param>
    /// <param name="rY"></param>
    /// <param name="rZ"></param>
    public SerializableVector3(float rX, float rY, float rZ)
    {
        x = rX;
        y = rY;
        z = rZ;
    }

    /// <summary>
    /// Returns a string representation of the object
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return String.Format("[{0}, {1}, {2}]", x, y, z);
    }

    /// <summary>
    /// Automatic conversion from SerializableVector3 to Vector3
    /// </summary>
    /// <param name="rValue"></param>
    /// <returns></returns>
    public static implicit operator Vector3(SerializableVector3 rValue)
    {
        return new Vector3(rValue.x, rValue.y, rValue.z);
    }

    /// <summary>
    /// Automatic conversion from Vector3 to SerializableVector3
    /// </summary>
    /// <param name="rValue"></param>
    /// <returns></returns>
    public static implicit operator SerializableVector3(Vector3 rValue)
    {
        return new SerializableVector3(rValue.x, rValue.y, rValue.z);
    }

    
}

[Serializable]
public class SerializableVector2
{
    /// <summary>
    /// x component
    /// </summary>
    public float x;

    /// <summary>
    /// y component
    /// </summary>
    public float y;

    public SerializableVector2() : this(0, 0){ }
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="rX"></param>
    /// <param name="rY"></param>
    /// <param name="rZ"></param>
    public SerializableVector2(float rX, float rY)
    {
        x = rX;
        y = rY;
    }

    /// <summary>
    /// Returns a string representation of the object
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return String.Format("[{0}, {1}]", x, y);
    }

    /// <summary>
    /// Automatic conversion from SerializableVector3 to Vector3
    /// </summary>
    /// <param name="rValue"></param>
    /// <returns></returns>
    public static implicit operator Vector2(SerializableVector2 rValue)
    {
        return new Vector3(rValue.x, rValue.y);
    }

    /// <summary>
    /// Automatic conversion from Vector3 to SerializableVector3
    /// </summary>
    /// <param name="rValue"></param>
    /// <returns></returns>
    public static implicit operator SerializableVector2(Vector2 rValue)
    {
        return new SerializableVector2(rValue.x, rValue.y);
    }

    public static SerializableVector2 operator +(SerializableVector2 a, SerializableVector2 b)
    {
        return new SerializableVector2(a.x + b.x, a.y + b.y);
    }

    public static SerializableVector2 operator -(SerializableVector2 a, SerializableVector2 b)
    {
        return new SerializableVector2(a.x - b.x, a.y - b.y);
    }

    public static SerializableVector2 operator *(float d, SerializableVector2 a)
    {
        return new SerializableVector2(d * a.x, d * a.y);
    }

    public static SerializableVector2 operator *(SerializableVector2 a, float d)
    {
        return new SerializableVector2(d * a.x, d * a.y);
    }

    public double magnitude
    {
        get
        {
            return Math.Sqrt(x * x + y * y);
        }
    }
}

public static class SerializableExtensions
{
    public static SerializableVector2[] ToSerializableVector2Array(this List<Vector2> listVector2)
    {
        List<SerializableVector2> result = new List<SerializableVector2>();
        foreach(var pt in listVector2.Select(p => new SerializableVector2(p.x, p.y)))
        {
            result.Add(pt);
        }
        return result.ToArray();
    }

    public static SerializableVector2[] ToSerializableVector2Array(this Vector2[] listVector2)
    {
        List<SerializableVector2> result = new List<SerializableVector2>();
        foreach (var pt in listVector2.Select(p => new SerializableVector2(p.x, p.y)))
        {
            result.Add(pt);
        }
        return result.ToArray();
    }

    public static SerializableVector3[] ToSerializableVector3Array(this Vector3[] listVector2)
    {
        List<SerializableVector3> result = new List<SerializableVector3>();
        foreach (var pt in listVector2.Select(p => new SerializableVector3(p.x, p.y, p.z)))
        {
            result.Add(pt);
        }
        return result.ToArray();
    }

    public static Vector3[] ToVector3Array(this SerializableVector3[] src)

    {
        Vector3[] result = new Vector3[src.Length];
        for (int i = 0; i < src.Length; i++)
        {
            result[i] = (Vector3)src[i];
        }
        return result;
    }

    public static Vector2[] ToVector2Array(this SerializableVector2[] src)

    {
        Vector2[] result = new Vector2[src.Length];
        for (int i = 0; i < src.Length; i++)
        {
            result[i] = (Vector2)src[i];
        }
        return result;
    }
}