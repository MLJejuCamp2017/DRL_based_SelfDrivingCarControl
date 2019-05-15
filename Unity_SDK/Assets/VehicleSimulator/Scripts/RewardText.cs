using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RewardText : MonoBehaviour {
	public static float reward;
	Text text;
	void Awake ()
	{
		text = GetComponent <Text> ();
		reward = 0;
	}
	void Update ()
	{
		text.text = "Reward : " + reward.ToString("F2");
	}
}
