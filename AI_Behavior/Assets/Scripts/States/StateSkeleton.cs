using UnityEngine;
using AIBehavior;


public class StateSkeleton : BaseState
{
	protected override void Init(AIBehaviors fsm)
	{
	}


	protected override void StateEnded(AIBehaviors fsm)
	{
	}


	protected override bool Reason(AIBehaviors fsm)
	{
		return true;
	}


	protected override void Action(AIBehaviors fsm)
	{
	}
	
	
	public override string DefaultDisplayName()
	{
		return "Skeleton";
	}
}