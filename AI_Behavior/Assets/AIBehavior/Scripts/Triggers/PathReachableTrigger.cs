using UnityEngine;
using AIBehavior;
using System.Collections;
using UnityEngine.AI;

#if UNITY_EDITOR
using UnityEditor;
using AIBehaviorEditor;
#endif


public class PathReachableTrigger : BaseTrigger
{
	NavMeshAgent navMeshAgent = null;
	Vector3 previousDestination = Vector3.one * Mathf.Infinity;
	NavMeshPath path = null;

	public float ureachableThreshold = 0.1f;
	public bool ignoreYPosition = true;


	protected override void Init (AIBehaviors fsm)
	{
		if ( navMeshAgent == null )
		{
			navMeshAgent = fsm.GetComponent<NavMeshAgent>();
		}

		path = null;

		previousDestination = Vector3.one * Mathf.Infinity;
	}


	protected override bool Evaluate(AIBehaviors fsm)
	{
		if ( navMeshAgent != null )
		{
			Transform targetTransform = fsm.GetClosestPlayer(objectFinder.GetTransforms());

			if ( targetTransform != null )
			{
				Vector3 targetPosition = targetTransform.position;
				Vector3 currentDestination = targetPosition;

				if ( path == null )
				{
					path = new NavMeshPath();
				}

				if ( currentDestination != previousDestination )
				{
					previousDestination = currentDestination;
					navMeshAgent.CalculatePath(currentDestination, path);
				}

				if ( path.status != NavMeshPathStatus.PathInvalid )
				{
					Vector3[] points = path.corners;

					if ( points.Length > 0 )
					{
						Vector3 lastPoint = points[points.Length-1];
						float sqrThreshold = ureachableThreshold * ureachableThreshold;
						float sqrDist;
						bool result;

						if ( ignoreYPosition )
						{
							lastPoint.y = 0.0f;
							targetPosition.y = 0.0f;
						}

						sqrDist = Vector3.SqrMagnitude(lastPoint - targetPosition);
						result = sqrDist < sqrThreshold;

						return result;
					}
				}
			}
		}

		// This will force the trigger status to be false even if they use the invertResult option
		return !invertResult;
	}
	
	
	public override string DefaultDisplayName()
	{
		return "Path Status";
	}


	void OnDrawGizmos()
	{
		if ( path != null )
		{
			Vector3[] points = path.corners;

			for ( int i = 0; i < points.Length; i++ )
			{
				Gizmos.DrawSphere(points[i], 0.5f);
			}

			Gizmos.DrawSphere(previousDestination, 0.5f);

			/*NavMeshHit hit;
			if ( NavMesh.FindClosestEdge(previousDestination, out hit, NavMesh.AllAreas) )
			{
				Gizmos.DrawSphere(hit.position, 0.5f);
			}*/
		}
	}


#if UNITY_EDITOR
	// Implement your own custom GUI here if you want to
	public override void DrawInspectorProperties(AIBehaviors fsm, SerializedObject sObject)
	{
		if ( fsm.GetComponent<NavMeshAgent>() == null )
		{
			Color oldColor = GUI.contentColor;
			GUI.contentColor = Color.yellow;
			EditorGUILayout.HelpBox("This trigger only works with an AI that uses a NavMeshAgent", MessageType.Warning);
			GUI.contentColor = oldColor;
		}

		InspectorHelper.DrawInspector(sObject);
	}
#endif
}