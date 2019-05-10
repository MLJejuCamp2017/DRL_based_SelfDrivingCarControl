using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamController : MonoBehaviour {

	public GameObject agentVehicle;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		Vector3 camPosition = new Vector3 (this.transform.position.x, this.transform.position.y, agentVehicle.transform.position.z);
		this.transform.position = camPosition;
	}
}
