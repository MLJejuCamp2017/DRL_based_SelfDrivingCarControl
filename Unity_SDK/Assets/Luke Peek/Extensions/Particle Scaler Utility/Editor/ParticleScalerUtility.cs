using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

class ParticleScalerUtility : EditorWindow {


	bool validSelection = false;
	bool scalePosition = false;
	bool scaleChildren = true;
	bool restartOnScale = true;
	float scale = 1f;

	private List<GameObject> selectedObjects = new List<GameObject>();



	[MenuItem ("Window/Luke Peek/Particle Scaler Utility")]
	public static void  ShowWindow () {
		EditorWindow.GetWindow(typeof(ParticleScalerUtility), false, "Particle Scaler Utility");
	}

	
	void OnGUI () {
		
		EditorGUILayout.Separator();

		EditorGUILayout.LabelField("Particle Scaler Utility", EditorStyles.whiteLargeLabel );

		if ( selectedObjects.Count == 0 ) {
			EditorGUILayout.HelpBox("Please select at least one GameObject in the hierarchy containing a Particle System.", MessageType.Warning);
		}
		
		EditorGUILayout.Separator();

		scaleChildren = EditorGUILayout.Toggle ("Scale Children", scaleChildren);
		scalePosition = EditorGUILayout.Toggle ("Scale Position", scalePosition);
		restartOnScale = EditorGUILayout.Toggle ("Clear on Scale", restartOnScale);

		EditorGUI.BeginDisabledGroup( !validSelection );
		
		EditorGUILayout.Separator();
		
		EditorGUILayout.LabelField("Preset Scales", EditorStyles.boldLabel );
		EditorGUILayout.BeginHorizontal ();
		if (GUILayout.Button("25%", GUILayout.Height(30))) ApplyScale(0.25f);
		if (GUILayout.Button("50%", GUILayout.Height(30))) ApplyScale(0.5f);
		if (GUILayout.Button("75%", GUILayout.Height(30))) ApplyScale(0.75f);
		EditorGUILayout.EndHorizontal ();
		EditorGUILayout.BeginHorizontal ();
		if (GUILayout.Button("150%", GUILayout.Height(30))) ApplyScale(1.5f);
		if (GUILayout.Button("200%", GUILayout.Height(30))) ApplyScale(2f);
		if (GUILayout.Button("250%", GUILayout.Height(30))) ApplyScale(2.5f);
		EditorGUILayout.EndHorizontal ();

		EditorGUILayout.Separator();

		EditorGUILayout.LabelField("Custom Scale", EditorStyles.boldLabel );
		scale = EditorGUILayout.Slider ("Custom Scale", scale, 0.1f, 10f);
		if (GUILayout.Button("Apply Custom Scale!", GUILayout.Height(40)))
		{
			ApplyScale( scale );
		}

		EditorGUILayout.Separator();

		EditorGUILayout.LabelField("Selected Objects:", EditorStyles.boldLabel );
		EditorGUILayout.BeginVertical("Box");
		for ( int i=0; i < selectedObjects.Count; i++ )
		{
			GameObject obj = selectedObjects[i];
			EditorGUILayout.LabelField( (i+1) + ". " +  obj.name, EditorStyles.miniLabel );
		}
		EditorGUILayout.EndVertical();

		EditorGUI.EndDisabledGroup();

		EditorGUILayout.Separator();
		EditorGUILayout.LabelField("Particle Scaler Utility v1.0 - lukepeek.com", EditorStyles.miniLabel);


		this.Repaint();

	}


	void OnSelectionChange()
	{
		GameObject[] selection = Selection.gameObjects;
		selectedObjects = new List<GameObject>();
		
		for ( int i=0; i < selection.Length; i++ )
		{
			selectedObjects.Add( selection[i] );
		}
		
		validSelection = (selectedObjects.Count > 0) ? true : false;
	}
	


