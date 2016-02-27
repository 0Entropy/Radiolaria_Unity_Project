using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public abstract class _Rectangle : _Quadrangle
{

    //   
    //  up(y)			1---------width---------2
    //  ^				|						|
    //  |				|					  length
    //  |				|						|
    //  .--->right(x)	0-----------------------3
    //

    public float width = 1.0f;
    public float blankCap = 1.0f;
    public Vector2 startPosition;
    public Vector2 endPosition;
    

    public void SetEndsPosition(Vector2 startPos, Vector2 endPos)
    {
        startPosition = startPos;
        endPosition = endPos;
    }

    protected override void HandleOnUpdateVertices()
    {
        float length = Vector3.Distance(startPosition, endPosition) - blankCap * 2;
        float halfLength = length * 0.5f;
        float halfWidth = width * 0.5f;

        Vector3 position = (endPosition + startPosition) * 0.5f;
        float angle = Mathf.Atan2(endPosition.x - startPosition.x, endPosition.y - startPosition.y) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.back);

        //Quaternion rotation = Quaternion.FromToRotation(Vector3.up, endPos - startPos);

        transform.localPosition = position;
        transform.localRotation = rotation;

        /*Vertices[0] = halfLength*Vector3.left + Vector3.down * size;
		Vertices[1] = halfLength*Vector3.left + Vector3.up * size;
		Vertices[2] = halfLength*Vector3.right + Vector3.up * size;
		Vertices[3] = halfLength*Vector3.right + Vector3.down * size;*/

        /*Vertices[0] = halfWidth * Vector3.left + Vector3.down * halfLength;
        Vertices[1] = halfWidth * Vector3.left + Vector3.up * halfLength;
        Vertices[2] = halfWidth * Vector3.right + Vector3.up * halfLength;
        Vertices[3] = halfWidth * Vector3.right + Vector3.down * halfLength;*/
        
        List<Vector3> vertices = new List<Vector3>(){
            (halfWidth * Vector3.left + Vector3.down * halfLength),
            (halfWidth * Vector3.left + Vector3.up * halfLength),
            (halfWidth * Vector3.right + Vector3.up * halfLength),
            (halfWidth * Vector3.right + Vector3.down * halfLength)
        };
        /*vertices.Add(halfWidth * Vector3.left + Vector3.down * halfLength);
        vertices.Add(halfWidth * Vector3.left + Vector3.up * halfLength);
        vertices.Add(halfWidth * Vector3.right + Vector3.up * halfLength);
        vertices.Add(halfWidth * Vector3.right + Vector3.down * halfLength);*/

        if (!ForceEarCut.AreVerticesClockwise(vertices.Select(v => new Vector2(v.x, v.y)).ToList(), 0, 4))
        {
            vertices.Reverse();

        }
        Vertices = vertices.ToArray();

        float f = length / width;

        Vector2[] uv = new Vector2[] {
            new Vector2(0, 0),
        new Vector2(0, f),
        new Vector2(1, f),
        new Vector2(1, 0)
        };

        UV = uv;
    }

    protected override void HandleOnInitial()
    {
        Name = "Rectangle";
        //IsUpdateAtRuntime = true;
    }

    //	public void ResetEndsPosition () {
    //		float halfLength = ( GetComponent<BoxCollider>().size.y + SPACE )* 0.5F;
    //		
    //		startPosition = transform.TransformPoint(halfLength*Vector3.up);
    //		endPosition = transform.TransformPoint(halfLength*Vector3.down);
    //	}
    //	
    //	public void ResetTransform () {
    //		
    //		if(startTransform == null || endTransform == null){
    //			return;
    //		}
    //		
    //		Vector3 startPosition = startTransform.localPosition;
    //		Vector3 endPosition = endTransform.localPosition;
    //		
    //		float length = Vector3.Distance(startPosition, endPosition) - SPACE;		
    //		float halfLength = length * 0.5f;
    //		
    //		Vector3 position = (endPosition + startPosition)*0.5f;		
    //		Quaternion rotation = Quaternion.FromToRotation(Vector3.up, startPosition - endPosition );
    //				
    //		gameObject.transform.localPosition = position;
    //		gameObject.transform.localRotation = rotation;
    //		
    //		GetComponent<BoxCollider>().size = new Vector3(HALFSPACE, length, HALFSPACE);
    //		
    //		Vertices[0] = halfLength*Vector3.down + basicVertices[0];
    //		Vertices[1] = halfLength*Vector3.up + basicVertices[1];
    //		Vertices[2] = halfLength*Vector3.up + basicVertices[2];
    //		Vertices[3] = halfLength*Vector3.down + basicVertices[3];
    //		
    //		ResetMesh();
    //		
    //	}
    //	
    //	public void ResetStartAndEndTransform () {
    //		float halfLength = ( GetComponent<BoxCollider>().size.y + SPACE )* 0.5F;
    //		
    //		startTransform.position = gameObject.transform.TransformPoint(halfLength*Vector3.up);
    //		endTransform.position = gameObject.transform.TransformPoint(halfLength*Vector3.down);
    //		
    //	}


}
