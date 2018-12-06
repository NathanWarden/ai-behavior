#if UNITY_EDITOR
using UnityEngine;
using System.Collections;

namespace AIBehaviorEditor
{
	public class AIBehaviorsStyles
	{
		public GUIContent blankContent = new GUIContent();

		public GUIStyle upStyle = new GUIStyle();
		public GUIStyle downStyle = new GUIStyle();
		public GUIStyle addStyle = new GUIStyle();
		public GUIStyle removeStyle = new GUIStyle();

		public int arrowButtonWidths { get { return 18; } }
		public int addRemoveButtonWidths { get { return 16; } }


		public AIBehaviorsStyles()
		{
			InitStyles();
		}


		void InitStyles()
		{
			upStyle.normal.background = LoadButtonImage("up");
			upStyle.active.background = LoadButtonImage("up_rollover");

			downStyle.normal.background = LoadButtonImage("down");
			downStyle.active.background = LoadButtonImage("down_rollover");

			addStyle.normal.background = LoadButtonImage("add");
			addStyle.active.background = LoadButtonImage("add_rollover");

			removeStyle.normal.background = LoadButtonImage("remove");
			removeStyle.active.background = LoadButtonImage("remove_rollover");
		}


		Texture2D LoadButtonImage(string imageName)
		{
			string path = "Assets/AIBehavior/Editor/AIBehaviorsMadeEasy/Images/" + imageName + ".png";
			return UnityEditor.AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D)) as Texture2D;
		}
	}
}
#endif