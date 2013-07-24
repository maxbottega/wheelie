using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChunkPlacer : MonoBehaviour 
{
	public GameObject mChunkFolder = null;
	public GameObject mAnchor = null;
	
	List<GameObject> mChunkList = new List<GameObject>();

	// Use this for initialization
	void Start () 
	{
		foreach(Transform child in mChunkFolder.transform)
		{
			mChunkList.Add(child.gameObject);
		}
		mChunkList[0].transform.position = mAnchor.transform.position;
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
