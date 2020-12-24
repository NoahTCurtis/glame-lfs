using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseWeapon : BaseUseItem
{
	protected int ammo = 0;
	protected int maxAmmo = int.MaxValue;
	protected int clip = 0;
	protected int maxClip = 0;
	protected bool showsAmmoUI = true;

	protected bool UsesClip { get => maxClip > 0; }
	protected bool HasAmmo { get => ammo > 0 || clip > 0; }
	protected bool HasAmmoLoaded { get {
			if (UsesClip) return clip > 0;
			else return ammo > 0;
	}}

	public override void AddFromPickup(BaseUseItemPickup pickup)
	{
		Debug.Assert(pickup is BaseWeaponPickup);

		if (pickup == null)
		{
			ammo++;
			return;
		}

		var bwp = pickup as BaseWeaponPickup;
		ammo += bwp.ammo;

		TryRefreshUI();
	}

	protected void SpendAmmo(int amount = 1)
	{
		if(UsesClip) {
			clip -= amount;
		} else {
			ammo -= amount;
		}

		TryRefreshUI();
	}

	private void TryRefreshUI()
	{
		if(IsHeldItem)
		{
			SetUI();
		}
	}

	public override void SetUI()
	{
		if(UIController.Instance != null)
		{
			UIController.Instance.ShowAmmoPanel();
			UIController.Instance.SetAmmoPanel(ItemName, ammo, clip);
		}
	}

	public override void ClearUI()
	{
		if (UIController.Instance != null)
		{
			UIController.Instance.ShowAmmoPanel(showPanel : false);
		}
	}
}
