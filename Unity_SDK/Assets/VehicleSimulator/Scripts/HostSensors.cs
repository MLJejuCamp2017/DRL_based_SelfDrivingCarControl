using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class HostSensors : MonoBehaviour {

	// This will be used in other scripts (TargetMovement to control the vehicle)
	public bool Left_warning = false;
	public bool Right_warning = false;
	public float forward_distance = 0f;
	public float forward_velocity = 0f;

    public Dropdown noise_dropdown;
    public GameObject NoiseSlider;
    public GameObject NoiseWeightText_obj;

	private LineRenderer laserLineRenderer;

	private float range_f = 40f; // How far front ray can go
	private float range_side = (7f + 3.5f);
	private float range_diag = (7f + 1f) * Mathf.Sqrt (2);
    //	private float range_diag2 = Mathf.Sqrt(Mathf.Pow((7f + (7f / 2)),2) + Mathf.Pow(2 * (7f + (7f / 2)), 2));

    Ray shootRay = new Ray(); // Shoot ray what we hit

	RaycastHit shootHit_forward; // return back to us whatever we hit (forward)
	RaycastHit shootHit_test;

	// Direction of ray
	private Vector3 forward_direction = new Vector3 (0, 0, 1); 
	private Vector3 left_direction    = new Vector3(-1, 0, 0);
	private Vector3 right_direction   = new Vector3 (1, 0, 0);
	private Vector3 LF_direction  = new Vector3 (-1, 0, 1); // Left Front 
	private Vector3 LR_direction  = new Vector3 (-1, 0, -1); // Left Rear 
	private Vector3 RF_direction  = new Vector3 (1, 0, 1); // Right Front
	private Vector3 RR_direction  = new Vector3 (1, 0, -1); // Right Rear

	// Ray for LIDAR 
	private Vector3 Lidar_direction;
	private float Lidar_range = 80f;
	public List<float> Range_list = new List<float>();
	private int Num_ray = 360; 
	RaycastHit shootHit_LIDAR; // return back to us whatever we hit (LIDAR)

	//	// Initialize dist variables
	private float forward_dist = 0f;

	void Start()
	{
		
	}

	void FixedUpdate()
	{
		Shoot ();
	}

	void Shoot ()
	{
		// 2 points of the line (First position (barrel of the gun), ?)
		shootRay.origin = this.transform.position; // Where the ray starts off
		Vector3 ShootOrigin = shootRay.origin;
		ShootOrigin.y = 1f;

		Vector3 Origin2 = shootRay.origin;
		Origin2.y = 0.5f;
		Origin2.z = Origin2.z + 3;
		Vector3 Origin3 = shootRay.origin;
		Origin3.y = 0.5f;
		Origin3.z = Origin3.z - 3;

		Vector3 FrontPoint = shootRay.origin;
		FrontPoint.z = FrontPoint.z + 4;
		FrontPoint.y = 0.5f;

		Vector3 Front_Left_Point = shootRay.origin;
		Front_Left_Point.z = Front_Left_Point.z + 4;
		Front_Left_Point.y = 0.5f;
		Front_Left_Point.x = Front_Left_Point.x - 1.5f;

		Vector3 Front_Right_Point = shootRay.origin;
		Front_Right_Point.z = Front_Right_Point.z + 4;
		Front_Right_Point.y = 0.5f;
		Front_Right_Point.x = Front_Right_Point.x + 1.5f;

        // Add Sensor Noise
        int NoiseVal = noise_dropdown.value;
       
        float noise_weight = 0f;

        if (NoiseVal == 0)
        {
            NoiseSlider.SetActive(false);
            NoiseWeightText_obj.SetActive(false);
        }
        else
        {
            NoiseSlider.SetActive(true);
            NoiseWeightText_obj.SetActive(true);
            float sliderval = GameObject.FindGameObjectWithTag("NoiseSlider").GetComponent<Slider>().value;
            noise_weight = sliderval;
            noise_weight = Mathf.Floor(100 * noise_weight);
            noise_weight = noise_weight / 100;

        }

        // Initialize dist variables as zero
        forward_dist = range_f;

		// shootRay: Ray
		// out shootHit: What we hit?
		// range: 100f (100 units)
		// shootableMask: we can hit only shootable thing

		// Initialize warning on right and left side of the vehicle
		Left_warning  = false;
		Right_warning = false;

		// LIDAR ray
		Range_list = new List<float>();

		for (int i = 0; i < Num_ray; i++) {
			float angle = i * (2 * Mathf.PI/Num_ray);

			float x_val = (Lidar_range * Mathf.Sin (angle)) / Lidar_range;
			float z_val = (Lidar_range * Mathf.Cos (angle)) / Lidar_range;

			Lidar_direction = new Vector3 (x_val, 0, z_val);

            //Debug.DrawRay (ShootOrigin, Lidar_range * Lidar_direction, Color.green);

            if (Physics.Raycast(ShootOrigin, Lidar_direction, out shootHit_LIDAR, Lidar_range)) {
                //Debug.DrawRay (ShootOrigin, shootHit_LIDAR.distance * Lidar_direction, Color.red);

                // Add Sensor Noise
                if (NoiseVal == 0)
                {
                    Range_list.Add(shootHit_LIDAR.distance);
                }
                else
                {
                    Range_list.Add(shootHit_LIDAR.distance + (noise_weight * Random.Range(-shootHit_LIDAR.distance, shootHit_LIDAR.distance)));
                }
			} else {
                // Add Sensor Noise
                if (NoiseVal == 0)
                {
                    Range_list.Add(Lidar_range);
                }
                else
                {
                    Range_list.Add(Lidar_range + (noise_weight * Random.Range(-Lidar_range, Lidar_range)));
                }
            }
		}

		forward_velocity = 0f;
		// Front ray left
		if (Physics.Raycast (Front_Left_Point, forward_direction, out shootHit_forward, range_f)) {

            forward_dist = shootHit_forward.distance;

            if (shootHit_forward.rigidbody != null) {
				if (shootHit_forward.rigidbody.GetComponent<TargetMovement> () != null) {
					forward_velocity = shootHit_forward.rigidbody.GetComponent<TargetMovement> ().m_Speed;
				}
			}
		}

		// Front ray right
		if (Physics.Raycast (Front_Right_Point, forward_direction, out shootHit_forward, range_f)) {

            forward_dist = shootHit_forward.distance;

            if (shootHit_forward.rigidbody != null) {
				if (shootHit_forward.rigidbody.GetComponent<TargetMovement> () != null) {
					forward_velocity = shootHit_forward.rigidbody.GetComponent<TargetMovement> ().m_Speed;
				}
			}
		}

		// Front ray center
		if (Physics.Raycast (FrontPoint, forward_direction, out shootHit_forward, range_f)) {

            forward_dist = shootHit_forward.distance;


            if (shootHit_forward.rigidbody != null) {
				if (shootHit_forward.rigidbody.GetComponent<TargetMovement> () != null) {
					forward_velocity = shootHit_forward.rigidbody.GetComponent<TargetMovement> ().m_Speed;
				}
			}
		}


		// Left Ray
		if (Physics.Raycast (ShootOrigin, left_direction, out shootHit_test, range_side)) {
			Left_warning = true;
		}

		// Right Ray
		if (Physics.Raycast (ShootOrigin, right_direction, out shootHit_test, range_side)) {
			Right_warning = true;
		}

		// Left Ray 2
		if (Physics.Raycast (Origin2, left_direction, out shootHit_test, range_side)) {
			Left_warning = true;
		}

		// Right Ray 2
		if (Physics.Raycast (Origin2, right_direction, out shootHit_test, range_side)) {
			Right_warning = true;
		}

		// Left Ray 3
		if (Physics.Raycast (Origin3, left_direction, out shootHit_test, range_side)) {
			Left_warning = true;
		}

		// Right Ray 3
		if (Physics.Raycast (Origin3, right_direction, out shootHit_test, range_side)) {
			Right_warning = true;
		}
			
		// Left Rear Ray
		if (Physics.Raycast (Origin3, LR_direction, out shootHit_test, range_diag)) {
			Left_warning = true;
		}
			

		// Right Rear Ray
		if (Physics.Raycast (Origin3, RR_direction, out shootHit_test, range_diag)) {
			Right_warning = true;
		}

		// Left Front Ray
		if (Physics.Raycast (Origin2, LF_direction, out shootHit_test, range_diag)) {
			Left_warning = true;
		}


		// Right Front Ray
		if (Physics.Raycast (Origin2, RF_direction, out shootHit_test, range_diag)) {
			Right_warning = true;
		}

		forward_distance = forward_dist;

	}
}
