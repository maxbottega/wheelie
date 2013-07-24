// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Transform)]
	[Tooltip("Useful to set up sideway camera")]
public class SideCameraFollowAction : MonoBehaviour 
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
	}