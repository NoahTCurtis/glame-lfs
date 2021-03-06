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
			MultiChunk mc = target as MultiChunk;

			if(GUILayout.Button("Generate"))
			{
				mc.Generate(true);
			}

			if(GUILayout.Button("Clear"))
			{
				mc.HardReset();
			}

			DrawDefaultInspector();
		}
	}
}