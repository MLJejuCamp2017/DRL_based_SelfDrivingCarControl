using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class RightWarningText : MonoBehaviour {
	public static bool right_warning;
	Text text;
	void Awake ()
	{
		text = GetComponent <Text> ();
		right_warning = false;
	}
	void Update ()
	{
		text.text = "Right Warning: " + right_warning;
	}
}
