using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerCore : MonoBehaviour
{
	public Transform HeadRoot;

	private WeapInventory _weapInventory;

	void Start()
	{
		_weapInventory = GetComponent<WeapInventory>();
	}

	void Update()
	{
		if (transform.position.y < -100)
			Respawn();

		//respawn if the player hits 'R'
		if (Input.GetKeyDown(KeyCode.R))
		{
			Respawn();
		}

		//Raycast to interact and pick up items
		if(Input.GetKeyDown(KeyCode.E))
		{
			Ray ray = new Ray(HeadRoot.position, HeadRoot.forward);
			RaycastHit info;
			float maxDist = 10f;
			int layerMask = LayerMask.NameToLayer("Player");
			if(Physics.Raycast(ray, out info, maxDist, layerMask))
			{
				//do different things depending on what we hit
				var pickup = info.transform.GetComponent<BaseUseItemPickup>();
				if (pickup)
				{
					_weapInventory.AddItemFromPickup(pickup);
					Destroy(pickup.gameObject);
				}
			}
		}

		if(Input.mouseScrollDelta.y < 0)
		{
			_weapInventory.SelectNextItem();
		}

		if (Input.mouseScrollDelta.y > 0)
		{
			_weapInventory.SelectPrevItem();
		}
	}

	void Respawn()
	{
		int scene = SceneManager.GetActiveScene().buildIndex;
		SceneManager.LoadScene(scene, LoadSceneMode.Single);
	}
}
