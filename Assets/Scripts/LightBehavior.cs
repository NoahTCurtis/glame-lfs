using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class LightBehavior : MonoBehaviour
{
	protected static HashSet<LightBehavior> _lightBehaviors = null;

	private Light _light;
	private float _startIntensity;

	void OnEnable()
	{
		if(_lightBehaviors == null)
		{
			_lightBehaviors = new HashSet<LightBehavior>();
		}
		_lightBehaviors.Add(this);

		_light = GetComponent<Light>();
		_startIntensity = _light.intensity;
	}

	void OnDisable()
	{
		if(_lightBehaviors != null)
		{
			_lightBehaviors.Remove(this);
		}
	}

	protected void SetLocalIntensity01(float intensity01)
	{
		_light.intensity = Mathf.Lerp(0, _startIntensity, intensity01);
	}

	public static void SetGlobalIntensity01(float intensity01)
	{
		if (_lightBehaviors == null)
			return;

		foreach (var lb in _lightBehaviors)
			lb.SetLocalIntensity01(intensity01);
	}
}
