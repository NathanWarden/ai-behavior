#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace AIBehaviorEditor
{
	public class AIBehaviorsGeneralEditorGUI
	{
		public static void Separator()
		{
			float dummyValue = 0.0f;
			EditorGUILayout.MinMaxSlider(ref dummyValue, ref dummyValue, 0, 0);
		}


		public static void DrawWarning(string label)
		{
			DrawColoredLabel(label, Color.yellow);
		}


		public static void DrawError(string label)
		{
			DrawColoredLabel(label, Color.red);
		}


		public static void DrawColoredLabel(string label, Color color)
		{
			Color oldColor = GUI.color;
			GUI.color = color;
			GUILayout.Label(label);
			GUI.color = oldColor;
		}
	}
}
#endif