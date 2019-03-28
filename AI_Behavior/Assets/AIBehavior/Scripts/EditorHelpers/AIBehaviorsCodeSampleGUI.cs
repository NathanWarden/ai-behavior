#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;


namespace AIBehaviorEditor
{
	public class AIBehaviorsCodeSampleGUI
	{
		const string scriptTypeKey = "AIBehaviors_ScriptType";


		public enum ScriptType	
		{
			JS,
			CS
		}


		public static void Draw(Type dataType, string parameterName, string methodName)
		{
			Draw(dataType, parameterName, 1, new string[1] { methodName });
		}


		public static void Draw(Type dataType, string parameterName, int methodCount, string[] methodNames)
		{
			string displayString = "";
			string typeString = dataType.ToString();

			Color oldColor = GUI.color;
			GUI.color = Color.yellow;
			GUILayout.Label("No components with appropriate method" + (methodCount > 1 ? "s" : "") + " found!", EditorStyles.boldLabel);
			GUI.color = oldColor;

			ScriptType scriptType = DrawScriptTypeSelection();

			if ( scriptType == ScriptType.JS )
			{
				for ( int i = 0; i < methodCount; i++ )
				{
					displayString += "function " + methodNames[i] + "(" + parameterName + " : " + typeString + ")\n";
					displayString += "{\n";
					displayString += "\t// Code Here...\n";
					displayString += "}";

					if ( i < methodCount-1 )
					{
						displayString += "\n\n";
					}
				}
			}
			else if ( scriptType == ScriptType.CS )
			{
				displayString = "using UnityEngine;\n";
				displayString += "using System.Collections;\n\n";
				displayString += "public class MyClass : MonoBehaviour\n";
				displayString += "{\n";

				for ( int i = 0; i < methodCount; i++ )
				{
					displayString += "\tpublic void " + methodNames[i] + "(" + typeString + " " + parameterName + ")\n";
					displayString += "\t{\n";
					displayString += "\t\t// Code Here...\n";
					displayString += "\t}\n";

					if ( i < methodCount-1 )
					{
						displayString += "\n";
					}
				}

				displayString += "}";
			}

			GUILayout.TextArea(displayString);
		}


		public static ScriptType DrawScriptTypeSelection()
		{
			ScriptType scriptType = ScriptType.JS;

			if ( EditorPrefs.HasKey(scriptTypeKey) )
			{
				scriptType = (ScriptType)EditorPrefs.GetInt(scriptTypeKey);
			}

			GUILayout.BeginHorizontal();
			{
				GUILayout.Label("Example:", EditorStyles.boldLabel);

				if ( GUILayout.Toggle(scriptType == ScriptType.JS, "JS", EditorStyles.radioButton) )
				{
					if ( scriptType != ScriptType.JS )
					{
						scriptType = ScriptType.JS;
						EditorPrefs.SetInt(scriptTypeKey, (int)scriptType);
					}
				}

				if ( GUILayout.Toggle(scriptType == ScriptType.CS, "C#", EditorStyles.radioButton) )
				{
					if ( scriptType != ScriptType.CS )
					{
						scriptType = ScriptType.CS;
						EditorPrefs.SetInt(scriptTypeKey, (int)scriptType);
					}
				}
			}
			GUILayout.EndHorizontal();

			return scriptType;
		}
	}
}
#endif