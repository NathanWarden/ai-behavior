using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AIBehavior;

public class NavMeshBaker : MonoBehaviour 
{
	public NavMeshSurface[] navMeshes;

	void Start () 
	{
		Bake();
	}

	public void Bake()
	{
		for(int i = 0; i < navMeshes.Length; i++)
		{
			navMeshes[i].BuildNavMesh();
		}
	}
}
