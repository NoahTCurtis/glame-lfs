using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedRagdoll : MonoBehaviour
{
	List<Collider> _colliders = new List<Collider>();
	List<Rigidbody> _rigidbodies = new List<Rigidbody>();
	List<CharacterJoint> _joints = new List<CharacterJoint>();

	void Awake()
	{
		foreach(var c in GetComponentsInChildren<Collider>())
			if(c.gameObject != gameObject)
				_colliders.Add(c);

		foreach (var rb in GetComponentsInChildren<Rigidbody>())
			if (rb.gameObject != gameObject)
				_rigidbodies.Add(rb);
	}

	void Start()
	{
		Animated();
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.B))
			Ragdoll();
	}

	void Animated()
	{
		foreach (var c in _colliders)
		{
			c.enabled = false;
		}

		foreach(var rb in _rigidbodies)
		{
			rb.isKinematic = true;
		}
	}

	void Ragdoll()
	{
		GetComponent<Animator>().enabled = false;

		foreach (var c in _colliders)
		{
			c.enabled = true;
		}

		foreach (var rb in _rigidbodies)
		{
			rb.isKinematic = false;
		}

		foreach(var j in _joints)
		{
			//j.enableProjection = true;
		}
	}
}
