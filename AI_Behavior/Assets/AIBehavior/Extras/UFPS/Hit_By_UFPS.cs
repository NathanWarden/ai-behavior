#if USE_UFPS
using AIBehavior;
using UnityEngine;
using System.Collections;

// Add this script directly to your AI Behaviors agent

public class Hit_By_UFPS : vp_DamageHandler
{
	AIBehaviors ai;
	public float damageMultiplier = 1.0f;


	protected override void Awake ()
	{
		base.Awake ();
		ai = GetComponentInParent<AIBehaviors>();
	}


	public override void Damage(vp_DamageInfo damageInfo)
	{
		ai.GotHit(damageInfo.Damage * damageMultiplier);
	}
}
#endif