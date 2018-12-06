using UnityEngine;
using UnityEngine.Rendering;


namespace AIBehaviorExamples
{
	public class Shooter : MonoBehaviour
	{
		public GameObject projectile;

		void Update ()
		{
			if ( Input.GetMouseButtonDown(0) )
			{
				GameObject go = Instantiate(projectile);
				Transform tfm = go.transform;
				Rigidbody rb = go.GetComponent<Rigidbody>();
				Transform cameraTfm = Camera.main.transform;

				go.GetComponent<Renderer>().lightProbeUsage = LightProbeUsage.BlendProbes;
				go.AddComponent<ProjectileCollider>();

				tfm.position = cameraTfm.position + cameraTfm.forward;

				rb.AddForce(cameraTfm.forward * 1500);
			}
		}
	}
}