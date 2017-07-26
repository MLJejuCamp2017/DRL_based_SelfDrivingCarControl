using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using SocketIO;
using UnityStandardAssets.Vehicles.Car;
using System;
using System.Security.AccessControl;

public class CommandServer : MonoBehaviour
{
	public HostMovement HostMovement;
	// public CarRemoteControl CarRemoteControl;
	public Camera FrontFacingCamera;
	public Camera RearFacingCamera;
	private SocketIOComponent _socket;
	private HostMovement _hostMovement;

	private String Front_camera_old;
	private String Rear_camera_old;

	// Use this for initialization
	void Start()
	{
		_socket = GameObject.Find("SocketIO").GetComponent<SocketIOComponent>();
		_socket.On("open", OnOpen);
		_socket.On("steer", OnSteer);
		_socket.On("manual", onManual);
		_hostMovement = HostMovement.GetComponent<HostMovement>();
	}

	// Update is called once per frame
	void Update()
	{
	}

	void OnOpen(SocketIOEvent obj)
	{
		Debug.Log("Connection Open");
		EmitTelemetry(obj);
	}

	// 
	void onManual(SocketIOEvent obj)
	{
		EmitTelemetry (obj);
	}
//
	void OnSteer(SocketIOEvent obj)
	{
		JSONObject jsonObject = obj.data;
		HostMovement.action = int.Parse (jsonObject.GetField ("action").str);

		EmitTelemetry(obj);
	}

	void EmitTelemetry(SocketIOEvent obj)
	{
		//UnityMainThreadDispatcher.Instance().Enqueue(() =>
		//{
			//print("Attempting to Send...");
			// send only if it's not being manually driven
			//if ((Input.GetKey(KeyCode.W)) || (Input.GetKey(KeyCode.S))) {
			//	_socket.Emit("telemetry", new JSONObject());
			//}
			//else {
					// Collect Data from the Car
				Dictionary<string, string> data = new Dictionary<string, string>();
				//List<float> data_Lidar = new List<float>();

				data ["Speed"] = _hostMovement.m_Speed.ToString ("N4");
				data["Action_vehicle"] = _hostMovement.action.ToString("N4");
				data["reward"] = _hostMovement.reward.ToString("N4");
				data["Front_ADAS"] = _hostMovement.Front_ADAS.ToString("N4");
				data["Left_ADAS"] = _hostMovement.Left_ADAS.ToString("N4");
				data["Right_ADAS"] = _hostMovement.Right_ADAS.ToString("N4");
				data["Left_Changing"] = _hostMovement.Left_Changing.ToString("N4");
				data["Right_Changing"] = _hostMovement.Right_Changing.ToString("N4");
				data["Vehicle_X"] = _hostMovement.Vehicle_x.ToString("N4");

				data["terminal"] = _hostMovement.terminal.ToString("N4");

				if (HostMovement.Range_list.Count < 360) {
					for (int i = 0; i < 360; i++){
						data[i.ToString("D")] = 0.ToString("N4");
					}
				} else {
					for (int i = 0; i < 360; i++){
						data[i.ToString("D")] = HostMovement.Range_list[i].ToString("N4");
					}
				}
					

				if (FrontFacingCamera != null && RearFacingCamera != null) {
					data["front_image"] = Convert.ToBase64String(CameraHelper.CaptureFrame(FrontFacingCamera));
					data["rear_image"] = Convert.ToBase64String(CameraHelper.CaptureFrame(RearFacingCamera));

					Front_camera_old = Convert.ToBase64String(CameraHelper.CaptureFrame(FrontFacingCamera));
					Rear_camera_old = Convert.ToBase64String(CameraHelper.CaptureFrame(RearFacingCamera));

				} else {
					data["front_image"] = Front_camera_old;
					data["rear_image"]  = Rear_camera_old;
				}

				//data_Lidar = HostMovement.Range_list.ToString("N4");
				_socket.Emit("telemetry", new JSONObject(data));
				//_socket.Emit("telemetry", new JSONObject.Type.));
			//}
		//});

	}
}