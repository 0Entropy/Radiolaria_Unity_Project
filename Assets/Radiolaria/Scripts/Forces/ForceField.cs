using UnityEngine;
using System.Collections;

public class ForceField
{
    Vector2[,] Field;
    int Cols, Rows;
    float Resolution;
    float Radius = 5.0f;
    float SqrRadius = 25.0f;

    Matrix4x4 WorldToGridMat;
    //Matrix4x4 GridToWorldMat;

    public ForceField(int resolution) { }

    public ForceField(float width, float height, float resolution)
    {
        Cols = (int)(width / resolution);
        Rows = (int)(height / resolution);
        Resolution = resolution;
        Field = new Vector2[Cols,Rows];

        WorldToGridMat = Matrix4x4.TRS(new Vector3(Cols, Rows, 0) * 0.5f, Quaternion.identity, Vector3.one *  (1.0f / resolution));
        //GridToWorldMat = WorldToGridMat.inverse;
    }

    public void AttachAttractiveForceAt(Vector2 worldPosition, bool isRepulsive = false)
    {
        Vector2 gridPos = WorldToGridMat.MultiplyPoint3x4(worldPosition);
        //Debug.Log(string.Format("worldPos : {0}, gridPos : {1}", worldPosition, gridPos));
        for (int i=0; i< Cols; i++)
        {
            for(int j=0; j< Rows; j++)
            {
                Vector2 srcVector =  gridPos - CellCenter(i, j);
                //Vector2 srcVector = CellCenter(i, j) - gridPos;
                //Debug.Log("srcVector : " + srcVector);
                //float distance = Vector2.Distance(gridPos, cellCenter);
                float distance = srcVector.magnitude;
                //Debug.Log(sqrMagni);
                if (distance < Radius / Resolution)
                {
                    if(Field[i,j].magnitude < 48.0f)
                        Field[i, j] += (isRepulsive ? -1 : 1) * srcVector.normalized * 2.0f;// * (1.0f - distance / Radius) * 2f;

                    //Debug.Log(string.Format("Field at [{0},{1}] is {2}, the CellCenter is {3}", i , j, Field[i, j], CellCenter(i, j)));
                }
            }
        }
    }

    public void AttachTorqueForceAt(Vector2 worldPosition, bool isAnticolockwise = false)
    {
        Vector2 gridPos = WorldToGridMat.MultiplyPoint3x4(worldPosition);
        for (int i = 0; i < Cols; i++)
        {
            for (int j = 0; j < Rows; j++)
            {
                Vector2 srcVector = gridPos - CellCenter(i, j);
                float distance = srcVector.magnitude;
                if (distance < Radius / Resolution)
                {

                    if (Field[i, j].magnitude < 48.0f)
                        Field[i, j] += (isAnticolockwise ? -1 : 1) * new Vector2(-srcVector.y, srcVector.x).normalized * 2.0f;// * 2.0f;// * (1.0f - distance / Radius) * 2f;

                    //Debug.Log(string.Format("Field at [{0},{1}] is {2}, the CellCenter is {3}", i, j, Field[i, j], CellCenter(i, j)));
                }
            }
        }
    }

    /*public Vector3 Lookup(float x, float y)
    {
        return Field[Mathf.Clamp((int)x, 0, Cols - 1), Mathf.Clamp((int)y, 0, Rows - 1)];
    }*/

    public Vector3 Lookup(Vector3 worldPosition)
    {
        Vector2 gridPos = WorldToGridMat.MultiplyPoint3x4(worldPosition);
        //return Lookup(gridPos.x, gridPos.y);
        return Field[Mathf.Clamp((int)gridPos.x, 0, Cols - 1), Mathf.Clamp((int)gridPos.y, 0, Rows - 1)];
    }

    Vector2 CellCenter(int col, int row)
    {
        return new Vector2(((float)col + 0.5f), ((float)row + 0.5f));
        //return GridToWorldMat.MultiplyPoint3x4(new Vector2(((float)col + 0.5f), ((float)row + 0.5f)) * Resolution);
    }

    public override string ToString()
    {
        return string.Format("ForceFidle : [{0}, {1}], Resolution : {2}", Cols, Rows, Resolution);
    }

    public void Clear()
    {
        for (int i = 0; i < Cols; i++)
        {
            for (int j = 0; j < Rows; j++)
            {
                Field[i, j] = Vector2.zero;
            }
        }
    }
}
