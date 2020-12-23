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
	}
}
