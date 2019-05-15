using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class LeftWarningText : MonoBehaviour {

	public static bool left_warning;
	Text text;
	void Awake ()
	{
		text = GetComponent <Text> ();
		left_warning = false;
	}
	void Update ()
	{
		text.text = "Left Warning: " + left_warning;
	}
}
