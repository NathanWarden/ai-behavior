using UnityEngine;
using AIBehavior;
using System.Collections;


namespace AIBehaviorExamples
{
	[RequireComponent(typeof(LineRenderer))]
	public class ExampleLaserAttackComponent : MonoBehaviour
	{
		public Transform shootPoint;
		LineRenderer lineRenderer;

		float hideTime = 0.0f;
		float laserVisibleDuration = 0.1f;
		bool laserShowing = false;


		void Awake()
		{
			lineRenderer = GetComponent<LineRenderer>();
		}


		public void LaserAttack(AttackData attackData)
		{
			if ( attackData.target != null )
			{
				AIBehaviors ai = attackData.target.GetComponent<AIBehaviors>();

				lineRenderer.positionCount = 2;
				lineRenderer.SetPosition(0, shootPoint.position);
				lineRenderer.SetPosition(1, attackData.target.position);

				hideTime = Time.time + laserVisibleDuration;
				StartCoroutine (HideRenderer());

				if ( ai != null )
				{
					ai.Damage(attackData.damage);
				}
			}
			else
			{
				Debug.LogWarning("attackData.target is null, you may want to have a NoPlayerInSight trigger on the AI '" + attackData.attackState.transform.parent.name + "'");
			}
		}


		IEnumerator HideRenderer ()
		{
			if ( !laserShowing )
			{
				laserShowing = true;

				lineRenderer.startColor = lineRenderer.endColor = Color.red;

				while ( Time.time < hideTime )
				{
					yield return null;
				}

				lineRenderer.startColor = lineRenderer.endColor = Color.clear;
				lineRenderer.positionCount = 0;

				laserShowing = false;
			}
		}
	}
}