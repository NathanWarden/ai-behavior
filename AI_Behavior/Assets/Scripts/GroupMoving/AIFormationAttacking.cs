using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AIBehavior;

public class AIFormationAttacking : MonoBehaviour 
{
	List<AIBehaviors> ais = new List<AIBehaviors>();

	public void SetAIs( AIBehaviors[] selectedAIs )
	{
		ais.Clear();
		ais.AddRange(selectedAIs);
	}

	public void SendAttack (GameObject objective, string seekAttackStateName) 
	{
		// Set state and target to ais
		for (int i = 0; i < ais.Count; i++)
		{
			SeekState formationState = ais[i].GetStateByName(seekAttackStateName) as SeekState;
			formationState.specifySeekTarget = true;
			formationState.seekTarget = objective.transform;
			ais[i].ChangeActiveState(formationState);
		}
	}
}
