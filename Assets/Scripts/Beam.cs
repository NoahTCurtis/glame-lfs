using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamData
{
	public BeamData(Ray ray, float radius)
	{
		this.ray = ray;
		this.radius = radius;
	}

	public void AddTarget(BreakableByGBE target)
	{
		targets.Add(target);
	}

	public void AddNewDebrisPiece(GameObject piece)
	{
		newDebrisPieces.Add(piece);
	}

	public Ray ray;
	public float radius;
	public List<BreakableByGBE> targets = new List<BreakableByGBE>();
	public List<GameObject> newDebrisPieces = new List<GameObject>();
	public List<DissolveBeam> dissolveBeams = new List<DissolveBeam>();

	public int targetsBrokenSoFar = 0;
}
