using UnityEngine;
using AIBehavior;
using System.Collections;

public class CachePlayerTagsOnAwake : MonoBehaviour
{
	void Awake ()
	{
		AIBehaviors[] ais = FindObjectsOfType<AIBehaviors>();

		for ( int i = 0; i < ais.Length; i++ )
		{
			ais[i].objectFinder.CacheTransforms(CachePoint.Awake);
			ais[i].objectFinder.CacheTransforms(CachePoint.StateChanged);
			ais[i].objectFinder.CacheTransforms(CachePoint.EveryFrame);
		}
	}
}