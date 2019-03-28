using UnityEngine;
using System.Collections;

namespace AIBehavior
{
	public static class SaveIdDistributor
	{
		private static int currentId = 0;


		public static int GetId(int id)
		{
			if ( id == -1 )
			{
				id = ++currentId;
			}
			
			return id;
		}
		
		
		public static void SetId(int id)
		{
			if ( currentId < id )
			{
				currentId = id;
			}
		}
	}
}