using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneGenerator : MonoBehaviour
{
	public NavMeshBaker baker;
	public GameObject sceneBlock;
	public Transform terrain;

	void Start () 
	{
		// First ramps
		Instantiate(sceneBlock, new Vector3(-10, 0.8f, 0), Quaternion.Euler(new Vector3(60, 0, 0)), terrain);
		Instantiate(sceneBlock, new Vector3(10, 0.8f, 0), Quaternion.Euler(new Vector3(60, 0, 0)), terrain);
		// First platforms
		for(int i = -10; i <= 10; i+=4)
		{
			Instantiate(sceneBlock, new Vector3(i, 2, 4.5f), Quaternion.Euler(new Vector3(90, 0, 0)), terrain);
		}
		// Second ramps
		Instantiate(sceneBlock, new Vector3(-2, 3.3f, 9.4f), Quaternion.Euler(new Vector3(60, 0, 0)), terrain);
		Instantiate(sceneBlock, new Vector3(2, 3.3f, 9.4f), Quaternion.Euler(new Vector3(60, 0, 0)), terrain);
		// Third ramp
		Instantiate(sceneBlock, new Vector3(0, 5.8f, 13.7f), Quaternion.Euler(new Vector3(60, 0, 0)), terrain);

		baker.Bake();
	}
}
