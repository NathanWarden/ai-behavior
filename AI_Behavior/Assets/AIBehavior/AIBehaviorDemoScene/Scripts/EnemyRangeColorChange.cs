using UnityEngine;
using AIBehavior;

namespace AIBehaviorDemo
{
	public class EnemyRangeColorChange : MonoBehaviour
	{
		public float distanceFalloff = 15.0f;

		float startDistance = 0.0f;
		float endDistance = 0.0f;
		Material material;
		Transform playerTFM;


		void Awake()
		{
			AIBehaviors fsm = GetComponent<AIBehaviors>();

			startDistance = fsm.sightDistance;
			endDistance = startDistance + distanceFalloff;

			material = GetComponentInChildren<SkinnedMeshRenderer>().material;
			GameObject playerGO = GameObject.FindWithTag("Player");
			if ( playerGO != null )
			{
				playerTFM = playerGO.transform;
			}
		}


		void Update()
		{
			if ( playerTFM != null )
			{
				float distance = Vector3.Distance(transform.position, playerTFM.position);
				float illuminValue = 1.0f - Mathf.InverseLerp(startDistance, endDistance, distance);

				material.SetFloat("_IlluminValue", illuminValue);
			}
		}
	}
}