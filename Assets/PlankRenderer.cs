using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DefaultNamespace;
using UnityEngine.Serialization;

[RequireComponent(typeof (MeshFilter))]
[RequireComponent(typeof (MeshRenderer))]
public class PlankRenderer : MonoBehaviour
{
	public float InnerDiameter = 340f;
	public float OuterDiameter = 380f;
	public float PlankWidth = 14.5f;
	public float Max = 120f;
	public float Min = 50f;
	public float Thickness = 1.8f;
	public float MiddleX = 40f;
	public float MiddleZ = 29.5f;
	public float Rotation = -90f;
	
	private float _scale = 0.1f;
	public Material PlankMaterial;
	public Material TerrasMaterial;
	private List<Bounds> _barriers;
	void Start ()
	{
		Physics.autoSyncTransforms = true;
		
		WatchInput();
		objects = new List<GameObject>();
		_barriers = GameObject.FindObjectsOfType<GameObject>()
			.Where(x => x.name == "Barrier")
			.Select(x => x.GetComponent<Collider>().bounds)
			.ToList();
		
		PlankLengthCalculator calculator = new PlankLengthCalculator(PlankWidth, Min, Max);
		var innerPlankWidth = (float)PlankWidth / ((float)(Math.PI * OuterDiameter) / (float)(Math.PI * InnerDiameter));
		var plankLengths = calculator.CalculateLengths(OuterDiameter);
		
		RenderPlanks(InnerDiameter, innerPlankWidth, plankLengths);
		RenderPlanks(OuterDiameter, PlankWidth, plankLengths);
		
		CenterTable();
	}

	public List<GameObject> objects = new List<GameObject>();
	private void RenderPlanks(float diameter, float plankWidth, List<float> planks)
	{
		var scaledWidth = plankWidth * _scale;
		var scaledThickness = Thickness * _scale;
		for(int i = 0; i < planks.Count; i++)
		{
			var degree = (float)i * (360f / (float)planks.Count) + Rotation;
			var rad = Math.PI / 180 * degree;
			var x = (float) Math.Sin(rad) * (diameter / 2) * _scale + MiddleX;
			var z = (float) Math.Cos(rad) * (diameter / 2) * _scale + MiddleZ;
			var yRotation = Rotation + 90 + 360f / planks.Count * i;
			var height = planks[i] * _scale;
			
			var obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
			obj.transform.localScale = new Vector3(scaledThickness, planks[i] * _scale, scaledWidth);
			obj.transform.position = new Vector3(x, height/2, z);
			obj.transform.Rotate(0, yRotation, 0);

			obj.GetComponent<Renderer>().material = PlankMaterial;
			
			objects.Add(obj);
			
			var objBounds = obj.GetComponent<Collider>().bounds;
			if (_barriers.Any(x => x.Intersects(objBounds)))
			{
				GameObject.Destroy(obj);
				objects.Remove(obj);
			}
		}
	}

	private void CenterTable()
	{
		var table = GameObject.Find("Tafel");
		table.transform.position = new Vector3(MiddleX, 3.5f, MiddleZ);

		var rondTerras = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
		rondTerras.transform.position = new Vector3(MiddleX, 0, MiddleZ);
		rondTerras.transform.localScale = new Vector3(InnerDiameter * _scale, 0.5f, InnerDiameter * _scale);
		rondTerras.GetComponent<Renderer>().material = TerrasMaterial;
		objects.Add(rondTerras);
	}

	private bool WatchStarted = false;

	private async Task WatchInput()
	{
		WatchStarted = true;

		var lastParams = (InnerDiameter, OuterDiameter, PlankWidth, Min, Max, Thickness, MiddleX, MiddleZ, Rotation, PlankMaterial, TerrasMaterial);
		while (true)
		{
			await Task.Delay(1000);
			var newParams = (InnerDiameter, OuterDiameter, PlankWidth, Min, Max, Thickness, MiddleX, MiddleZ, Rotation, PlankMaterial, TerrasMaterial);
			if (lastParams != newParams)
			{
				lastParams = newParams;
				foreach (var o in objects)
					Destroy(o);

				Start();
			}
		}
	}
}