using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamManager : MonoBehaviour {

	public GameObject HostVehicle;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = new Vector3 (-4, 40, HostVehicle.transform.position.z); 
	}
}
