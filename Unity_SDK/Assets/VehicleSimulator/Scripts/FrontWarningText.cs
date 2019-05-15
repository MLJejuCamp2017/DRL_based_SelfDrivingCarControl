using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class FrontWarningText : MonoBehaviour {

	public static bool front_warning;
	Text text;
	void Awake ()
	{
		text = GetComponent <Text> ();
		front_warning = false;
	}
	void Update ()
	{
		text.text = "Front Warning: " + front_warning;
	}
}
