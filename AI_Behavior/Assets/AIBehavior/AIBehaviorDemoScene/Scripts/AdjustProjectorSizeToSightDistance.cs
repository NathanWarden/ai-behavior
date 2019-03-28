using UnityEngine;
using AIBehavior;

namespace AIBehaviorDemo
{
	public class AdjustProjectorSizeToSightDistance : MonoBehaviour
	{
		void Awake()
		{
			GetComponent<Projector>().orthographicSize = transform.parent.GetComponent<AIBehaviors>().sightDistance;
		}
	}
}