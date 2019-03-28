using UnityEngine;
using AIBehavior;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif


public class InSameLayerTrigger : BaseTrigger
{
	protected override bool Evaluate(AIBehaviors fsm)
	{
		Transform[] targets = objectFinder.GetTransforms();
		int layer = fsm.gameObject.layer;

		for ( int i = 0; i < targets.Length; i++ )
		{
			if ( targets[i].gameObject.layer == layer )
			{
				return true;
			}
		}

		return false;
	}
	
	
	public override string DefaultDisplayName()
	{
		return "In Same Layer";
	}
}