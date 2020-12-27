using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DissolveBeamParent : DissolveBeam
{
	List<MeshRenderer> _renderers = new List<MeshRenderer>();
	List<DissolveBeam> _dissolveBeams = new List<DissolveBeam>();

	void Awake()
	{
		foreach(var mr in GetComponentsInChildren<MeshRenderer>())
		{
			_renderers.Add(mr);

			var beam = mr.GetComponent<DissolveBeam>();

			if(beam == null)
				beam = mr.gameObject.AddComponent<DissolveBeam>();

			_dissolveBeams.Add(beam);
		}

		Debug.Assert(_renderers.Count == _dissolveBeams.Count);
	}

	public override void AddBeam(BeamData beamData)
	{
		if (enabled == false) return;

		foreach(var beam in _dissolveBeams)
		{
			beam.AddBeam(beamData);
		}
	}
}
