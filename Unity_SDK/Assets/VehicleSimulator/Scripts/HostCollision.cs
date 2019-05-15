using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HostCollision : MonoBehaviour {

	public bool collision_check = false;

	// Use this for initialization
	void Start () {
		collision_check = false;		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		
	}

	void OnTriggerEnter (Collider other)
	{
		if (other.gameObject.tag == "Other1" ||
		    other.gameObject.tag == "Other2" ||
		    other.gameObject.tag == "Other3" ||
		    other.gameObject.tag == "Other4" ||
		    other.gameObject.tag == "Other5" ||
		    other.gameObject.tag == "Other6" ||
		    other.gameObject.tag == "Other7" ||
		    other.gameObject.tag == "Other8") {

			collision_check = true;
		} else {
			collision_check = false;
		}
	}
}
