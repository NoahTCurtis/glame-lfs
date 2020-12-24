using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class SunGrenade : MonoBehaviour
{
	//control numbers
	private float _flyHeight = 2.5f;
	private float _maxIntensity = 100f;
	private float _activationDelay = 5f;
	private float _fadeInDuration = 10f;
	private float _holdDuration = 2f;
	private float _fadeOutDuration = 2f;

	//times
	private float _startTime;
	private float _activationEndTime;
	private float _fadeInEndTime;
	private float _holdEndTime;
	private float _fadeOutEndTime;

	private Light _light;

	void Start()
	{
		_light = GetComponent<Light>();
		_startTime         = Time.time;
		_activationEndTime = Time.time + _activationDelay;
		_fadeInEndTime     = Time.time + _activationDelay + _fadeInDuration;
		_holdEndTime       = Time.time + _activationDelay + _fadeInDuration + _holdDuration;
		_fadeOutEndTime    = Time.time + _activationDelay + _fadeInDuration + _holdDuration + _fadeOutDuration;
}

	void Update()
	{
		if(Time.time < _activationEndTime)
		{
			//activation delay
			float t = Mathf.InverseLerp(_startTime, _activationEndTime, Time.time);
		}
		else if(Time.time < _fadeInEndTime)
		{
			//fade in
			float t = Mathf.InverseLerp(_activationEndTime, _fadeInEndTime, Time.time);
			float intensity = Mathf.Lerp(0, _maxIntensity, t);
			_light.intensity = intensity;
			LightBehavior.SetGlobalIntensity01(1.0f - intensity);
		}
		else if (Time.time < _holdEndTime)
		{
			//hold
			float t = Mathf.InverseLerp(_fadeInEndTime, _holdEndTime, Time.time);
		}
		else if (Time.time < _fadeOutEndTime)
		{
			//fade out
			float t = Mathf.InverseLerp(_holdEndTime, _fadeOutEndTime, Time.time);
			float intensity = Mathf.Lerp(_maxIntensity, 0, t);
			_light.intensity = intensity;
			LightBehavior.SetGlobalIntensity01(1.0f - intensity);
		}
		else
		{
			//end
			LightBehavior.SetGlobalIntensity01(1.0f);
			Destroy(gameObject);
		}
	}
}
