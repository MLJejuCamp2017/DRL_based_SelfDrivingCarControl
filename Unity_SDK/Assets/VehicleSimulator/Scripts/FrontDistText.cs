using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FrontDistText : MonoBehaviour {

	public static float front_dist;
	Text text;
	void Awake ()
	{
		text = GetComponent <Text> ();
		front_dist = 0;
	}
	void Update ()
	{
		if (front_dist < 40) {
			text.text = "Front Distance: " + (front_dist / 2f).ToString ("F2") + "m";
		} else {
			text.text = "Front Distance: None";
		}
	}
}
