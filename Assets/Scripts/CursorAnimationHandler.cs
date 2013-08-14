using UnityEngine;
using System.Collections;

public class CursorAnimationHandler : MonoBehaviour {
	
	public GameObject targetObject;
	private RaycastHit hit;
	
	void Update () {		
		if (targetObject!=null){
			//Debug.Log(Input.mousePosition);
			Ray raySelector=this.camera.ScreenPointToRay (Input.mousePosition); 
			if (Physics.Raycast(raySelector.origin, raySelector.direction,out hit, 300f,1<<31)){
				targetObject.transform.position= hit.point;				
			}
		}		
	}
	
	public void SetObjectToFixedPosition(GameObject target, Vector3 targetPos)
	{
		Ray raySelector=this.camera.ScreenPointToRay (targetPos); 
		if (Physics.Raycast(raySelector.origin, raySelector.direction,out hit, 300f,1<<31)){
			target.transform.position= hit.point;				
		}
	
		//target.transform.position = targetPos;
	}
    
}


