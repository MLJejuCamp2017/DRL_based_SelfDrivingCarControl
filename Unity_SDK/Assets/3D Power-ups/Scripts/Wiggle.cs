using UnityEngine;
using System.Collections;
 
public class Wiggle : MonoBehaviour {
	public enum Axis {
		Y,
		X,
		Z
	}
	public Axis axis;
	public bool sway;
	public float swayAngle = 15.0f;
	public float speed = 1.0f;
	
	void Update () {
		if (!sway) {
			float spin = Time.time * 360.0f * speed;

			switch (axis) {
			case Axis.X:
				transform.localEulerAngles = new Vector3 (spin, 0f, 0f);
				break;
				
			case Axis.Y:
				transform.localEulerAngles = new Vector3 (0f, spin, 0f);
				break;
				
			case Axis.Z:
				transform.localEulerAngles = new Vector3 (0f, 0f, spin);
				break;
			}
		}
		if (sway) {
			float spin = swayAngle * Mathf.Cos(speed * Mathf.PI * Time.time);

			switch (axis) {
			case Axis.X:
				transform.localEulerAngles = new Vector3 (spin, 0f, 0f);
				break;
				
			case Axis.Y:
				transform.localEulerAngles = new Vector3 (0f, spin, 0f);
				break;
				
			case Axis.Z:
				transform.localEulerAngles = new Vector3 (0f, 0f, spin);
				break;
			}
		}
	}
}
