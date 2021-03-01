using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voxel
{
	public enum Material
	{
		Air = 0,
		Concrete = 1,
	}

	public class MatUtil
	{
		public static bool IsSolid(Material m)
		{
			return m == Material.Concrete;
		}
	}
}
