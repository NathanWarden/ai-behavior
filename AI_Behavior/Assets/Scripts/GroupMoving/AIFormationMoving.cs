using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AIBehavior;

public class AIFormationMoving : MonoBehaviour 
{
	List<AIBehaviors> ais = new List<AIBehaviors>();
	public FormationType formationType;
	public int count;
	public float unitSpacing;

	public enum FormationType
	{
		Rows,
		Columns
	}


	public void SetAIs( AIBehaviors[] selectedAIs )
	{
		ais.Clear();
		ais.AddRange(selectedAIs);
	}


	public void SendAIsTo( Vector3 targetPoint, string seekStateName )
	{
		List<Vector3> aisTargets = new List<Vector3>();
		Vector3 startingPoint;
		int rows = 0, cols = 0;

		// Set rows and columns
		if(formationType == FormationType.Rows)
		{
			rows = count;
			cols = ais.Count/rows;
		}
		else if(formationType == FormationType.Columns)
		{
			cols = count;
			rows = ais.Count/cols;
		}

		// Set starting point
		startingPoint = new Vector3 ( targetPoint.x - ((cols-1)*unitSpacing/2), targetPoint.y, targetPoint.z + ((rows-1)*unitSpacing/2));

		// Set points
		int currentCol = 1;
		int currentRow = 1;
		for (int i=0; i<ais.Count; i++)
		{
			Vector3 target = new Vector3 (startingPoint.x + (currentCol-1)*unitSpacing, startingPoint.y, startingPoint.z - (currentRow-1)*unitSpacing);
			aisTargets.Add (target);

			if (formationType == FormationType.Columns) 
			{
				if (currentCol == count) 
				{
					currentCol = 1;
					currentRow++;
				}
				else
					currentCol++;
			}
			else if (formationType == FormationType.Rows) 
			{
				if (currentRow == count)
				{
					currentRow = 1;
					currentCol++;
				}
				else
					currentRow++;
			}
			
		}
				
		// Set state and target to ai
		for (int i = 0; i < ais.Count; i++)
		{
			GameObject newFormationTarget = new GameObject ("FormationTarget");
			newFormationTarget.transform.position = aisTargets [i];

			SeekState formationState = ais[i].GetStateByName(seekStateName) as SeekState;
			formationState.specifySeekTarget = true;
			formationState.seekTarget = newFormationTarget.transform;
			ais[i].ChangeActiveState(formationState);
		}
	}
}
