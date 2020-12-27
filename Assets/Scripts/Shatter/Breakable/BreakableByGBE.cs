using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableByGBE : MonoBehaviour
{
	//These functions are called in order
	public virtual void PrepareForInstantVisualChange(BeamData beam) { }
	public virtual void PerformInstantVisualChange(BeamData beam) { }
	public virtual void PrepareForBreak(BeamData beam) { }
	public virtual void CalculateBreak(BeamData beam) { }
	public virtual void ActivateBreak() { }
}
