using UnityEngine;

namespace AIBehaviorDemo
{
	public class SpaceShipCheck : MonoBehaviour
	{
		public GameManager gameManger;


		void OnTriggerEnter(Collider other)
		{
			if ( other.tag == "Player" )
			{
				if ( gameManger.AllTowersDeactivated() )
				{
					gameManger.Win();
				}
				else
				{
					gameManger.QueueLabel("You need to deactivate all towers before leaving!");
				}
			}
		}
	}
}