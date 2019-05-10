using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class VehicleAgent : Agent
{

	public float position;
	public GameObject HostVehicle;

	// Action related information
	private float vehicle_speed_max = 0.8f;
	private float vehicle_speed_min = 0.4f;

	public float vehicle_speed = 0.6f;

	private int target_lane = 2;

	// Lane Information
	private int current_lane = 2;
	private int current_lane_old = 2;
	private float lane_center = 0;

	private List<float> laneCenter_list = new List<float>();

	// Reward related information
	private float collision_reward = -10f;
	private float lateral_penalty = 0f;

	private float lat_reward = 0f;
	private float lon_reward = 0f;
    private float overtake_reward = 0f;
    private float reward_violation = 0f;

    // info for display
    public float dist_vehicle = 0f;

	// Variables of Other vehicle
	private GameObject[] others;

	// Variables for collision
	private bool col_check = false;
	private bool col_check_old = false;

	// List for getting LIDAR data
	private List<float> LIDAR_data = new List<float>();

	// Count lane change
	private int count_laneChange = 0;

	// For counting overtake
	public int num_overtake = 0;
	private int num_overtake_old = 0;

	private float host_z = 0f;
	private float host_z_old = 0f; 

	private GameObject[] vehicle1_obj; // Count the number of other vehicles
	private GameObject[] vehicle2_obj;
	private GameObject[] vehicle3_obj;
	private GameObject[] vehicle4_obj;
	private GameObject[] vehicle5_obj;
	private GameObject[] vehicle6_obj;
	private GameObject[] vehicle7_obj;
	private GameObject[] vehicle8_obj;

	private float other_z = 0f;
	private List<float> other1_z_old = new List<float>();
	private List<float> other2_z_old = new List<float>();
	private List<float> other3_z_old = new List<float>();
	private List<float> other4_z_old = new List<float>();
	private List<float> other5_z_old = new List<float>();
	private List<float> other6_z_old = new List<float>();
	private List<float> other7_z_old = new List<float>();
	private List<float> other8_z_old = new List<float>();


	void Start()
	{
		laneCenter_list.Add (-18);
		laneCenter_list.Add (-11);
		laneCenter_list.Add (-4);
		laneCenter_list.Add (3);
		laneCenter_list.Add (10);
	}

	public override void CollectObservations()
	{
		LIDAR_data = HostVehicle.gameObject.GetComponent<HostSensors> ().Range_list;

		foreach (float i in LIDAR_data) {
			AddVectorObs (i / 80f);
		}

		// Get sensor data
		bool Left_warning = false;
		bool Right_warning = false;
		float forward_dist = 0f;
		float forward_vel = 0f;
		bool Forward_warning = false;

		Left_warning = HostVehicle.gameObject.GetComponent<HostSensors> ().Left_warning;
		Right_warning= HostVehicle.gameObject.GetComponent<HostSensors> ().Right_warning;
		forward_dist = HostVehicle.gameObject.GetComponent<HostSensors> ().forward_distance;
		forward_vel = HostVehicle.gameObject.GetComponent<HostSensors> ().forward_velocity;

		if (forward_dist < vehicle_speed * 15 && forward_dist != 0) {
			Forward_warning = true;
		}

		if (Left_warning == true) {
			AddVectorObs (1f);
		} else {
			AddVectorObs (0f);
		}

		if (Right_warning == true) {
			AddVectorObs (1f);
		} else {
			AddVectorObs (0f);
		}

		if (Forward_warning == true) {
			AddVectorObs (1f);
		} else {
			AddVectorObs (0f);
		}

		AddVectorObs (forward_dist / 40f);
		AddVectorObs (forward_vel);

		AddVectorObs (vehicle_speed);

		// Send for plotting info.
		AddVectorObs(num_overtake);
		AddVectorObs (count_laneChange);

        // Send rewards
        AddVectorObs(lon_reward);
        AddVectorObs(lat_reward);
        AddVectorObs(overtake_reward);
        AddVectorObs(reward_violation);
        AddVectorObs(collision_reward);
	}

	public override void AgentAction(float[] vectorAction, string textAction)
	{
		float movement = vectorAction[0];

		float host_x = HostVehicle.transform.position.x;
		float host_y = HostVehicle.transform.position.y;
		float host_z = HostVehicle.transform.position.z;

		/* Get Lane Information
		 * left most lane: 1, right most lane: 5
		 * lateral_penalty: lateral error from current lane center and vehicle lateral position
		 */

		if (host_x < (laneCenter_list[0] + laneCenter_list[1]) / 2) {
			current_lane = 0;
			lane_center = laneCenter_list[0];
			lateral_penalty = Mathf.Abs (host_x - lane_center);
		} else if (host_x < (laneCenter_list[1] + laneCenter_list[2]) / 2) {
			current_lane = 1;
			lane_center = laneCenter_list[1];
			lateral_penalty = Mathf.Abs (host_x - lane_center);
		} else if (host_x < (laneCenter_list[2] + laneCenter_list[3]) / 2) {
			current_lane = 2;
			lane_center = laneCenter_list[2];
			lateral_penalty = Mathf.Abs (host_x - lane_center);
		} else if (host_x < (laneCenter_list[3] + laneCenter_list[4]) / 2) {
			current_lane = 3;
			lane_center = laneCenter_list[3];
			lateral_penalty = Mathf.Abs (host_x - lane_center);
		} else {
			current_lane = 4;
			lane_center = laneCenter_list[4];
			lateral_penalty = Mathf.Abs (host_x - lane_center);
		}

		if (current_lane != current_lane_old) {
			count_laneChange = count_laneChange + 1;
		}

		current_lane_old = current_lane;

		LaneText.lane = current_lane;

		// Get sensor data
		bool Left_warning = false;
		bool Right_warning = false;
		float forward_dist = 0f;
		float forward_vel = 0f;
		bool Forward_warning = false;

		Left_warning = HostVehicle.gameObject.GetComponent<HostSensors> ().Left_warning;
		Right_warning= HostVehicle.gameObject.GetComponent<HostSensors> ().Right_warning;
		forward_dist = HostVehicle.gameObject.GetComponent<HostSensors> ().forward_distance;
		forward_vel = HostVehicle.gameObject.GetComponent<HostSensors> ().forward_velocity;

		if (forward_dist < vehicle_speed * 15 && forward_dist != 0) {
			Forward_warning = true;
		}

		// Reward function
		float reward = 0;
		lat_reward = 0;

		// Control the vehicle (Lateral)
		if ((movement == 0) && (host_x > -18) && (Left_warning == false)) {
			target_lane = target_lane - 1;
			lat_reward = lat_reward - 0.5f;
		} else if ((movement == 1) && (host_x < 10) && (Right_warning == false)) {
			target_lane = target_lane + 1;
			lat_reward = lat_reward - 0.5f;
		} else if (movement == 2) {
//			target_lane = current_lane;
//			HostVehicle.transform.Translate (Vector3.right * 0);
		}

		// Constraint (lateral)
		if (target_lane < current_lane - 1) {
			target_lane = current_lane - 1;
		}

		if (target_lane > current_lane + 1) {
			target_lane = current_lane + 1;
		}

		if (target_lane < 0) {
			target_lane = 0;
		}

		if (target_lane > 4) {
			target_lane = 4;
		}

		float target_lane_diff = host_x - laneCenter_list [target_lane];

		if (laneCenter_list[target_lane] + 0.3 < host_x) {
			HostVehicle.transform.Translate (Vector3.right * -0.5f * vehicle_speed);
		} 

		if (laneCenter_list[target_lane] - 0.3 > host_x) {
			HostVehicle.transform.Translate (Vector3.right * 0.5f * vehicle_speed);
		} 		

		// Control the vehicle (Longitudinal)
		if (Forward_warning == true) {

			// Cruise Control
			float k_p = 0.002f;
			float k_i = 0.002f;

			vehicle_speed -= (k_p * (vehicle_speed - forward_vel) + k_i * (vehicle_speed - forward_vel) / Time.deltaTime);

			if (movement == 4) {
				vehicle_speed = vehicle_speed - 0.05f;
			}

		} else {
			if (movement == 3) {
				vehicle_speed = vehicle_speed + 0.05f;
			} else if (movement == 4) {
				vehicle_speed = vehicle_speed - 0.05f;
			}
		}

		// Constraint (longitudinal)
		if (vehicle_speed > vehicle_speed_max) {
			vehicle_speed = vehicle_speed_max;
		}

		if (vehicle_speed < vehicle_speed_min) {
			vehicle_speed = vehicle_speed_min;
		}

		HostVehicle.transform.Translate (Vector3.forward * vehicle_speed);

        // Count num overtake
        num_overtake = countOvertake ();

		// Reward function
		lon_reward = ((vehicle_speed - vehicle_speed_min) / (vehicle_speed_max - vehicle_speed_min)); // max = 1
		overtake_reward = 0.5f * (num_overtake - num_overtake_old); // 1 overtake = 0.5 
		reward = (lon_reward + lat_reward + overtake_reward);

		reward_violation = 0.1f;

		if (Forward_warning == true && movement == 3) {
			reward = reward - reward_violation;	
		}

		if (Left_warning == true && movement == 0) {
			reward = reward - reward_violation;	
		}

		if (Right_warning == true && movement == 1) {
			reward = reward - reward_violation;	
		}

		// Send vehicle information to texts
		SpeedText.speed = 100f * vehicle_speed;
		FrontWarningText.front_warning = Forward_warning;
		LeftWarningText.left_warning = Left_warning;
		RightWarningText.right_warning = Right_warning;
		FrontDistText.front_dist = forward_dist;
		RewardText.reward = reward;
		OvertakeText.count_overtake = num_overtake;

		// Done when over the boundary
		dist_vehicle = host_z + 380;
		ProgressText.progress = (dist_vehicle);

		// Terminal states
		if (dist_vehicle > 2600) {
			Done();
		}

		col_check = HostVehicle.GetComponent<HostCollision> ().collision_check;

		if (col_check == true && col_check_old == true) {
			col_check = false;
		}

		if (col_check == true && host_z > -360) {
			AddReward (collision_reward);
			Done();
		}
		AddReward(reward);

		col_check_old = col_check;
        num_overtake_old = num_overtake;
	}

	public override void AgentReset()
	{
		// Destroy all other vehicles
		string[] other_vehicles = {"Other1", "Other2", "Other3", "Other4", "Other5", "Other6", "Other7", "Other8"};

		for (int i=0; i < other_vehicles.Length; i++) {
			others = GameObject.FindGameObjectsWithTag (other_vehicles[i]);
			for (int j = 0; j < others.Length; j++) {
				Object.Destroy (others[j]);
			}
		}

		// Initialize position
		HostVehicle.transform.position = new Vector3(-4f, 0f, -380f);
		vehicle_speed = 0.6f;
		target_lane = 2;
		current_lane = 2;
		current_lane_old = 2;
        num_overtake = 0;
        num_overtake_old = 0;
		count_laneChange = 0;
		
	}

	public override void AgentOnDone()
	{

	}

	private int countOvertake() {
		// Count overtake
		float host_z = HostVehicle.transform.position.z;

		// Get other vehicle objects
		vehicle1_obj = GameObject.FindGameObjectsWithTag ("Other1");
		vehicle2_obj = GameObject.FindGameObjectsWithTag ("Other2");
		vehicle3_obj = GameObject.FindGameObjectsWithTag ("Other3");
		vehicle4_obj = GameObject.FindGameObjectsWithTag ("Other4");
		vehicle5_obj = GameObject.FindGameObjectsWithTag ("Other5");
		vehicle6_obj = GameObject.FindGameObjectsWithTag ("Other6");
		vehicle7_obj = GameObject.FindGameObjectsWithTag ("Other7");
		vehicle8_obj = GameObject.FindGameObjectsWithTag ("Other8");

        // Count number of overtakes
        if (vehicle1_obj.Length == 0) {
			other1_z_old.Clear ();
			other1_z_old.Add (host_z);
		}

		for (int i = 0; i < vehicle1_obj.Length; i++) {

			other_z = vehicle1_obj [i].transform.position.z;

			if (i + 1 > other1_z_old.Count) {
				other1_z_old.Add (other_z);
			} else if (vehicle1_obj.Length < other1_z_old.Count) {
				other1_z_old.RemoveAt (0);
			}

			if ((other_z < host_z) && (other1_z_old [i] > host_z_old)) {
                num_overtake = num_overtake + 1;
			}

			if ((other_z > host_z) && (other1_z_old [i] < host_z_old)) {
                num_overtake = num_overtake - 1;
			}

			other1_z_old[i] = other_z;
		}

		if (vehicle2_obj.Length == 0) {
			other2_z_old.Clear ();
			other2_z_old.Add (host_z);
		}

		for (int i = 0; i < vehicle2_obj.Length; i++) {

			other_z = vehicle2_obj [i].transform.position.z;

			if (i + 1 > other2_z_old.Count) {
				other2_z_old.Add (other_z);
			} else if (vehicle2_obj.Length < other2_z_old.Count) {
				other2_z_old.RemoveAt (0);
			}

			if ((other_z < host_z) && (other2_z_old [i] > host_z_old)) {
                num_overtake = num_overtake + 1;
			}

			if ((other_z > host_z) && (other2_z_old [i] < host_z_old)) {
                num_overtake = num_overtake - 1;
			}

			other2_z_old[i] = other_z;
		} 

		if (vehicle3_obj.Length == 0) {
			other3_z_old.Clear ();
			other3_z_old.Add (host_z);
		}

		for (int i = 0; i < vehicle3_obj.Length; i++) {

			other_z = vehicle3_obj [i].transform.position.z;

			if (i + 1 > other3_z_old.Count) {
				other3_z_old.Add (other_z);
			} else if (vehicle3_obj.Length < other3_z_old.Count) {
				other3_z_old.RemoveAt (0);
			}

			if ((other_z < host_z) && (other3_z_old [i] > host_z_old)) {
                num_overtake = num_overtake + 1;
			}

			if ((other_z > host_z) && (other3_z_old [i] < host_z_old)) {
                num_overtake = num_overtake - 1;
			}

			other3_z_old[i] = other_z;
		} 

		if (vehicle4_obj.Length == 0) {
			other4_z_old.Clear ();
			other4_z_old.Add (host_z);
		}

		for (int i = 0; i < vehicle4_obj.Length; i++) {

			other_z = vehicle4_obj [i].transform.position.z;

			if (i + 1 > other4_z_old.Count) {
				other4_z_old.Add (other_z);
			} else if (vehicle4_obj.Length < other4_z_old.Count) {
				other4_z_old.RemoveAt (0);
			}

			if ((other_z < host_z) && (other4_z_old [i] > host_z_old)) {
                num_overtake = num_overtake + 1;
			}

			if ((other_z > host_z) && (other4_z_old [i] < host_z_old)) {
                num_overtake = num_overtake - 1;
			}

			other4_z_old[i] = other_z;
		} 

		if (vehicle5_obj.Length == 0) {
			other5_z_old.Clear ();
			other5_z_old.Add (host_z);
		}

		for (int i = 0; i < vehicle5_obj.Length; i++) {

			other_z = vehicle5_obj [i].transform.position.z;

			if (i + 1 > other5_z_old.Count) {
				other5_z_old.Add (other_z);
			} else if (vehicle5_obj.Length < other5_z_old.Count) {
				other5_z_old.RemoveAt (0);
			}

			if ((other_z < host_z) && (other5_z_old [i] > host_z_old)) {
                num_overtake = num_overtake + 1;
			}

			if ((other_z > host_z) && (other5_z_old [i] < host_z_old)) {
                num_overtake = num_overtake - 1;
			}

			other5_z_old[i] = other_z;
		} 

		if (vehicle6_obj.Length == 0) {
			other6_z_old.Clear ();
			other6_z_old.Add (host_z);
		}

		for (int i = 0; i < vehicle6_obj.Length; i++) {

			other_z = vehicle6_obj [i].transform.position.z;

			if (i + 1 > other6_z_old.Count) {
				other6_z_old.Add (other_z);
			} else if (vehicle6_obj.Length < other6_z_old.Count) {
				other6_z_old.RemoveAt (0);
			}

			if ((other_z < host_z) && (other6_z_old [i] > host_z_old)) {
                num_overtake = num_overtake + 1;
			}

			if ((other_z > host_z) && (other6_z_old [i] < host_z_old)) {
                num_overtake = num_overtake - 1;
			}

			other6_z_old[i] = other_z;
		} 

		if (vehicle7_obj.Length == 0) {
			other7_z_old.Clear ();
			other7_z_old.Add (host_z);
		}

		for (int i = 0; i < vehicle7_obj.Length; i++) {

			other_z = vehicle7_obj [i].transform.position.z;

			if (i + 1 > other7_z_old.Count) {
				other7_z_old.Add (other_z);
			} else if (vehicle7_obj.Length < other7_z_old.Count) {
				other7_z_old.RemoveAt (0);
			}

			if ((other_z < host_z) && (other7_z_old [i] > host_z_old)) {
                num_overtake = num_overtake + 1;
			}

			if ((other_z > host_z) && (other7_z_old [i] < host_z_old)) {
                num_overtake = num_overtake - 1;
			}

			other7_z_old[i] = other_z;
		} 

		if (vehicle8_obj.Length == 0) {
			other8_z_old.Clear ();
			other8_z_old.Add (host_z);
		}

		for (int i = 0; i < vehicle8_obj.Length; i++) {

			other_z = vehicle8_obj [i].transform.position.z;

			if (i + 1 > other8_z_old.Count) {
				other8_z_old.Add (other_z);
			} else if (vehicle8_obj.Length < other8_z_old.Count) {
				other8_z_old.RemoveAt (0);
			}

			if ((other_z < host_z) && (other8_z_old [i] > host_z_old)) {
                num_overtake = num_overtake + 1;
			}

			if ((other_z > host_z) && (other8_z_old [i] < host_z_old)) {
                num_overtake = num_overtake - 1;
			}

			other8_z_old[i] = other_z;
		} 



		if (Mathf.Abs (host_z - host_z_old) > 50) {
            num_overtake = 0;
		}

		host_z_old = host_z;

		return num_overtake;
	}
}
