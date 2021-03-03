using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breakable_Voxel : BreakableByGBE
{
	Voxel.Chunk _chunk;
	Voxel.ChunkRenderer _chunkRenderer;

	void Awake()
	{
		_chunk = GetComponent<Voxel.Chunk>();
		_chunkRenderer = GetComponent<Voxel.ChunkRenderer>();
	}

	public override void PrepareForInstantVisualChange(BeamData beam) { }

	public override void PerformInstantVisualChange(BeamData beam) { }

	public override void PrepareForBreak(BeamData beam) { }

	public override void CalculateBreak(BeamData beam)
	{
		_chunk.BeamCut(beam);
		_chunkRenderer.CreateMesh();
		_chunkRenderer.EnablePhysics();
	}

	public override void ActivateBreak() { }
}
