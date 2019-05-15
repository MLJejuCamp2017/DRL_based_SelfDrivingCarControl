using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FogScript : MonoBehaviour {
    public GameObject Fog1;
    public GameObject Fog2;
    public GameObject Fog3;
    public GameObject Fog4;

    public GameObject HostVehicle;

    public Dropdown FogDrop;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        float host_x = HostVehicle.transform.position.x;
        float host_y = HostVehicle.transform.position.y;
        float host_z = HostVehicle.transform.position.z;

        int FogVal = FogDrop.value;
        
        if(FogVal == 0)
        {
            Fog1.SetActive(false);
            Fog2.SetActive(false);
            Fog3.SetActive(false);
            Fog4.SetActive(false);
        }
        else
        {
            Fog1.SetActive(true);
            Fog2.SetActive(true);
            Fog3.SetActive(true);
            Fog4.SetActive(true);
        }

        Fog1.transform.position = new Vector3(5f, host_y, host_z + 15f);
        Fog2.transform.position = new Vector3(-5f, host_y, host_z + 15f);
        Fog3.transform.position = new Vector3(5f, host_y, host_z - 15f);
        Fog4.transform.position = new Vector3(-5f, host_y, host_z - 15f);

    }
}
