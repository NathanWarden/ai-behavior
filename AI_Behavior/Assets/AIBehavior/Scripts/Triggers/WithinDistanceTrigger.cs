using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace AIBehavior
{
	public class WithinDistanceTrigger : DistanceTrigger
	{
		protected override bool Compare (float sqrMagnitude, float sqrThreshold)
		{
			return sqrMagnitude < sqrThreshold;
		}
		
		
		protected override bool ResultForNoTaggedObjectsFound()
		{
			return false;
		}
		
		
		protected override DistanceNegotiation GetDefaultNegotiationMode()
		{
			return DistanceNegotiation.Any;
		}
		
		
		public override string DefaultDisplayName()
		{
			return "Within Distance";
		}


#if UNITY_EDITOR
		protected override Color GetGizmoColor()
		{
			return Color.blue;
		}
#endif
	}
}