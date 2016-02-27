using UnityEngine;
using System.Collections;

public class _RotateHandler : MonoBehaviour {

    public bool IsRotatable = false;
    
    public float deltaAngle = 0.5f;

    public void StartRotate()
    {
        IsRotatable = true;
    }

    public void StopRotate()
    {
        IsRotatable = false;
        transform.eulerAngles = Vector3.zero;
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {

        if (!IsRotatable)
            return;

        transform.eulerAngles += new Vector3(0, deltaAngle, 0);

	}
}
