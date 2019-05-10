using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class LaneText : MonoBehaviour
{
	public static float lane;
	Text text;
	void Awake ()
	{
		text = GetComponent <Text> ();
		lane = 0;
	}
	void Update ()
	{
		text.text = "Current Lane: Lane " + lane;
	}
}
