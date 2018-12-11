using UnityEngine;
using System.Collections;

namespace AIBehavior
{
	public class AIComponent : MonoBehaviour
	{
		// === General Items === //

		protected string displayName = "";


		public AIComponent()
		{
			displayName = DefaultDisplayName ();
		}


		public virtual string DefaultDisplayName()
		{
			return GetType().ToString();
		}


		public virtual string GetDisplayName()
		{
			return displayName;
		}


		public virtual void SetDisplayName(string newDisplayName)
		{
			displayName = newDisplayName;
		}


		// === Save Variables === //
		
		public int saveId = -1;


		// === Save Methods === //
		
		public int GetSaveID()
		{
			saveId = SaveIdDistributor.GetId(saveId);
			return saveId;
		}
	
	
		public void SetSaveID(int id)
		{
			saveId = id;
			SaveIdDistributor.SetId(id);
		}
	}
}