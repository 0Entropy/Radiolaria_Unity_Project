using System;
using UnityEngine;

public struct Intersection
{
    public int EdgeStartIndex;      //相交点所处的边序列
    public int EdgeEndIndex;        //
    public int VertIndex;           //与多边形相交与顶点时，该顶点序列号
    public PointType Type;          //
    public Vector2 Point0;          //

    public static Intersection none = new Intersection() { Type = PointType.NULL };

    public static Intersection Duplicate(Intersection value)
    {
        return new Intersection()
        {
            Point0 = value.Point0,
            Type = value.Type,
            VertIndex = value.VertIndex,
            EdgeStartIndex = value.EdgeStartIndex,
            EdgeEndIndex = value.EdgeEndIndex
        };
    }
}

public enum PointType
{
    NULL = 0,   //
    INSIDE = 1, //在多边形内部
    ON_EDGE = 2,    //在多边形的边上
    ON_VERTEX = 3,  //在多边形的顶点上
                    //OUTSIDE		= 4	//在多边形外部（应该不会发生）

}
