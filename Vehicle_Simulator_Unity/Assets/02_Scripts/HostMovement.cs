using UnityEngine;
using System.Collections;

public class script : MonoBehaviour {

	public float m_Speed = 10f;
	public float m_TurnSpeed = 180f;

	private string m_MovementAxisName = "Vertical";
	private string m_TurnAxisName = "Horizontal";

	private Rigidbody m_Rigidbody;
	private float m_MovementInputValue;
	private float m_TurnInputValue;

	// Use this for initialization
	private void Start() 
	{
		m_Rigidbody = GetComponent<Rigidbody> (); // At start, define rigidbody
	}

	// Update is called once per frame
	private void Update() 
	{
		// get the value of movement input and rotation input
		m_MovementInputValue = Input.GetAxis(m_MovementAxisName);
		m_TurnInputValue = Input.GetAxis(m_TurnAxisName);

//		Debug.Log (m_MovementInputValue);

		Move ();
		Turn ();
	}

	private void Move()
	{
		Vector3 movement = transform.forward * m_MovementInputValue * m_Speed * Time.deltaTime;
		m_Rigidbody.MovePosition (m_Rigidbody.position + movement);
	}

	private void Turn()
	{
		float turn = m_TurnInputValue * m_TurnSpeed * Time.deltaTime;
		Quaternion turnRotation = Quaternion.Euler (0f, turn, 0f);
		m_Rigidbody.MoveRotation(m_Rigidbody.rotation * turnRotation);
	}

}
