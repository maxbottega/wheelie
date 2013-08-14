using UnityEngine;

using System.Collections;



public class Car4WDControl : MonoBehaviour 

{

	public float m_EngineDrive;

	public Vector3 centerOfMass;

	public float m_FrontSuspensionSpring = 50;

	public float m_FrontSuspensionDamper = 2;

	public float m_FrontSuspensionHeight = 1;

	public float m_RearSuspensionSpring = 50;

	public float m_RearSuspensionDamper = 2;

	public float m_RearSuspensionHeight = 1;

	public bool  m_EngineTorqueTilt = true;

	public float m_TiltTorqueMultiplier = 1;

 	public float[] m_RPS = new float[4];

 	public float m_WheelFL_RPS;
//
 	public float m_WheelFR_RPS;
//
 	public float m_WheelBL_RPS;
//
 	public float m_WheelBR_RPS;
	
	public bool m_WheelFLGrounded;

 	public bool m_WheelFRGrounded;

 	public bool m_WheelBLGrounded;

 	public bool m_WheelBRGrounded;
	
	public float m_MaxAngularSpeed;



	
	

	public Joystick m_AcceleratorPad = null;

	public Joystick m_BrakePad = null;

	float m_fwdDrive = 0;
	
	float m_tiltInput = 0;

	private Transform[] wheels = null;

	private Transform[] wMesh = null;

	private WheelCollider[] colls = null;

	Vector3[] targetPosition =  new Vector3[4];


	

	// Use this for initialization

	void Start () 

	{

		Application.targetFrameRate = 60;

		wheels = new Transform[4];

		wheels[0] = transform.Find("WheelFL");

		wheels[1] = transform.Find("WheelFR");

		wheels[2] = transform.Find("WheelBL");

		wheels[3] = transform.Find("WheelBR");

		

		wMesh = new Transform[4];

		colls = new WheelCollider[4];

		

		for (int i = 0; i < 4; i++) 

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

		for (int i = 2; i < 4; i++) 

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

		const float brakeMult = 4;

		colls[0].brakeTorque = 0;

		colls[1].brakeTorque = 0;

		colls[2].brakeTorque = 0;

		colls[3].brakeTorque = 0;

		

		

		if (m_fwdDrive>=0)

		{

			if (colls[0].rpm < -1.0f)

			{

				colls[0].brakeTorque = m_fwdDrive * brakeMult;

				colls[1].brakeTorque = m_fwdDrive * brakeMult;

				colls[2].brakeTorque = m_fwdDrive * brakeMult;

				colls[3].brakeTorque = m_fwdDrive * brakeMult;

			}

			else

			{

				colls[0].motorTorque = m_fwdDrive;

				colls[1].motorTorque = m_fwdDrive;

				colls[2].motorTorque = m_fwdDrive;

				colls[3].motorTorque = m_fwdDrive;

			}	

		}

		else if (m_fwdDrive<0)

		{

			if (colls[0].rpm > 1.0f)

			{

				colls[0].brakeTorque = -m_fwdDrive * brakeMult;

				colls[1].brakeTorque = -m_fwdDrive * brakeMult;

				colls[2].brakeTorque = -m_fwdDrive * brakeMult;

				colls[3].brakeTorque = -m_fwdDrive * brakeMult;

			}

			else

			{

				colls[0].motorTorque = m_fwdDrive;

				colls[1].motorTorque = m_fwdDrive;

				colls[2].motorTorque = m_fwdDrive;

				colls[3].motorTorque = m_fwdDrive;

			}	

		}

		

		//Wheel rot-pos

		int numHits = 0;

		for (int i = 0; i < 4; i++) 

		{ 

			float scale = colls[i].transform.lossyScale.x;

			float radius = colls[i].radius * scale;

			

			//position

			WheelHit hit;
			
			m_WheelFLGrounded = m_WheelFRGrounded = m_WheelBLGrounded = m_WheelBRGrounded = false;

			if (colls[i].GetGroundHit(out hit))

			{

				Vector3 offSet = colls[i].transform.position - hit.point;

				Vector3 localPos = Vector3.zero;

				localPos.y -= Vector3.Dot(offSet, colls[i].transform.up) - radius;

				targetPosition[i] = localPos / scale;

				wMesh[i].transform.localPosition = Vector3.Lerp(wMesh[i].transform.localPosition, targetPosition[i], 0.5f);	

				++numHits;

				m_WheelFLGrounded = m_WheelFRGrounded = m_WheelBLGrounded = m_WheelBRGrounded = true;
			}

			else

			{	

				targetPosition[i] = colls[i].transform.position - colls[i].transform.up * colls[i].suspensionDistance * scale;

				wMesh[i].transform.position = Vector3.Lerp(wMesh[i].transform.position, targetPosition[i], 0.4f);

			}

			

//			//Rotation
//
			float rps = colls[i].rpm / 60.0f;

 			m_RPS[i] = rps;

			float angle = 360 * rps * Time.fixedDeltaTime;

			wMesh[i].Rotate(-Vector3.up * angle);

		}

		

//Accrocchio temporaneo x prendere gli RPS

 	m_WheelFL_RPS = m_RPS[0];
 
 	m_WheelFR_RPS = m_RPS[1];
 
 	m_WheelBL_RPS = m_RPS[2];
 
 	m_WheelBR_RPS = m_RPS[3];

		

		if (numHits==0 && m_EngineTorqueTilt)

		{	

			m_tiltInput = Input.GetAxis("Horizontal");

			//modificato perchè inverte il senso del tilt. Aggiunta variabile tiltInput per separare il tilt dall'engineDrive

			// (prima piu era veloce lka macchina piu si ruotava, ingestibile)

						

 			//rigidbody.AddRerlativeTorque(-m_fwdDrive*m_TiltTorqueMultiplier, 0, 0, ForceMode.Force);

			rigidbody.AddRelativeTorque(m_tiltInput*m_TiltTorqueMultiplier, 0, 0, ForceMode.Impulse);

			rigidbody.SetMaxAngularVelocity(m_MaxAngularSpeed);
			

		}

	}

}





