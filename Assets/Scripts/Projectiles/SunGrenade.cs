using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class SunGrenade : MonoBehaviour
{
	private Light _light;

	void Start()
	{
		_light = GetComponent<Light>();
	}
}
