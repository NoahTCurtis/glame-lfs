using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamManager : Manager
{
	[Header("Shot Options")]
	public bool UseShaderHoles = true;
	public bool UseBreakables = true;
	public bool UseRubblePhysics = true;

	//current shot data
	private BeamData _beam;
	public bool ShotInProgress { get => _beam != null; }

	protected override void Awake()
	{
		base.Awake();
	}

	void Start()
	{
		
	}

	public void CreateBeam(Vector3 beamStart, Vector3 beamDirection, float beamRadius) //All shooting stuff happens in the lifetime of this function
	{
		//create beam data
		Ray beamRay = new Ray(beamStart, beamDirection);
		_beam = new BeamData(beamRay, beamRadius);

		//shot pipeline
		CollectBeamTargets(_beam);

		IEnumerator enablePhysicsOnDebris = EnablePhysicsOnDebris(_beam, EndShot());
		IEnumerator breakBeamTargets = BreakBeamTargets(_beam, enablePhysicsOnDebris);

		StartCoroutine(breakBeamTargets);
	}

	void CollectBeamTargets(BeamData beam)
	{
		Transform cam = Camera.main.transform;
		var hits = Physics.SphereCastAll(cam.position, beam.radius, cam.forward);

		System.Array.Sort(hits, (x, y) => x.distance.CompareTo(y.distance));

		foreach (var hit in hits)
		{
			//remember it so they can all be broken later
			BreakableByGBE breaker = hit.collider.GetComponent<BreakableByGBE>();
			if (breaker != null)
			{
				beam.targets.Add(breaker);
			}

			//make the piece appear broken via shaders
			var dissolveBeam = hit.collider.GetComponent<DissolveBeam>();
			if (dissolveBeam != null && UseShaderHoles)
			{
				dissolveBeam.AddBeam(beam);
				beam.dissolveBeams.Add(dissolveBeam);
			}
		}
	}

	private IEnumerator EndShot()
	{
		_beam = null;
		yield return null;
	}

	IEnumerator BreakBeamTargets(BeamData beam, IEnumerator next = null)
	{
		int maxBreaksPerFrame = 5;
		int brokenOnThisFrame = 0;
		foreach (var breaker in beam.targets)
		{
			if (UseBreakables)
				breaker.CalculateBreak(beam); //TODO! Use all funcs and move this into beam mgr

			brokenOnThisFrame += 1;
			beam.targetsBrokenSoFar += 1;

			if (brokenOnThisFrame >= maxBreaksPerFrame)
			{
				yield return null;
				brokenOnThisFrame = 0;
			}
		}

		if (next != null) StartCoroutine(next);
	}

	IEnumerator EnablePhysicsOnDebris(BeamData beam, IEnumerator next = null)
	{
		foreach (var debris in beam.newDebrisPieces)
		{
			Rigidbody rb = debris.GetComponent<Rigidbody>();
			if (rb != null && UseRubblePhysics)
				rb.isKinematic = false;
		}

		yield return null;

		if (next != null) StartCoroutine(next);
	}
}
