using UnityEngine;
using System.Collections;

public class ModifyFrictionParameters : MonoBehaviour {
		
	public void ModifyFriction(float ExtrValue)
	{
		WheelCollider wc = gameObject.GetComponent<WheelCollider>();
		
		if(wc != null)
		{
			WheelFrictionCurve fri = wc.forwardFriction;
			
			fri.asymptoteValue = ExtrValue/2;
			fri.extremumValue = ExtrValue;
			
			wc.forwardFriction = fri;
			
		}			
		
		
	}
		
		
		
		
}
