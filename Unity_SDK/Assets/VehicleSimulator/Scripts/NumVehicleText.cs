using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NumVehicleText : MonoBehaviour
{
    private float numVehicle;
    Text text;
    void Awake()
    {
        text = GetComponent<Text>();
        numVehicle = 0;
    }
    void Update()
    {
        numVehicle = GameObject.FindGameObjectWithTag("VehicleSlider").GetComponent<Slider>().value;
        text.text = "Number of Vehicles: " + numVehicle;
    }
}

