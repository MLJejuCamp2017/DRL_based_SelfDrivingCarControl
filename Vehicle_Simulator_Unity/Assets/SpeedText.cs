using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class SpeedText : MonoBehaviour
{
	public static float speed;
	Text text;
	void Awake ()
	{
		text = GetComponent <Text> ();
		speed = 0;
	}
	void Update ()
	{
		text.text = "Current Speed: " + speed;
	}
}

