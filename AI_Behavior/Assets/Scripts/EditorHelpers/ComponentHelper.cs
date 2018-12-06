using UnityEngine;
using System;

namespace AIBehavior
{
	public static class ComponentHelper
	{
		public static ScriptableObject AddComponentByName (string componentName, string namespaceName = "AIBehavior")
		{
			bool emptyNameSpace = string.IsNullOrEmpty (namespaceName);
			string typeName;
			Type result;

			if (emptyNameSpace)
			{
				typeName = componentName;
			}
			else
			{
				typeName = namespaceName + "." + componentName;
			}

			result = Type.GetType(typeName);

			if ( result == null )
			{
				return emptyNameSpace ? null : AddComponentByName (componentName, "");
			}
			else
			{
				return ScriptableObject.CreateInstance(Type.GetType(typeName));
			}
		}
	}
}