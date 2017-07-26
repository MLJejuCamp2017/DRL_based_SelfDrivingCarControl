using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrontCamManager : MonoBehaviour {

	//public GameObject HostVehicle;
	private GameObject HostVehicle;

	// Use this for initialization
	void Start () {
		//DontDestroyOnLoad (this.gameObject);
		HostVehicle = GameObject.FindGameObjectWithTag("Player");
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = new Vector3 (HostVehicle.transform.position.x, 2, HostVehicle.transform.position.z + 4); 
	}
}
