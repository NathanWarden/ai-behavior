#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using AIBehavior;
using System;
using System.IO;
using System.Reflection;
using UnityEngine.AI;


namespace AIBehaviorEditor
{
	public class AIBehaviorsMenuItems : Editor
	{
		const string rootMenu = "Tools/AI Behavior/";

		[MenuItem(rootMenu + "Mecanim Setup", true, 0)]
		public static bool MecanimSetupValidator()
		{
			return GetGameObject() != null && GetGameObject().GetComponent<Animator>() != null;
		}


		[MenuItem(rootMenu + "Mecanim Setup", false, 0)]
		public static void MecanimSetup()
		{
			GameObject go = GetGameObject();

			go.AddComponent<AIBehaviors>();
			go.AddComponent<MecanimAnimation>();
			go.AddComponent<MecanimNavMeshPathScript>();
		}


		[MenuItem(rootMenu + "Legacy Setup", true, 0)]
		public static bool LegacySetupValidator()
		{
			return GetGameObject() != null;
		}
		

		[MenuItem(rootMenu + "Legacy Setup", false, 0)]
		public static void LegacySetup()
		{
			GameObject go = GetGameObject();

			go.AddComponent<AIBehaviors>();
			go.AddComponent<CharacterAnimator>();
			go.AddComponent<NavMeshAgent>();
		}


		[MenuItem(rootMenu + "AI Behavior Component Only", true, 0)]
		public static bool AddAIBehaviorsComponentValidator()
		{
			return GetGameObject() != null;
		}


		[MenuItem(rootMenu + "AI Behavior Component Only", false, 0)]
		public static void AddAIBehaviorsComponent()
		{
			GetGameObject().AddComponent(typeof(AIBehaviors));
		}


		private static GameObject GetGameObject()
		{
			return Selection.activeObject as GameObject;
		}


		const string astarDefine = "USE_ASTAR";

		[MenuItem(rootMenu + "Other/Enable Astar Pathfinding Project Integration", false, 11)]
		public static void EnableAstar()
		{
			AddSmcsDefine(astarDefine);
		}


		[MenuItem(rootMenu + "Other/Disable Astar Pathfinding Project Integration", false, 11)]
		public static void DisableAstar()
		{
			RemoveSmcsDefine(astarDefine);
		}


		const string astarProDefine = "USE_ASTAR_PRO";

		[MenuItem(rootMenu + "Other/Enable Astar Pathfinding Project PRO Integration", false, 11)]
		public static void EnableAstarPro()
		{
			AddSmcsDefine(astarProDefine);
		}


		[MenuItem(rootMenu + "Other/Disable Astar Pathfinding Project PRO Integration", false, 11)]
		public static void DisableAstarPro()
		{
			RemoveSmcsDefine(astarProDefine);
		}


		const string ufpsDefine = "USE_UFPS";

		[MenuItem(rootMenu + "Other/Enable UFPS Integration", false, 11)]
		public static void EnableUFPS()
		{
			AddSmcsDefine(ufpsDefine);
		}


		[MenuItem(rootMenu + "Other/Disable UFPS Integration", false, 11)]
		public static void DisableUFPS()
		{
			RemoveSmcsDefine(ufpsDefine);
		}


		const string playmakerDefine = "USE_PLAYMAKER";

		[MenuItem(rootMenu + "Other/Enable PlayMaker Integration", false, 11)]
		public static void EnablePlayMaker()
		{
			AddSmcsDefine(playmakerDefine);
		}


		[MenuItem(rootMenu + "Other/Disable PlayMaker Integration", false, 11)]
		public static void DisablePlayMaker()
		{
			RemoveSmcsDefine(playmakerDefine);
		}


		static void AddSmcsDefine(string defineName)
		{
			string smcsPath = GetSmcsPath();
			string defineLine = "-define:" + defineName;
			string smcsText = "";

			if ( !File.Exists(smcsPath) )
			{
				File.WriteAllText(smcsPath, "");
			}
			else
			{
				smcsText = File.ReadAllText(smcsPath);
			}

			string[] lines = smcsText.Split('\n');

			if ( SmcsHasDefine(lines, defineLine) )
			{
				Debug.LogWarning(defineName + " already defined.");
				return;
			}

			smcsText += "\n" + defineLine;
			File.WriteAllText(smcsPath, smcsText);

			AssetDatabase.Refresh();

			Debug.LogWarning(defineName + " now defined. You may need to reimport any script for the changes to take affect.");
		}


		static void RemoveSmcsDefine(string defineName)
		{
			string smcsPath = GetSmcsPath();

			if ( File.Exists(smcsPath) )
			{
				string smcsText = File.ReadAllText(smcsPath);
				string defineLine = "-define:" + defineName;
				string[] lines = smcsText.Split('\n');

				if ( SmcsHasDefine(lines, defineLine, true) )
				{
					string output = "";

					for ( int i = 0; i < lines.Length; i++ )
					{
						string line = StripWhiteSpace(lines[i]);

						if ( !string.IsNullOrEmpty(line) )
						{
							output += line;

							if ( i+1 < lines.Length )
							{
								output += "\n";
							}
						}
					}

					File.WriteAllText(smcsPath, output);
				}

				AssetDatabase.Refresh();
			}
		}


		static bool SmcsHasDefine(string[] lines, string defineLine, bool removeIfExists = false)
		{
			for ( int i = 0; i < lines.Length; i++ )
			{
				if ( StripWhiteSpace(lines[i]).Equals(defineLine) )
				{
					if ( removeIfExists )
					{
						lines[i] = "";
					}

					return true;
				}
			}

			return false;
		}


		static string StripWhiteSpace(string source)
		{
			return System.Text.RegularExpressions.Regex.Replace(source, @"\s+", "");
		}


		static string GetSmcsPath()
		{
			return Application.dataPath + "/smcs.rsp";
		}


		/*[MenuItem(rootMenu + "Documentation", false, 22)]
		public static void Documentation()
		{
			Application.OpenURL("http://walkerboystudio.com/html/aibehavior/index.html");
		}*/
		

		[MenuItem(rootMenu + "About AI Behavior", false, 22)]
		public static void About()
		{
			AIBehaviorsAboutWindow.ShowAboutWindow();
		}


		[MenuItem(rootMenu + "Contact Us or Report a Bug (via your email client)", false, 22)]
		public static void ReportBug()
		{
			Application.OpenURL("mailto:py_daniel@hotmail.com");
		}
	}
}
#endif