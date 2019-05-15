using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class TargetManagement : MonoBehaviour {
	// Public Variables
	public float range_ = 100f;

	// Private Variables
	private GameObject[] HostVehicle;
	private Rigidbody m_Rigidbody;

	// Use this for initialization
	void Start () {
		m_Rigidbody = GetComponent<Rigidbody> (); // At start, define rigidbody
	}
	
	// Update is called once per frame
	void FixedUpdate () {

//		collision_ = 0;
		// Delete Host vehicle which is out of range 

		// Get the Host vehicle GameObject
		HostVehicle = GameObject.FindGameObjectsWithTag ("Player");

		// Host vehicle coordinate
		float x_host = HostVehicle[0].transform.position.x;
		float z_host = HostVehicle[0].transform.position.z;

		// Target vehicle coordinate 
		float x_target = m_Rigidbody.position.x;
		float z_target = m_Rigidbody.position.z;

		// Calculate distance btw host vehicle and target vehicle
		float distance_ = Mathf.Sqrt((x_host - x_target) * (x_host - x_target) + (z_host - z_target) * (z_host - z_target));

		// If Vehicle is out of the range, destroy it
		if (distance_ > range_) {
			Object.Destroy (this.gameObject);
		}
			
	}

	// If other vehiclse collide each other, destroy themselves
	void OnTriggerEnter (Collider other)
	{
		if (other.gameObject.tag == "Other1" || 
			other.gameObject.tag == "Other2" ||
			other.gameObject.tag == "Other3" ||
			other.gameObject.tag == "Other4" ||
			other.gameObject.tag == "Other5" ||
			other.gameObject.tag == "Other6" ||
			other.gameObject.tag == "Other7" ||
			other.gameObject.tag == "Other8") 
		{
			Object.Destroy (this.gameObject);
		} 
	}

}
