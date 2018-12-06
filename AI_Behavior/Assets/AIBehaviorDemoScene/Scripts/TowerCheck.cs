using UnityEngine;

namespace AIBehaviorDemo
{
	public class TowerCheck : MonoBehaviour
	{
		public bool towerTriggerComplete = false;


		void OnTriggerEnter(Collider other)
		{
			if ( !towerTriggerComplete && other.tag == "Player" )
			{
				SetTowerTrigger();
			}
		}


		public void SetTowerTrigger ()
		{
			towerTriggerComplete = true;
			gameObject.GetComponent<Renderer>().material.shader = Shader.Find("Self-Illumin/Diffuse");
			SendMessageUpwards ("SetTower");
		}
	}
}