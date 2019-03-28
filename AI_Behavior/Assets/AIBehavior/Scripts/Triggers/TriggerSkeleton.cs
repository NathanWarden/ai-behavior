using UnityEngine;
using AIBehavior;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif


public class TriggerSkeleton : BaseTrigger
{
	protected override bool Evaluate(AIBehaviors fsm)
	{
		// Logic here, return true if the trigger was triggered
		return true;
	}
	
	
	public override string DefaultDisplayName()
	{
		return "Skeleton";
	}


#if UNITY_EDITOR
	/*
	// Implement your own custom GUI here if you want to
	public override void DrawInspectorProperties(AIBehaviors fsm, SerializedObject sObject)
	{
	}
	*/
#endif
}