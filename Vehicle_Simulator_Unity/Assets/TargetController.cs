using UnityEngine;
using System.Collections;

public class TargetController : MonoBehaviour {

	// This will be used in other scripts (TargetMovement to control the vehicle)
	public bool Left_warning = false;
	public bool Right_warning = false;
	public float forward_distance = 0f;

	private float range_f = 50f; // How far ray can go
	private float range_side = (7f + (7f / 2));
	private float range_diag = (7f + (7f / 2)) * Mathf.Sqrt (2);
	private float range_diag2 = Mathf.Sqrt(Mathf.Pow((7f + (7f / 2)),2) + Mathf.Pow(2 * (7f + (7f / 2)), 2));

	Ray shootRay = new Ray(); // Shoot ray what we hit

	RaycastHit shootHit_forward; // return back to us whatever we hit (forward)

	// Direction of ray
	private Vector3 forward_direction = new Vector3 (0, 0, 1); 
	private Vector3 left_direction    = new Vector3(-1, 0, 0);
	private Vector3 right_direction   = new Vector3 (1, 0, 0);
	private Vector3 LF_direction  = new Vector3 (-1, 0, 1); // Left Front 
	private Vector3 LF_direction2 = new Vector3 (-1, 0, 2); // Left Front2
	private Vector3 LR_direction  = new Vector3 (-1, 0, -1); // Left Rear 
	private Vector3 LR_direction2 = new Vector3 (-1, 0, -2); // Left Rear2
	private Vector3 RF_direction  = new Vector3 (1, 0, 1); // Right Front
	private Vector3 RF_direction2 = new Vector3 (1, 0, 2); // Right Front2
	private Vector3 RR_direction  = new Vector3 (1, 0, -1); // Right Rear
	private Vector3 RR_direction2 = new Vector3 (1, 0, -2); // Right Rear2

//	// Initialize dist variables
	private float forward_dist = 0f;

	void Update()
	{
		Shoot ();
	}

	void Shoot ()
	{
		// 2 points of the line (First position (barrel of the gun), ?)
		shootRay.origin = this.transform.position; // Where the ray starts off

		Vector3 Origin2 = shootRay.origin;
		Origin2.z = Origin2.z + 2;
		Vector3 Origin3 = shootRay.origin;
		Origin3.z = Origin3.z - 2;

		Vector3 StartPoint = shootRay.origin;
		StartPoint.z = StartPoint.z + 4;

//		// Initialize dist variables as zero
		forward_dist = 0f;

		// shootRay: Ray
		// out shootHit: What we hit?
		// range: 100f (100 units)
		// shootableMask: we can hit only shootable thing

		// Initialize warning on right and left side of the vehicle
		Left_warning  = false;
		Right_warning = false;

		// Front ray
		if (Physics.Raycast (StartPoint, forward_direction, out shootHit_forward, range_f)) {
			forward_dist = shootHit_forward.distance;
		}

		// Left Ray
		if (Physics.Raycast (shootRay.origin, left_direction, range_side)) {
			Left_warning = true;
		}

		// Right Ray
		if (Physics.Raycast (Origin2, right_direction, range_side)) {
			Right_warning = true;
		}

		// Left Ray 2
		if (Physics.Raycast (Origin2, left_direction, range_side)) {
			Left_warning = true;
		}

		// Right Ray 2
		if (Physics.Raycast (Origin3, right_direction, range_side)) {
			Right_warning = true;

		}

		// Left Ray 3
		if (Physics.Raycast (Origin3, left_direction, range_side)) {
			Left_warning = true;
		}

		// Right Ray 3
		if (Physics.Raycast (shootRay.origin, right_direction, range_side)) {
			Right_warning = true;
		}

		// Left Front Ray
		if (Physics.Raycast (shootRay.origin, LF_direction, range_diag)) {
			Left_warning = true;
		}

		// Left Front Ray 2
		if (Physics.Raycast (shootRay.origin, LF_direction2, range_diag2)) {
			Left_warning = true;
		}

		// Left Rear Ray
		if (Physics.Raycast (shootRay.origin, LR_direction, range_diag)) {
			Left_warning = true;
		}

		// Left Rear Ray2
		if (Physics.Raycast (shootRay.origin, LR_direction2, range_diag2)) {
			Left_warning = true;
		}

		// Right Front Ray
		if (Physics.Raycast (shootRay.origin, RF_direction, range_diag)) {
			Right_warning = true;
		}

		// Right Front Ray 2
		if (Physics.Raycast (shootRay.origin, RF_direction2, range_diag2)) {
			Right_warning = true;
		}

		// Right Rear Ray
		if (Physics.Raycast (shootRay.origin, RR_direction, range_diag)) {
			Right_warning = true;
		}

		// Right Rear Ray2
		if (Physics.Raycast (shootRay.origin, RR_direction2, range_diag2)) {
			Right_warning = true;
		}

		forward_distance = forward_dist;
	}
}
