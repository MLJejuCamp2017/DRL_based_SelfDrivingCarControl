using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Vehicle6_Manager : MonoBehaviour
{
	public GameObject vehicle1; // Which one we are gonna spawn
	public GameObject vehicle2;
	public GameObject vehicle3;
	public GameObject vehicle4;
	public GameObject vehicle5;
	public GameObject vehicle6;
	public GameObject vehicle7;
	public GameObject vehicle8;

	public GameObject Agent;

	public int X_margin = 7;
	public int Y_margin = 10;

	public int WaitTime_min = 2;
	public int WaitTime_max = 8;

	public Vector3 SpawnVector;

	public Transform[] spawnPoints; // If I set the point, enemy will spawn in the exact point

	private float sum_time = 0f;
	private float wait_time;

    private float Num_vehicles;

    private GameObject[] vehicle1_obj; // Count the number of other vehicles
	private GameObject[] vehicle2_obj;
	private GameObject[] vehicle3_obj;
	private GameObject[] vehicle4_obj;
	private GameObject[] vehicle5_obj;
	private GameObject[] vehicle6_obj;
	private GameObject[] vehicle7_obj;
	private GameObject[] vehicle8_obj;

	private bool IsOthers = false;

	private void FixedUpdate()
	{
		IsOthers = false;

		sum_time += Time.deltaTime;

        // Get values from slider
        float sliderval = GameObject.FindGameObjectWithTag("VehicleSlider").GetComponent<Slider>().value;

        Num_vehicles = Mathf.Floor(sliderval / 8f);

        if (sliderval % 8 > 5)
        {
            Num_vehicles = Num_vehicles + 1;
        }

        wait_time = Random.Range (WaitTime_min, WaitTime_max);
		if (sum_time > wait_time)
		{
			sum_time = 0;
			// Current position of host vehicle
//			float x_host = spawnPoints[0].position.x;
			float y_host = spawnPoints[0].position.y;
			float z_host = spawnPoints[0].position.z;

			// Determine lane of other vehicles
			int other_lane = 0;

			other_lane = Random.Range (1, 6);

//			if (host_lane == 1) {
//				other_lane = Random.Range (1, 6);
//			} else if (host_lane == 5) {
//				other_lane = Random.Range (1, 6);
//			} else {
//				other_lane = Random.Range (1, 6);
//			}

			// Determine lateral position of other vehicles
			int other_x = 0;

			if (other_lane == 1) {
				other_x = -18;
			} else if (other_lane == 2) {
				other_x = -11;
			} else if (other_lane == 3) {
				other_x = -4;
			} else if (other_lane == 4) {
				other_x = 3;
			} else {
				other_x = 10;
			}

			// Determine longitudinal position of other vehicles
			int other_z = 0;
			
			if (Random.Range (0, 1f) < 0.7f) {
				// Positive longitudinal position
				other_z = Random.Range (30, 90);
			} else {
				other_z = Random.Range (-30, -60);
			}

			// Do not spawn other vehicles when it's over 3
			vehicle1_obj = GameObject.FindGameObjectsWithTag (vehicle1.tag);
			vehicle2_obj = GameObject.FindGameObjectsWithTag (vehicle2.tag);
			vehicle3_obj = GameObject.FindGameObjectsWithTag (vehicle3.tag);
			vehicle4_obj = GameObject.FindGameObjectsWithTag (vehicle4.tag);
			vehicle5_obj = GameObject.FindGameObjectsWithTag (vehicle5.tag);
			vehicle6_obj = GameObject.FindGameObjectsWithTag (vehicle6.tag);
			vehicle7_obj = GameObject.FindGameObjectsWithTag (vehicle7.tag);
			vehicle8_obj = GameObject.FindGameObjectsWithTag (vehicle8.tag);
	
			SpawnVector = new Vector3 (other_x, y_host, z_host + other_z);

			IsOthers = IsOtherVehicle (vehicle1_obj, SpawnVector);
			IsOthers = IsOthers || IsOtherVehicle (vehicle2_obj, SpawnVector);
			IsOthers = IsOthers || IsOtherVehicle (vehicle3_obj, SpawnVector);
			IsOthers = IsOthers || IsOtherVehicle (vehicle4_obj, SpawnVector);
			IsOthers = IsOthers || IsOtherVehicle (vehicle5_obj, SpawnVector);
			IsOthers = IsOthers || IsOtherVehicle (vehicle6_obj, SpawnVector);
			IsOthers = IsOthers || IsOtherVehicle (vehicle7_obj, SpawnVector);
			IsOthers = IsOthers || IsOtherVehicle (vehicle8_obj, SpawnVector);

			if (vehicle6_obj.Length < Num_vehicles && IsOthers == false) {
				// Set spawn position
				Vector3 spawn_position = SpawnVector;
				GameObject spawned_vehicle = Instantiate (vehicle6, spawn_position, Quaternion.identity);		
				spawned_vehicle.transform.parent = Agent.transform;	
			}

		}
        if (vehicle6_obj != null)
        {
            if (vehicle6_obj.Length > Num_vehicles)
            {
                Destroy(vehicle6_obj[0]);
            }
        }
    }

	private bool IsOtherVehicle(GameObject[] vehicle_obj, Vector3 SpawnVector)
	{
		bool other = false;
		float obj_x = 0;
		float obj_z = 0;

		if (vehicle_obj.Length > 0) {
			for (int i = 0; i < vehicle_obj.Length; i++) {
				obj_x = vehicle_obj [i].transform.position.x;
				obj_z = vehicle_obj [i].transform.position.z;

				if (SpawnVector.x > obj_x - X_margin && SpawnVector.x < obj_x + X_margin && SpawnVector.z - Y_margin < obj_z && SpawnVector.z + Y_margin > obj_z) {
					other = true;
				}
			}
		}

		return other;
	}
}