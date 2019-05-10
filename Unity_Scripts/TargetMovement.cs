using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TargetMovement : MonoBehaviour {

	public float m_Speed = 0.5f;
	private float lateral_speed;

	public float wait_time_lon = 3f;
	public float wait_time_lat = 7f;

	private Rigidbody m_Rigidbody;

	// Longitudinal Variable
	private float max_speed = 0.8f;
	private float min_speed = 0.4f;

	// Lateral Variable
	private bool Is_right_lane_change = false;
	private bool Is_left_lane_change = false;
	private float start_y = 0;

	private float sum_time_lon = 0f;
	private float sum_time_lat = 0f;

	private float mode_lon = 0;
	private int mode_lat = 0;

	private float lane1_x = -18;
	private float lane2_x = -11;
	private float lane3_x = -4;
	private float lane4_x = 3;
	private float lane5_x = 10;

	private int current_lane = 3;

	// 0: Do nothing 
	// Mode_lon => 1: speed up, 2: speed down
	// Mode_lat => 1: right change, 2: left change

	// Use this for initialization
	private void Start() 
	{
		m_Rigidbody = GetComponent<Rigidbody> (); // At start, define rigidbody
		m_Speed = Random.Range(0.4f, 0.6f); // Initial speed
	}

	// Update is called once per frame
	private void FixedUpdate() 
	{
		lateral_speed = m_Speed * 0.25f;

        float random_slider_val = GameObject.FindGameObjectWithTag("RandomSlider").GetComponent<Slider>().value;
        wait_time_lon = 8f - random_slider_val;
        wait_time_lat = 8f - random_slider_val;

        sum_time_lon += Time.deltaTime;
		sum_time_lat += Time.deltaTime;

		if (sum_time_lon > wait_time_lon) 
		{
			mode_lon = Random.Range (0, 1f);
			sum_time_lon = 0;
		}

		if (sum_time_lat > wait_time_lat) {
			mode_lat = Random.Range (0, 3);
			sum_time_lat = 0;
		}

		bool Left_warning = false;
		bool Right_warning = false;
		float forward_dist = 0f;
		float forward_vel = 0f;
		bool Forward_warning = false;

		Left_warning = this.gameObject.GetComponent<TargetController> ().Left_warning;
		Right_warning= this.gameObject.GetComponent<TargetController> ().Right_warning;
		forward_dist = this.gameObject.GetComponent<TargetController> ().forward_distance;
		forward_vel = this.gameObject.GetComponent<TargetController> ().forward_velocity;

		// Forward distance controller 
		float forward_threshold = 5f + (m_Speed / 3f);
		float k_p = 0.001f;
		float k_i = 0.001f;

		// AEB like motion
		if (forward_dist < 6f) {
			k_p = 0.5f;
		}

		float accel_prob = 0.75f;

		if ((forward_dist < forward_threshold) && (forward_dist != 0)) {
			Forward_warning = true;
		}

		////////////////////// Longitudinal Move //////////////////////
		if (Forward_warning == false) {
			if (mode_lon < accel_prob && mode_lon > 0 && m_Speed < max_speed) {
				m_Speed += 0.05f;
				mode_lon = 0;
			}

			if (mode_lon > accel_prob && m_Speed > min_speed) {
				m_Speed -= 0.05f;
				mode_lon = 0;
			}
		} else {
			m_Speed -= (k_p * (m_Speed - forward_vel) + k_i * (m_Speed - forward_vel) / Time.deltaTime);
			mode_lon = 0;
		}

		VerticalMove();

		if (m_Speed > max_speed) {
			m_Speed = max_speed;
		} 

		if (m_Speed < min_speed) {
			m_Speed = min_speed;
		}

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

		// Right Change
		if (mode_lat == 1 && Is_right_lane_change == false && current_lane != 5 && Right_warning == false) 
		{
			Is_right_lane_change = true;
			start_y = m_Rigidbody.position.x;
		}

		if (Is_right_lane_change == true) 
		{
			Vector3 movement = transform.right * lateral_speed;
			m_Rigidbody.MovePosition (m_Rigidbody.position + movement);
			if (Abs(m_Rigidbody.position.x - start_y) >= 7) 
			{
				Is_right_lane_change = false;
				mode_lat = 0;
			}
		}

		// Left Change
		if (mode_lat == 2 && Is_left_lane_change == false && current_lane != 1 && Left_warning == false) 
		{
			Is_left_lane_change = true;
			start_y = m_Rigidbody.position.x;
		}

		if (Is_left_lane_change == true) 
		{
			Vector3 movement = transform.right * lateral_speed;
			m_Rigidbody.MovePosition (m_Rigidbody.position - movement);
			if (Abs(start_y - m_Rigidbody.position.x) >= 7) 
			{
				Is_left_lane_change = false;
				mode_lat = 0;
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

//			float lateral_diff = 0.1f * (m_Rigidbody.position.x - current_lane_x);
//			Vector3 movement = transform.right * lateral_diff;
//			m_Rigidbody.MovePosition (m_Rigidbody.position + movement);

			Vector3 pos_vehicle = new Vector3 (current_lane_x, m_Rigidbody.position.y, m_Rigidbody.position.z);

			this.transform.position = pos_vehicle;
		}

	}

	private void VerticalMove()
	{
		Vector3 movement = transform.forward * m_Speed;
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
}
