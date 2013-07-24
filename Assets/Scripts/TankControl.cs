using UnityEngine;
using System.Collections;

public class TankControl : MonoBehaviour 
{
	public float m_EngineDrive;
	public Vector3 centerOfMass;
	public float m_FrontSuspensionSpring = 50;
	public float m_FrontSuspensionDamper = 2;
	public float m_FrontSuspensionHeight = 1;
	public float m_CenterSuspensionSpring = 50;
	public float m_CenterSuspensionDamper = 2;
	public float m_CenterSuspensionHeight = 1;
	public float m_RearSuspensionSpring = 50;
	public float m_RearSuspensionDamper = 2;
	public float m_RearSuspensionHeight = 1;
	public bool  m_EngineTorqueTilt = true;
	public float m_TiltTorqueMultiplier = 20;
	
	public Joystick m_AcceleratorPad = null;
	public Joystick m_BrakePad = null;
	
	private Transform[] wheels = null;
	private Transform[] wMesh = null;
	private WheelCollider[] colls = null;
	Vector3[] targetPosition =  new Vector3[6];
	float m_fwdDrive = 0;


	// Use this for initialization
	void Start () 
	{
		Application.targetFrameRate = 60;
		wheels = new Transform[6];
		wheels[0] = transform.Find("WheelFL");
		wheels[1] = transform.Find("WheelFR");
		wheels[2] = transform.Find("WheelBL");
		wheels[3] = transform.Find("WheelBR");
		wheels[4] = transform.Find("WheelCL");
		wheels[5] = transform.Find("WheelCR");
		
		wMesh = new Transform[6];
		colls = new WheelCollider[6];
		
		for (int i = 0; i < 6; i++) 
		{
			wMesh[i] = wheels[i].Find("WheelMesh");
			colls[i] = wheels[i].gameObject.GetComponent<WheelCollider>();
		}
		
		rigidbody.centerOfMass = centerOfMass;
	
	}
	
	void Update()
	{
		//Front suspensions Update
		JointSpring suspension = new JointSpring();
		suspension.spring = m_FrontSuspensionSpring;
		suspension.damper = m_FrontSuspensionDamper;
		
		for (int i = 0; i < 2; i++) 
		{	
			colls[i].suspensionSpring = suspension;
			colls[i].suspensionDistance = m_FrontSuspensionHeight;
		}
		
		suspension.spring = m_RearSuspensionSpring;
		suspension.damper = m_FrontSuspensionDamper;
		for (int i = 2; i < 6; i++) 
		{	
			colls[i].suspensionSpring = suspension;
			colls[i].suspensionDistance = m_RearSuspensionHeight;
		}
		
#if ((UNITY_IPHONE || UNITY_ANDROID) && !UNITY_EDITOR)
		m_fwdDrive = m_AcceleratorPad.tapCount > 0 ? m_EngineDrive : 0;
		m_fwdDrive = m_BrakePad.tapCount > 0 ? -m_EngineDrive : m_fwdDrive;
#elif UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_WEBPLAYER
		m_fwdDrive = Input.GetAxis("Horizontal") * m_EngineDrive;
#endif	
	}
	
	void FixedUpdate()
	{
		const float brakeMult = 6;
		colls[2].brakeTorque = 0;
		colls[3].brakeTorque = 0;
		
		if (m_fwdDrive>=0)
		{
			if (colls[2].rpm < -1.0f)
			{
				colls[2].brakeTorque = m_fwdDrive * brakeMult;
				colls[3].brakeTorque = m_fwdDrive * brakeMult;
			}
			else
			{
				colls[2].motorTorque = m_fwdDrive;
				colls[3].motorTorque = m_fwdDrive;
			}	
		}
		else if (m_fwdDrive<0)
		{
			if (colls[2].rpm > 1.0f)
			{
				colls[2].brakeTorque = -m_fwdDrive * brakeMult;
				colls[3].brakeTorque = -m_fwdDrive * brakeMult;
			}
			else
			{
				colls[2].motorTorque = m_fwdDrive;
				colls[3].motorTorque = m_fwdDrive;
			}	
		}
		
		//Wheel rot-pos
		int numHits = 0;
		for (int i = 0; i < 6; i++) 
		{ 
			float scale = colls[i].transform.lossyScale.x;
			float radius = colls[i].radius * scale;
			
			//position
			WheelHit hit;
			if (colls[i].GetGroundHit(out hit))
			{
				Vector3 offSet = colls[i].transform.position - hit.point;
				Vector3 localPos = Vector3.zero;
				localPos.y -= Vector3.Dot(offSet, colls[i].transform.up) - radius;
				targetPosition[i] = localPos / scale;
				wMesh[i].transform.localPosition = Vector3.Lerp(wMesh[i].transform.localPosition, targetPosition[i], 0.5f);	
				++numHits;

			}
			else
			{	
				targetPosition[i] = colls[i].transform.position - colls[i].transform.up * colls[i].suspensionDistance * scale;
				wMesh[i].transform.position = Vector3.Lerp(wMesh[i].transform.position, targetPosition[i], 0.4f);
			}
			
			//Rotation
			float rps = colls[i].rpm / 60.0f;
			float angle = 360 * rps * Time.fixedDeltaTime;
			wMesh[i].Rotate(-Vector3.up * angle);
		}
		
		if (numHits==0 && m_EngineTorqueTilt)
		{
			rigidbody.AddRelativeTorque(-m_fwdDrive*m_TiltTorqueMultiplier, 0, 0, ForceMode.Force);
			rigidbody.SetMaxAngularVelocity(10);
		}
	}
}


