using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class ProgressText : MonoBehaviour
{
	public static float progress;
	Text text;
	void Awake ()
	{
		text = GetComponent <Text> ();
		progress = 0;
	}
	void Update ()
	{
		text.text = "Distance: " + progress + "/2600";
	}
}
