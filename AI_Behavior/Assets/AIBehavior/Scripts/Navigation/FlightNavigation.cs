using UnityEngine;
using System.Collections;

namespace AIBehavior
{
	public class FlightNavigation : MonoBehaviour
	{
		public FlightArea[] flightAreas = new FlightArea[0];
		public int lookAheadPoints = 2;

		public delegate void GotPathDelegate(Vector3[] path);
		public delegate void GotPathFailedDelegate(Vector3 attemptedDestination);


		public virtual void GetPath(Vector3 startPoint, Vector3 destination, GotPathDelegate onGotPath, GotPathFailedDelegate onGotPathFailed)
		{
			if (onGotPath != null)
			{
				onGotPath (new Vector3[] { startPoint, destination });
			}
		}


		void OnDrawGizmosSelected()
		{
			for (int i = 0; i < flightAreas.Length; i++)
			{
				flightAreas [i].OnDrawGizmosSelected ();
			}
		}
	}
}