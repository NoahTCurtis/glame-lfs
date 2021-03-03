using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Voxel
{
	[CustomEditor(typeof(MultiChunk))]
	[CanEditMultipleObjects]
	public class MultiChunkEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			if(GUILayout.Button("Generate"))
			{
				
			}

			if(GUILayout.Button("Clear"))
			{
				
			}

			DrawDefaultInspector();
		}
	}
}