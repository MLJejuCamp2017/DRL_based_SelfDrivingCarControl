using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RandomText : MonoBehaviour
{
    private float RandomLevel;
    Text text;
    void Awake()
    {
        text = GetComponent<Text>();
        RandomLevel = 3;
    }
    void Update()
    {
        RandomLevel = GameObject.FindGameObjectWithTag("RandomSlider").GetComponent<Slider>().value;
        text.text = "Random Action Level: " + RandomLevel;
    }
}

