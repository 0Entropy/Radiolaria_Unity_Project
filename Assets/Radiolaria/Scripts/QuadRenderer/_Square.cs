using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public abstract class _Square : _Quadrangle
{

    //public float width = 1f;

    //   
    //  up(y)				1-----width----2
    //  |					|              |
    //  |					|              |
    //  |					|            length
    //  |					|              |
    //  |					|              |
    //  .-------->right(x)	0--------------3
    //

    public float dimension = 1.0f;

    protected override void HandleOnInitial()
    {
        Name = "Square";
    }

    protected override void HandleOnUpdateVertices()
    {
        Vertices = new Vector3[]
        {
            new Vector3(-dimension, -dimension, 0),
            new Vector3(-dimension, dimension, 0),
            new Vector3(dimension, dimension, 0),
            new Vector3(dimension, -dimension, 0)
        };
    }
    
}
