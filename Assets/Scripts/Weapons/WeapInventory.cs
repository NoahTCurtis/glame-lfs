using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeapInventory : MonoBehaviour
{
	public Transform HandRoot;
	public List<BaseUseItemPickup> StartingItems;

	private List<BaseUseItem> _items;
	private int _heldIndex = -1;
	private int _prevHeldIndex = -1;

	void Start()
	{
		_items = new List<BaseUseItem>();
		foreach(var pickup in StartingItems)
		{
			AddItemFromPickup(pickup);
		}
	}

	public void AddItemFromPickup(BaseUseItemPickup pickup)
	{
		//don't add if we already have it
		var existingItem = GetItem(pickup.Prefab.ItemName);
		if(existingItem != null)
		{
			existingItem.AddFromPickup(pickup);
			return;
		}

		//spawn weap prefab
		var item = GameObject.Instantiate(pickup.Prefab, HandRoot);
		var obj = item.transform;
		obj.localPosition = Vector3.zero;
		obj.localRotation = Quaternion.identity;
		obj.localScale = Vector3.one;

		//register the weapon
		_items.Add(item);

		//add whatever was in the pickup
		item.AddFromPickup(pickup);

		//select the weapon because it's new
		SelectItem(_items.Count - 1);
	}

	private BaseUseItem GetItem(string name)
	{
		return _items.Find(i => i.ItemName == name);
	}

	private void SelectItem(int index)
	{
		index = Mathf.Clamp(index, -1, _items.Count - 1);
		if (index == _heldIndex)
			return;

		_prevHeldIndex = _heldIndex;
		_heldIndex = index;

		//disable previous weapon
		if(_prevHeldIndex != -1)
		{
			_items[_prevHeldIndex].gameObject.SetActive(false);
		}

		//enable new weapon
		if(_heldIndex != -1)
		{
			_items[_heldIndex].gameObject.SetActive(true);
		}

		Debug.Log($"Selected Item {_heldIndex}");
	}

	public void SelectNextItem()
	{
		int index = _heldIndex + 1;
		if (index > _items.Count - 1)
			index = 0;

		SelectItem(index);
	}

	public void SelectPrevItem()
	{
		int index = _heldIndex - 1;
		if (index < 0)
			index = _items.Count - 1;

		SelectItem(index);
	}

	public void HideItem()
	{
		SelectItem(-1);
	}
}
