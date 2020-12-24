using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseUseItem : MonoBehaviour
{
	[Header("Base Use Item")]
	public string ItemName = "baseItem";

	private WeapInventory _inventory = null;
	public void SetInventory(WeapInventory inventory)
	{
		_inventory = inventory;
	}

	protected bool IsHeldItem { get {
		return _inventory.IsHoldingItem && (_inventory.HeldItem == this);
	}}

	public virtual void AddFromPickup(BaseUseItemPickup pickup) { }
	public virtual void SetUI() { }
	public virtual void ClearUI() { }
}
