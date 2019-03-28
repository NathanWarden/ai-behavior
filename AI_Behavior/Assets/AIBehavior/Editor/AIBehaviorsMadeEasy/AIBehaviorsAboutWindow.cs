#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;


namespace AIBehaviorEditor
{
	public class AIBehaviorsAboutWindow : EditorWindow
	{
		GUIStyle titleStyle = new GUIStyle(EditorStyles.whiteLargeLabel);
		GUIStyle labelStyle = new GUIStyle(EditorStyles.label);
		GUIStyle linkStyle = new GUIStyle(EditorStyles.boldLabel);


		public static void ShowAboutWindow()
		{
			AIBehaviorsAboutWindow window = AIBehaviorsAboutWindow.GetWindow<AIBehaviorsAboutWindow>(true, "About...");

			window.minSize = window.maxSize = new Vector2(300.0f, 220.0f);

			window.linkStyle.alignment = window.labelStyle.alignment = window.titleStyle.alignment = TextAnchor.MiddleCenter;

			window.Show();
		}


		void OnGUI()
		{
			DrawCenteredLabel("AI Behavior", titleStyle);

			EditorGUILayout.Separator();

			DrawCenteredLabel("Maintained by:");
			DrawCenteredLabel("Daniel Py");

			EditorGUILayout.Separator();

			DrawCenteredLabel("Originally Designed by:");
			DrawCenteredLabel("Walker Boys Studio\n");
			DrawCenteredLabel("Chad Walker");
			DrawCenteredLabel("Eric Walker");
			DrawCenteredLabel("Nathan Warden");

			EditorGUILayout.Separator();

			GUILayout.BeginHorizontal();
			{
				GUILayout.Label("");
				if ( GUILayout.Button("www.walkerboystudio.com", linkStyle) )
				{
					Application.OpenURL("http://www.walkerboystudio.com/");
				}
				GUILayout.Label("");
			}
			GUILayout.EndHorizontal();
		}


		private void DrawCenteredLabel(string label)
		{
			DrawCenteredLabel(label, labelStyle);
		}


		private void DrawCenteredLabel(string label, GUIStyle guiStyle)
		{
			GUILayout.BeginHorizontal();
			{
				GUILayout.Label("");
				GUILayout.Label(label, guiStyle);
				GUILayout.Label("");
			}
			GUILayout.EndHorizontal();
		}
	}
}
#endif