using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BeamAdder : MonoBehaviour
{
	
}


[CustomEditor(typeof(BeamAdder))]
public class BeamAdderEditor : Editor
{
	public override void OnInspectorGUI()
	{
		var beamAdder = target as BeamAdder;
		if (GUILayout.Button("Add Beams and Colliders"))
		{
			//make immune list
			HashSet<GameObject> immune = new HashSet<GameObject>();
			foreach (var t in beamAdder.GetComponentsInChildren<Transform>())
			{
				immune.Add(t.gameObject);
			}

			var addedCol = new List<MeshCollider>();
			var addedBeam = new List<DissolveBeam>();
			int CollidersChanged = 0;

			var meshes = FindObjectsOfType<MeshRenderer>();
			foreach (var mesh in meshes)
			{
				if (immune.Contains(mesh.gameObject) == false)
				{
					if(mesh.GetComponent<Collider>() != false && mesh.GetComponent<MeshCollider>() == false)
					{
						DestroyImmediate(mesh.GetComponent<Collider>());
						CollidersChanged++;
					}

					if (mesh.GetComponent<Collider>() == false)
					{
						var newMesh = mesh.gameObject.AddComponent<MeshCollider>();
						addedCol.Add(newMesh);
					}

					if (mesh.GetComponent<DissolveBeam>() == false)
					{
						var newBeam = mesh.gameObject.AddComponent<DissolveBeam>();
						addedBeam.Add(newBeam);
					}
				}
			}

			Debug.Log($"Added {addedBeam.Count} beams and {addedCol.Count} colliders ({CollidersChanged} were other types).");
		}
	}
}