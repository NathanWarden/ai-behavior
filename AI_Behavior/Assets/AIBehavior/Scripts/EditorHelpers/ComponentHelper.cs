using UnityEngine;
using System;

namespace AIBehavior
{
	public static class ComponentHelper
	{
		public static Component AddComponentByName (GameObject targetObject, string componentName, string namespaceName = "AIBehavior")
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
				return emptyNameSpace ? null : AddComponentByName (targetObject, componentName, "");
			}
			else
			{
				return targetObject.AddComponent(Type.GetType(typeName));
			}
		}
	}
}