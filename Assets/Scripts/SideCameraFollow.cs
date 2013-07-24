using UnityEngine;
using System.Collections;

public class SideCameraFollow : MonoBehaviour 
{
	public Transform mTarget = null;
	public float mDistance = 10;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		transform.position = mTarget.position + mTarget.right * mDistance;
	}
}
