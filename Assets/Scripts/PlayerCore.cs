using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerCore : MonoBehaviour
{
	public Transform HeadRoot;

	private WeapInventory _weapInventory;

	private GameObject _selectedObject = null;

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
		UpdateSelectedObject();
		if(Input.GetKeyDown(KeyCode.E) && _selectedObject != null)
		{
			//do different things depending on what is selected
			var pickup = _selectedObject.GetComponent<BaseUseItemPickup>();
			if (pickup)
			{
				_weapInventory.AddItemFromPickup(pickup);
				Destroy(pickup.gameObject);
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

	void UpdateSelectedObject()
	{
		float radius = 0.2f;
		float maxDist = 2f;
		Ray fatRay = new Ray(HeadRoot.position, HeadRoot.forward);
		int layerMask = LayerMask.NameToLayer("Player");

		var fatCastResults = Physics.SphereCastAll(fatRay, radius, maxDist, layerMask);
		foreach (var fatHit in fatCastResults)
		{
			var pickup = fatHit.transform.GetComponent<BaseUseItemPickup>();
			if (pickup != null)
			{
				RaycastHit thinHitInfo;
				Ray thinRay = new Ray(HeadRoot.position, fatHit.transform.position - HeadRoot.position);
				var thinHit = Physics.Raycast(thinRay, out thinHitInfo, maxDist, layerMask, QueryTriggerInteraction.UseGlobal);
				if (thinHit && (fatHit.transform == thinHitInfo.transform))
				{
					_selectedObject = thinHitInfo.transform.gameObject;
					break;
				}
			}
		}
	}

	void Respawn()
	{
		int scene = SceneManager.GetActiveScene().buildIndex;
		SceneManager.LoadScene(scene, LoadSceneMode.Single);
	}
}
