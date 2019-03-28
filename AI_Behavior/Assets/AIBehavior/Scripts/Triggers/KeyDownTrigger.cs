using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace AIBehavior
{
	public class KeyDownTrigger : BaseTrigger
	{
		public KeyCode keycode = KeyCode.E;


		protected override bool Evaluate(AIBehaviors fsm)
		{
			// Logic here, return true if the trigger was triggered
			return Input.GetKeyDown(keycode);
		}
		
		
		public override string DefaultDisplayName()
		{
			return "Key Down";
		}
	}
}