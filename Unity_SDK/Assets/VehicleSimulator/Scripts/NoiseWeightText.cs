using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NoiseWeightText : MonoBehaviour {
    public Dropdown noise_dropdown;
    private float noise_weight;
    Text text;

    void Awake()
    {
        text = GetComponent<Text>();
        noise_weight = 0.3f;
    }
    void Update()
    {

        noise_weight = GameObject.FindGameObjectWithTag("NoiseSlider").GetComponent<Slider>().value;
        noise_weight = Mathf.Floor(100 * noise_weight);
        noise_weight = noise_weight / 100;

        text.text = "Noise Weight:  " + noise_weight;
    }
}
