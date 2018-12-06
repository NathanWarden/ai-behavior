using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AIBehavior
{
	public class IsObstructedFromPlayers : BaseTrigger
	{
		public float maximumCheckDistance = 100.0f;


		protected override bool Evaluate(AIBehaviors fsm)
		{
			Transform[] players = objectFinder.GetTransforms();
			Vector3 fsmPosition = fsm.GetSightPosition(fsm.sightTransform);
			float maximumSqrCheckDistance = maximumCheckDistance * maximumCheckDistance;

			for ( int i = 0; i < players.Length; i++ )
			{
				Vector3 playerPosition = players[i].position;
				float sqrDistance = (fsmPosition-playerPosition).sqrMagnitude;

				if ( sqrDistance < maximumSqrCheckDistance )
				{
					Vector3 differenceVector = playerPosition-fsmPosition;
					Vector3 direction = differenceVector.normalized;
					float distance = differenceVector.magnitude;

					Debug.DrawRay(fsmPosition, direction * distance);
					if ( !Physics.Raycast(fsmPosition, direction, distance, fsm.raycastLayers) )
					{
						return false;
					}
				}
			}

			return true;
		}
		
		
		public override string DefaultDisplayName()
		{
			return "Is Obstructed From Players";
		}
	}
}