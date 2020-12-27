using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breakable_Destroy : BreakableByGBE
{
	public override void CalculateBreak(BeamData beam)
	{
		Destroy(gameObject);
	}
}
