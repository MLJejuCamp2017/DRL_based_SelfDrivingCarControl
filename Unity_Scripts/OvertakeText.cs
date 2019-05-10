using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class OvertakeText : MonoBehaviour
{
	public static int count_overtake;
	Text text;
	void Awake ()
	{
		text = GetComponent <Text> ();
		count_overtake = 0;
	}
	void Update ()
	{
		text.text = "Num overtake: " + count_overtake;
	}
}
