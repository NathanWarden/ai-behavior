using UnityEngine;
using AIBehavior;

#if UNITY_EDITOR
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;


namespace AIBehaviorEditor
{
	public class AIBehaviorsComponentInfoHelper
	{
		public const BindingFlags standardBindingFlags = BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Instance;


		public static Component[] GetNonFSMComponents(GameObject gameObject)
		{
			Component[] components = gameObject.GetComponents<Component>();
			List<Component> componentsList = new List<Component>();

			for ( int i = 0; i < components.Length; i++ )
			{
				Component comp = components[i];

				if ( comp != null )
				{
					Type compType = comp.GetType();

					// Make sure it's a non FSM component
					if ( compType != typeof(Transform) && compType != typeof(AIBehaviors) && compType != typeof(AIAnimationStates) )
					{
						componentsList.Add(comp);
					}
				}
			}

			return componentsList.ToArray();
		}


		public static Component[] GetComponentsWithField(GameObject gameObject, string fieldName, Type type)
		{
			Component[] nonFSMComponents = GetNonFSMComponents(gameObject);
			List<Component> components = new List<Component>();

			for ( int i = 0; i < nonFSMComponents.Length; i++ )
			{
				FieldInfo property = nonFSMComponents[i].GetType().GetField(fieldName);

				if ( property != null && type == property.FieldType )
				{
					components.Add(nonFSMComponents[i]);
				}
			}

			return components.ToArray();
		}


		public static Component[] GetComponentsWithMethod(GameObject gameObject, string methodName, Type type)
		{
			return GetComponentsWithMethod(gameObject, methodName, new Type[1] { type });
		}


		public static Component[] GetComponentsWithMethod(GameObject gameObject, string methodName, Type[] types)
		{
			Component[] nonFSMComponents = GetNonFSMComponents(gameObject);
			List<Component> components = new List<Component>();

			for ( int i = 0; i < nonFSMComponents.Length; i++ )
			{
				MethodInfo mi = GetMethodInfo(nonFSMComponents[i], methodName, types);

				if ( mi != null )
				{
					components.Add(nonFSMComponents[i]);
				}
			}

			return components.ToArray();
		}


		public static string[] GetArrayFromComponentNames(Component[] components)
		{
			string[] names = new string[components.Length];

			for ( int i = 0; i < components.Length; i++ )
			{
				names[i] = components[i].GetType().ToString();
			}

			return names;
		}


		public static MethodInfo GetMethodInfo(Component component, string methodName)
		{
			return GetMethodInfo(component, methodName, new Type[0] );
		}


		public static MethodInfo GetMethodInfo(Component component, string methodName, Type parameterType)
		{
			return GetMethodInfo(component, methodName, new Type[1] { parameterType } );
		}


		public static MethodInfo GetMethodInfo(Component component, string methodName, Type[] parameterTypes)
		{
			MethodInfo[] methods = component.GetType().GetMethods(standardBindingFlags);

			for ( int i = 0; i < methods.Length; i++ )
			{
				if ( methods[i].Name == methodName )
				{
					ParameterInfo[] parameters = methods[i].GetParameters();

					if ( parameters.Length == parameterTypes.Length )
					{
						bool parametersSame = true;

						for ( int j = 0; j < parameters.Length; j++ )
						{
							if ( parameters[j].ParameterType != parameterTypes[j] )
							{
								parametersSame = false;
								break;
							}
						}

						if ( parametersSame )
							return methods[i];
					}
				}
			}

			return null;
		}


		public static string[] GetStateTypeNames()
		{
			List<Type> types = FindAllDerivedTypes<BaseState>();
			List<string> namesList = new List<string>();
			string[] exclusions = { "MeleeAttackState", "RangedAttackState", "Base", "StateSkeleton" };

			for ( int i = 0; i < types.Count; i++ )
			{
				object[] attributes = types[i].GetCustomAttributes(typeof(ExcludeFromStatesList), true);
				string typeName = types[i].ToString();
				bool included = true;

				if ( attributes.Length > 0 )
				{
					continue;
				}

				foreach ( string excludeName in exclusions )
				{
					if ( typeName.Contains(excludeName) )
					{
						included = false;
						break;
					}
				}

				if ( included )
				{
					namesList.Add(types[i].Name);
				}
			}

			return namesList.ToArray();
		}


		public static string[] GetTriggerTypeNames(bool returnOnlySubtriggers = false)
		{
			List<Type> types = FindAllDerivedTypes<BaseTrigger>();
			List<string> namesList = new List<string>();

			for ( int i = 0; i < types.Count; i++ )
			{
				object[] attributes = types[i].GetCustomAttributes(typeof(ExcludeFromTriggersList), true);
				string typeName = types[i].Name;

				if ( attributes.Length > 0 )
				{
					continue;
				}

				if ( returnOnlySubtriggers )
				{
					attributes = types[i].GetCustomAttributes(typeof(ExcludeFromSubTriggersList), true);

					if ( attributes.Length > 0 )
					{
						continue;
					}
				}

				if ( typeName != "TriggerSkeleton" && !typeName.Contains("Base") && typeName != "" )
				{
					namesList.Add(typeName);
				}
			}

			return namesList.ToArray();
		}


		public static string GetNameFromType(string typeString)
		{
			List<char> characters = new List<char>();
			int charNum = 0;
			int aVal = (int)'A';
			int zVal = (int)'Z';
			int lastSplitStart = 0;
			int spaceCount = 0;

			foreach ( char character in typeString )
			{
				if ( charNum++ > 0 )
				{
					int charVal = (int)character;

					if ( charVal >= aVal && charVal <= zVal )
					{
						lastSplitStart = charNum;
						spaceCount++;

						characters.Add(' ');
					}
				}

				characters.Add(character);
			}

			typeString = new string(characters.ToArray());

			typeString = typeString.Substring(0, lastSplitStart + spaceCount-2);

			return typeString;
		}


		private static List<Type> FindAllDerivedTypes<T>()
		{
			return FindAllDerivedTypes<T>(Assembly.GetAssembly(typeof(T)));
		}


		private static List<Type> FindAllDerivedTypes<T>(Assembly assembly)
		{
			Type derivedType = typeof(T);
			return assembly.GetTypes().Where(t => t != derivedType && derivedType.IsAssignableFrom(t) && !t.IsAbstract).ToList();
		}
	}
}
#endif