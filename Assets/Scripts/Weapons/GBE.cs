using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class GBE : BaseWeapon
{
	[Header("Shot Pipeline")]
	public bool UseAudio = true;
	public bool UseBeamAnimation = true;
	public bool UseChargeRequirement = true; //cheatcode

	#region GameObjectReferences
	[Header("Rail Pieces")]
	public Transform Rail1;
	public Transform Rail2;
	public Transform Rail3;
	public Transform Rail4;

	[Header("Barrel Pieces")]
	public Transform b1;
	public Transform b2;
	public Transform b3;
	public Transform b4;
	public Transform b5;
	public Transform b6;
	public Transform b7;
	public Transform b8;

	[Header("Backplate UI")]
	public BackPlateBar barN;
	public BackPlateBar barE;
	public BackPlateBar barS;
	public BackPlateBar barW;
	public BackPlateBar barNE;
	public BackPlateBar barSE;
	public BackPlateBar barSW;
	public BackPlateBar barNW;
	public BackPlateBar centerCircle;

	[Header("Beam")]
	public Transform BeamOrigin;
	public Transform Beam;

	[Header("Other Pieces")]
	public Transform Handle;
	public Transform Trigger;

	[Header("VFX")]
	public List<ParticleSystem> Particles;
	#endregion

	//Singleton pattern
	public static GBE instance = null;

	//component references
	private AudioSource _audioSource;

	//GBE animation data
	public float charge = 0.0f;

	void Awake()
	{
		_audioSource = GetComponent<AudioSource>();
	}

	// Start is called before the first frame update
	void Start()
	{
		instance = this;
		Beam.transform.localScale = Vector3.zero;
		Beam.transform.localPosition = Vector3.zero;
	}

	// Update is called once per frame
	void Update()
	{
		//find charge level
		float secondsItTakesToCharge = 1.75f;
		float secondsItTakesToUncharge = 0.75f;
		if (Input.GetMouseButton(1))
		{
			charge += Time.deltaTime / secondsItTakesToCharge;
		}
		else
		{
			charge -= Time.deltaTime / secondsItTakesToUncharge;
		}
		charge = Mathf.Clamp(charge, 0, 6);

		//animate the gun
		float openness01 = Mathf.Clamp01(charge - 4);
		OpenRail01(openness01);
		FillUIBars();
		GunShake01(openness01);

		//shoot, if possible
		bool shotInProgress = Game.Manager<BeamManager>().ShotInProgress;
		if(Input.GetMouseButtonDown(0) && !shotInProgress && (charge > 1 || !UseChargeRequirement))
		{
			Shoot();
		}
	}

	void OpenRail01(float value)
	{
		float t = value * 0.5f;
		Rail1.localPosition = Vector3.Lerp(new Vector3(0, -0.23f, -0.23f), new Vector3(0, +1.0f, +1.0f), t);
		Rail2.localPosition = Vector3.Lerp(new Vector3(0, +0.23f, -0.23f), new Vector3(0, -1.0f, +1.0f), t);
		Rail3.localPosition = Vector3.Lerp(new Vector3(0, +0.23f, +0.23f), new Vector3(0, -1.0f, -1.0f), t);
		Rail4.localPosition = Vector3.Lerp(new Vector3(0, -0.23f, +0.23f), new Vector3(0, +1.0f, -1.0f), t);
	}

	void FillUIBars()
	{
		barN.SetFill01(charge > 5 ? 0 : Mathf.Clamp(charge, 0, 1) - 0.0f);
		barE.SetFill01(charge > 5 ? 0 : Mathf.Clamp(charge, 1, 2) - 1.0f);
		barS.SetFill01(charge > 5 ? 0 : Mathf.Clamp(charge, 2, 3) - 2.0f);
		barW.SetFill01(charge > 5 ? 0 : Mathf.Clamp(charge, 3, 4) - 3.0f);

		centerCircle.SetFill01( Mathf.Clamp(charge, 4, 5) - 4.0f);

		barNE.SetFill01(charge > 5 ? 1 : 0);
		barSE.SetFill01(charge > 5 ? 1 : 0);
		barSW.SetFill01(charge > 5 ? 1 : 0);
		barNW.SetFill01(charge > 5 ? 1 : 0);
	}

	void GunShake01(float value)
	{
		float shakeLateral = 0.0015f;
		float y = Random.Range(-shakeLateral, shakeLateral);
		float z = Random.Range(-shakeLateral, shakeLateral);
		transform.localPosition = new Vector3(transform.localPosition.x, y, z) * value;

		foreach(var ps in Particles)
		{
			var emission = ps.emission;
			emission.rateOverTime = (int)(1000.0f * value * value * value);
		}
	}

	void Shoot() //All shooting stuff happens in the lifetime of this function
	{
		//find the shot radius at this charge level
		float beamRadius = 0;
		switch ((int)Mathf.Floor(charge))
		{
			case 0: beamRadius = 0.5f; break; //no bars
			case 1: beamRadius = 1.0f; break; //1 bar
			case 2: beamRadius = 2.0f; break; //2 bars
			case 3: beamRadius = 4.0f; break; //3 bars
			case 4: beamRadius = 8.0f; break; //4 bars
			case 5: beamRadius = 16.0f; break;//X shot
			case 6: beamRadius = 16.0f; break;//X shot again
		}

		//create beam
		Transform cam = Camera.main.transform;
		Ray beamRay = new Ray(cam.position, cam.forward);

		Game.Manager<BeamManager>().CreateBeam(cam.position, cam.forward, beamRadius);

		//animation
		if (UseBeamAnimation)
			StartCoroutine(AnimateBeam(beamRadius));

		charge = 0;

		//sound
		if(UseAudio)
			_audioSource.Play();
	}
	
	private IEnumerator AnimateBeam(float beamRadius, IEnumerator next = null)
	{
		float startTime = Time.time;
		float endTime = Time.time + 0.25f;
		float startRadius = beamRadius;

		float length = 10.0f;
		float t = 0;
		while (t < 1)
		{
			float timeT = Mathf.InverseLerp(startTime, endTime, Time.time);
			//float breakT = 1;
			//if(beam != null)
			//	breakT = (float)beam.targetsBrokenSoFar / (float)beam.targets.Count;
			//t = Mathf.Min(timeT, breakT);
			t = timeT;

			float diameter = startRadius * 2.0f * (1.0f - t);
			Beam.localPosition = new Vector3(length, 0, 0);
			Beam.localScale = new Vector3(diameter, length, diameter);
			
			yield return null;
		}

		//reset for next time
		Beam.transform.localScale = Vector3.zero;
		Beam.transform.localPosition = Vector3.zero;

		if (next != null) StartCoroutine(next);
	}
}
