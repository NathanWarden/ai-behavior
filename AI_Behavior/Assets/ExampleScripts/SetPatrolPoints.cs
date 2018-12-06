using UnityEngine;
using AIBehavior;

public class SetPatrolPoints : MonoBehaviour
{
	public Transform patrolPoints;


	void Awake ()
	{
		AIBehaviors ai = GetComponent<AIBehaviors>();
		PatrolState state = ai.GetState<PatrolState>();	

		state.SetPatrolPoints(patrolPoints);
	}
}