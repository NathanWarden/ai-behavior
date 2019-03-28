using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace AIBehavior
{
	public class BeyondDistanceTrigger : DistanceTrigger
	{
		protected override bool Compare (float sqrMagnitude, float sqrThreshold)
		{
			return sqrMagnitude > sqrThreshold;
		}


		protected override bool ResultForNoTaggedObjectsFound()
		{
			return true;
		}


		protected override DistanceNegotiation GetDefaultNegotiationMode()
		{
			return DistanceNegotiation.All;
		}
		
		
		public override string DefaultDisplayName()
		{
			return "Beyond Distance";
		}
	}
}