	private void ApplyScale( float scaleFactor )
	{
		List<GameObject> scalableObjects = new List<GameObject>();

		// Get objects
		for ( int i=0; i < selectedObjects.Count; i++ )
		{
			GameObject curObj = selectedObjects[i];
			ParticleSystem curObjPS = curObj.GetComponent<ParticleSystem>();
			if ( curObjPS != null )
			{
				if ( !scaleChildren )
				{
					scalableObjects.Add(curObj);
				} else {
					ParticleSystem[] childParticleSystems = curObj.GetComponentsInChildren<ParticleSystem>();

					foreach ( ParticleSystem childPS in childParticleSystems )
					{
						scalableObjects.Add(childPS.gameObject);
					}
					
				}
			}
		}

		if ( scalableObjects.Count > 0 )
		{
			for ( int i=0; i < scalableObjects.Count; i++ )
			{
				ParticleSystem soPs = scalableObjects[i].GetComponent<ParticleSystem>();
				ScaleParticleSystem( soPs, scaleFactor );
				if ( restartOnScale ) soPs.Clear(scaleChildren);
			}
		}

	}


	private void ScaleParticleSystem( ParticleSystem particleSystem, float scaleFactor )
	{

		ParticleSystem.MainModule 	main = particleSystem.main;
		ParticleSystem.ShapeModule 	shape = particleSystem.shape;
		ParticleSystem.MinMaxCurve 	minMaxCurve;

		// Record Undo
		Undo.RecordObject( particleSystem , "Scale Particles");

		// Transform
		if ( scalePosition ) {
			particleSystem.transform.localPosition *= scaleFactor;
		}

		// Shape
		
		shape.angle *= scaleFactor;
		shape.scale *= scaleFactor;
		shape.radius *= scaleFactor;
		shape.randomDirectionAmount *= scaleFactor;
		shape.sphericalDirectionAmount *= scaleFactor;
		shape.meshScale *= scaleFactor;
		shape.normalOffset *= scaleFactor;

		// Speed

		minMaxCurve = main.startSpeed;
		minMaxCurve.constant *= scaleFactor;
		minMaxCurve.constantMin *= scaleFactor;
		minMaxCurve.constantMax *= scaleFactor;
		ScaleCurve(minMaxCurve.curveMin, scaleFactor);
		ScaleCurve(minMaxCurve.curveMax, scaleFactor);
		main.startSpeed = minMaxCurve;
		
		minMaxCurve = main.startSize;
		minMaxCurve.constant *= scaleFactor;
		minMaxCurve.constantMin *= scaleFactor;
		minMaxCurve.constantMax *= scaleFactor;
		ScaleCurve(minMaxCurve.curveMin, scaleFactor);
		ScaleCurve(minMaxCurve.curveMax, scaleFactor);
		main.startSize = minMaxCurve;
		
		minMaxCurve = main.gravityModifier;
		minMaxCurve.constant *= scaleFactor;
		minMaxCurve.constantMin *= scaleFactor;
		minMaxCurve.constantMax *= scaleFactor;
		ScaleCurve(minMaxCurve.curveMin, scaleFactor);
		ScaleCurve(minMaxCurve.curveMax, scaleFactor);
		main.gravityModifier = minMaxCurve;
	



		SerializedObject particleSystemSerialized;

		particleSystemSerialized = new SerializedObject(particleSystem);
		
		particleSystemSerialized.FindProperty("VelocityModule.x.scalar").floatValue *= scaleFactor;
		particleSystemSerialized.FindProperty("VelocityModule.y.scalar").floatValue *= scaleFactor;
		particleSystemSerialized.FindProperty("VelocityModule.z.scalar").floatValue *= scaleFactor;

		particleSystemSerialized.FindProperty("ClampVelocityModule.magnitude.scalar").floatValue *= scaleFactor;
		particleSystemSerialized.FindProperty("ClampVelocityModule.dampen").floatValue *= scaleFactor;
		particleSystemSerialized.FindProperty("ClampVelocityModule.x.scalar").floatValue *= scaleFactor;
		particleSystemSerialized.FindProperty("ClampVelocityModule.y.scalar").floatValue *= scaleFactor;
		particleSystemSerialized.FindProperty("ClampVelocityModule.z.scalar").floatValue *= scaleFactor;
		
		particleSystemSerialized.FindProperty("ForceModule.x.scalar").floatValue *= scaleFactor;
		particleSystemSerialized.FindProperty("ForceModule.y.scalar").floatValue *= scaleFactor;
		particleSystemSerialized.FindProperty("ForceModule.z.scalar").floatValue *= scaleFactor;

		particleSystemSerialized.ApplyModifiedProperties();

	}



	private void ScaleCurve(AnimationCurve curve, float scaleFactor)
	{
		if (curve == null) { return; }
		for (int i=0; i<curve.keys.Length; i++) { 
			curve.keys[i].value *= scaleFactor;
		}
	}





}