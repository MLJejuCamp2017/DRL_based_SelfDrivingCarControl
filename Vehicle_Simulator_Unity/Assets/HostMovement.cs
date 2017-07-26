using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class HostMovement : MonoBehaviour {

	public float m_Speed = 20f;
	public float terminal = 0; 
	public float reward;
	public int Left_ADAS = 0;
	public int Right_ADAS = 0;
	public int Front_ADAS = 0;
	public float Vehicle_x = 0f;
	public int Left_Changing  = 0;
	public int Right_Changing = 0;

	private float lateral_speed;

	private Rigidbody m_Rigidbody;
	private float m_VerticalMovementInputValue;

	// Longitudinal Variable
	private float max_speed = 40f;
	private float min_speed = 20f;

	//private float max_forward_dist = 15f;
	//private float min_forward_dist = 5f;

	private float forward_dist_old = 0f;
	private float forward_vel = 0f;

	//private float forward_control_dist = 10;
	//private bool forward_warning_check = false;

	// Lateral Variable
	private bool Is_right_lane_change = false;
	private bool Is_left_lane_change = false;
	private float start_y_left = 0;
	private float start_y_right = 0;

	private float lane1_x = -18;
	private float lane2_x = -11;
	private float lane3_x = -4;
	private float lane4_x = 3;
	private float lane5_x = 10;

	private int current_lane = 3;


	public int action { get; set; }
	//public float Acceleration { get; set; }

	public List<float> Range_list = new List<float>();

	private int Disconnect_check = 0;

	// Use this for initialization
	private void Start() 
	{
		//DontDestroyOnLoad(this.gameObject);
		m_Rigidbody = GetComponent<Rigidbody> (); // At start, define rigidbody
	}

	// Update is called once per frame
	private void Update() 
	{
		//action = 3;
		terminal = 0;
		reward = m_Speed/10;
		Range_list = this.gameObject.GetComponent<HostController> ().Range_list;

		////////////////////// Longitudinal Move //////////////////////
		lateral_speed = m_Speed * 0.25f;

		bool Left_warning = false;
		bool Right_warning = false;
		float forward_dist = 0f;
		bool Forward_warning = false;

		Left_warning = this.gameObject.GetComponent<HostController> ().Left_warning;
		Right_warning= this.gameObject.GetComponent<HostController> ().Right_warning;
		forward_dist = this.gameObject.GetComponent<HostController> ().forward_distance;

		// Forward distance controller 
		float forward_threshold = 5f + (m_Speed / 3f);
		float k_p = 0.02f;
		float k_i = 0.0001f;

		// AEB like motion
		if (forward_dist < 3f) {
			k_p = 0.1f;
		}
			
		if ((forward_dist < forward_threshold) && (forward_dist != 0)) {
			Forward_warning = true;
			forward_dist_old = forward_dist;
			forward_vel = m_Speed;
		}

		////////////////////// No ADAS code //////////////////////
		Forward_warning = false;
		Left_warning = false; 
		Right_warning = false; 

		////////////////////// Longitudinal Move //////////////////////
		if (Forward_warning == false) {
//			if (Input.GetKeyDown("up") && m_Speed < max_speed) {
			if (action == 1 && m_Speed < max_speed) {
				m_Speed += 1;
			}

			//if (Input.GetKeyDown("down") && m_Speed > min_speed) {
			if (action == 2 && m_Speed > min_speed) {
				m_Speed -= 1;
			}
		} else {
			forward_vel = (forward_dist - forward_dist_old) / Time.deltaTime;
			m_Speed -= (k_p * (m_Speed - forward_vel) + k_i * (m_Speed - forward_vel) / Time.deltaTime);
			forward_dist_old = forward_dist;

			//if (Input.GetKeyDown("up") && m_Speed < max_speed) {
			if (action == 1 && m_Speed < max_speed) {
				m_Speed += 1;
			}

//			if (Input.GetKeyDown("down") && m_Speed > min_speed) {
			if (action == 2 && m_Speed > min_speed) {
				m_Speed -= 1;
			}

		}
			
		if (m_Speed > max_speed) {
			m_Speed = max_speed;
		} 

		if (m_Speed < min_speed) {
			m_Speed = min_speed;
		}

		// Return speed to text (SpeedText)
		SpeedText.speed = m_Speed;

		VerticalMove();

		////////////////////// Lateral Move //////////////////////

		if (m_Rigidbody.position.x < (lane1_x + lane2_x) / 2) {
			current_lane = 1;
		} else if (m_Rigidbody.position.x < (lane2_x + lane3_x) / 2) {
			current_lane = 2;
		} else if (m_Rigidbody.position.x < (lane3_x + lane4_x) / 2) {
			current_lane = 3;
		} else if (m_Rigidbody.position.x < (lane4_x + lane5_x) / 2) {
			current_lane = 4;
		} else {
			current_lane = 5;
		}

		LaneText.lane = current_lane;

//		float target_lane_x = 0f;
//		if (action == 3) {
//			if (m_Rigidbody.position.x < lane2_x) {
//				target_lane_x = lane1_x;
//			} else if (m_Rigidbody.position.x < lane3_x) {
//				target_lane_x = lane2_x;
//			} else if (m_Rigidbody.position.x < lane4_x) {
//				target_lane_x = lane3_x;
//			} else {
//				target_lane_x = lane4_x;
//			}
//		}
//
//		if (action == 4) {
//			if (m_Rigidbody.position.x > lane4_x) {
//				target_lane_x = lane5_x;
//			} else if (m_Rigidbody.position.x > lane3_x) {
//				target_lane_x = lane4_x;
//			} else if (m_Rigidbody.position.x > lane2_x) {
//				target_lane_x = lane3_x;
//			} else {
//				target_lane_x = lane2_x;
//			}
//		}

		// Right Change
//		if (Input.GetKeyDown("right") && Is_right_lane_change == false && m_Rigidbody.position.x < 10 && Right_warning == false) 
		if (action == 4 && Is_right_lane_change == false && current_lane != 5 && Right_warning == false) 
		{
			Is_right_lane_change = true;
			start_y_right = m_Rigidbody.position.x;

			// Lane change -> - reward
		}

		// If vehicle is changing to left, there is no right change
		if (Is_left_lane_change == true) {
			Is_right_lane_change = false;
		}

		if (Is_right_lane_change == true) 
		{
			// Lane change -> - reward
			reward -= 1;

			Vector3 movement = transform.right * lateral_speed * Time.deltaTime;
			m_Rigidbody.MovePosition (m_Rigidbody.position + movement);
			if (Abs(m_Rigidbody.position.x - start_y_right) >= 7) 
			{
			Is_right_lane_change = false;
			}
		}

		// Left Change
//		if (Input.GetKeyDown("left") && Is_left_lane_change == false && m_Rigidbody.position.x > -18 && Left_warning == false) 
		if (action == 3 && Is_left_lane_change == false && current_lane != 1 && Left_warning == false) 
		{
			reward -= 1;
			Is_left_lane_change = true;
			start_y_left = m_Rigidbody.position.x;

		}

		// If vehicle is changing to right, there is no left change 
		if (Is_right_lane_change == true) {
			Is_left_lane_change = false;
		}

		if (Is_left_lane_change == true) 
		{
			// Lane change -> - reward
			reward -= 1;
			Vector3 movement = transform.right * lateral_speed * Time.deltaTime;
			m_Rigidbody.MovePosition (m_Rigidbody.position - movement);
			if (Abs(start_y_left - m_Rigidbody.position.x) >= 7) 
			{
				Is_left_lane_change = false;
			}
		}

		// Move to center lane if it is not changing the lane
		float current_lane_x = 0;
		if (Is_left_lane_change == false && Is_right_lane_change == false) {
			if (current_lane == 1) {
				current_lane_x = lane1_x;
			}
			if (current_lane == 2) {
				current_lane_x = lane2_x;
			}
			if (current_lane == 3) {
				current_lane_x = lane3_x;
			}
			if (current_lane == 4) {
				current_lane_x = lane4_x;
			}
			if (current_lane == 5) {
				current_lane_x = lane5_x;
			}

			float lateral_diff = 0.5f * (m_Rigidbody.position.x - current_lane_x);
			Vector3 movement = transform.right * lateral_diff * Time.deltaTime;
			m_Rigidbody.MovePosition (m_Rigidbody.position - movement);
		}

		// If host vehicle is over z: 2200 then move the vehicle to the original position
		Vector3 original_pos = new Vector3 (-4, 0, -380);

		if (m_Rigidbody.position.z > 2200) {
			// Back to initial position
			transform.position = original_pos;
			//reward = 10;
			//terminal = 1;
			//SceneManager.LoadScene (0);
		}

		// Progress
		ProgressText.progress = (m_Rigidbody.position.z + 380) / (2200 + 380);

		Front_ADAS = 0;
		Left_ADAS = 0;
		Right_ADAS = 0;

		// Send warning signals to the CommandServer
		if (Forward_warning == true){
			Front_ADAS = 1;
		}

		if (Left_warning == true) {
			Left_ADAS = 1;
		}

		if (Right_warning == true) {
			Right_ADAS = 1;
		}

		// Return vehicle's x position
		Vehicle_x = m_Rigidbody.position.x;

		Right_Changing = 0;
		Left_Changing = 0;
		// Return if vehicle is lane changing or not
		if (Is_left_lane_change == true) {
			Left_Changing = 1;
		}

		if (Is_right_lane_change == true) {
			Right_Changing = 1;
		}

		// If game and algorithm disconnected, game is restarted
		if (m_Rigidbody.position.x == -4 && m_Speed == 20 && 
			Is_left_lane_change == false && Is_right_lane_change == false) {
			Disconnect_check += 1;
		} else {
			Disconnect_check = 0;
		}

		if (Disconnect_check > 60) {
			SceneManager.LoadScene (0);
			Disconnect_check = 0;
		}

		if (Input.GetKeyDown ("space")) {
			SceneManager.LoadScene (0);
			Disconnect_check = 0;
		}
	}
		
	private void VerticalMove()
	{
		Vector3 movement = transform.forward * m_Speed * Time.deltaTime;
		m_Rigidbody.MovePosition (m_Rigidbody.position + movement);
	}
		
	// Abs function to make input as absolute value
	private float Abs(float val)
	{
		if (val < 0)
		{
			return -val;
		}
		else 
		{
			return val;
		}
	}

	// Get Action from remote Controller
	public int Set_Mode(int mode)
	{
		return mode;
	}

	// If other vehicle had collision with host vehicle, restart the game
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
			//Vector3 original_pos = new Vector3 (-4, 0, -380);
			terminal = 1;
			reward = -10;
			//transform.position = original_pos;
			SceneManager.LoadScene (0);
		} 
	}
}

