using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeapInventory : MonoBehaviour
{
	public Transform HandRoot;
	public List<BaseUseItemPickup> StartingItems;

	public bool IsHoldingItem { get => _heldIndex != -1; }
	public bool WasHoldingItem { get => _prevHeldIndex != -1; }
	public BaseUseItem HeldItem { get => IsHoldingItem ? _items[_heldIndex] : null; }
	public BaseUseItem PrevHeldItem { get => WasHoldingItem ? _items[_prevHeldIndex] : null; }

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
		var existingItem = GetItemByName(pickup.Prefab.ItemName);
		if(existingItem != null)
		{
			existingItem.AddFromPickup(pickup);
			if (existingItem == HeldItem)
				HeldItem.SetUI();
			return;
		}

		//spawn the item prefab (probably weap prefab)
		var item = GameObject.Instantiate(pickup.Prefab, HandRoot);
		var obj = item.transform;
		obj.localPosition = Vector3.zero;
		obj.localRotation = Quaternion.identity;
		obj.localScale = Vector3.one;

		//register the weapon
		_items.Add(item);
		item.SetInventory(this);

		//add whatever was in the pickup
		item.AddFromPickup(pickup);

		//select the new item because it's new
		SelectItem(_items.Count - 1);
	}

	private BaseUseItem GetItemByName(string name)
	{
		return _items.Find(i => i.ItemName == name);
	}

	private void SelectItem(int index)
	{
		//decide which item to hold
		index = Mathf.Clamp(index, -1, _items.Count - 1);
		if (index == _heldIndex)
			return;

		//swap to the new item
		_prevHeldIndex = _heldIndex;
		_heldIndex = index;

		//disable previous item
		if(_prevHeldIndex != -1)
		{
			PrevHeldItem.ClearUI();
			PrevHeldItem.gameObject.SetActive(false);
		}

		//enable new item
		if(_heldIndex != -1)
		{
			HeldItem.SetUI();
			HeldItem.gameObject.SetActive(true);
		}
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
