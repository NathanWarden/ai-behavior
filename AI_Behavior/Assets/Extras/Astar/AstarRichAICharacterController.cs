#if USE_ASTAR_PRO
using UnityEngine;
using System.Collections;
using Pathfinding;

namespace AIBehavior
{
	[RequireComponent(typeof(RichAI))]
	[RequireComponent(typeof(CharacterController))]
	public class AstarRichAICharacterController : MonoBehaviour
	{
		public Transform target = null;
		private RichAI richAI = null;
		
		
		void Awake()
		{
			AIBehaviors ai = GetComponent<AIBehaviors>();
			
			if ( target == null )
			{
				target = new GameObject().transform;
			}
			
			richAI = GetComponent<RichAI>();
			richAI.target = target;
			
			if ( richAI == null )
			{
				Debug.LogError("You must add the 'RichAI' component to the Game Object '" + name + "' in order to use the Astar Pathfinding Project integration.");
				return;
			}
			
			richAI.target = target;
			ai.externalMove = OnMove;
		}
		
		
		void OnMove(Vector3 targetPoint, float targetSpeed, float rotationSpeed)
		{
			richAI.maxSpeed = targetSpeed;
			richAI.rotationSpeed = rotationSpeed;
			target.position = targetPoint;
		}


		void OnDestroy()
		{
			Destroy(target.gameObject);
		}
	}
}
#endif