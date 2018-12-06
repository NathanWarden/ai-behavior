#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;


namespace AIBehaviorEditor
{
	public class AIBehaviorsAssignableObjectArray
	{
		public static void Draw(SerializedObject m_Object, string propertyName)
		{
			string kArraySize = propertyName + ".Array.size";
			string kArrayData = propertyName + ".Array.data[{0}]";
			int arraySize = m_Object.FindProperty(kArraySize).intValue;

			for ( int i = 0; i < arraySize; i++ )
			{
				SerializedProperty m_Prop = m_Object.FindProperty(string.Format(kArrayData, i));

				GUILayout.BeginHorizontal();

				EditorGUILayout.PropertyField(m_Prop, new GUIContent("Hi how are you... will this label appear"));

				GUI.enabled = i > 0;
				if ( GUILayout.Button("U", GUILayout.MaxWidth(22)) )
				{
					Swap(m_Object, i, i-1, kArrayData);
				}

				GUI.enabled = i < arraySize-1;
				if ( GUILayout.Button("D", GUILayout.MaxWidth(22)) )
				{
					Swap(m_Object, i, i+1, kArrayData);
				}

				GUI.enabled = true;
				if ( GUILayout.Button("Remove", GUILayout.MaxWidth(65)) )
				{
					RemoveObjectAtIndex(m_Object, i, kArrayData);
					break;
				}

				GUILayout.Space(10);

				GUILayout.EndHorizontal();
			}

			m_Object.ApplyModifiedProperties();

		}


		public static void Swap(SerializedObject m_Object, int i1, int i2, string kArrayData)
		{
			Object temp = m_Object.FindProperty(string.Format(kArrayData, i1)).objectReferenceValue;
			m_Object.FindProperty(string.Format(kArrayData, i1)).objectReferenceValue = m_Object.FindProperty(string.Format(kArrayData, i2)).objectReferenceValue;
			m_Object.FindProperty(string.Format(kArrayData, i2)).objectReferenceValue = temp;
		}


		public static void RemoveObjectAtIndex(SerializedObject m_Object, int index, string propertyName)
		{
			string kArraySize = propertyName + ".Array.size";
			string kArrayData = propertyName + ".Array.data[{0}]";
			int arraySize = m_Object.FindProperty(kArraySize).intValue;
			int curIndex = 0;

			for ( int i = 0; i < arraySize; i++ )
			{
				if ( i != index )
				{
					Object obj = m_Object.FindProperty(string.Format(kArrayData, i)).objectReferenceValue;

					m_Object.FindProperty(string.Format(kArrayData, curIndex)).objectReferenceValue = obj;
					curIndex++;
				}
			}

			m_Object.FindProperty(kArraySize).intValue--;
		}


		public static void RemoveStringAtIndex(SerializedObject m_Object, int index, string propertyName)
		{
			string kArraySize = propertyName + ".Array.size";
			string kArrayData = propertyName + ".Array.data[{0}]";
			int arraySize = m_Object.FindProperty(kArraySize).intValue;
			int curIndex = 0;

			for ( int i = 0; i < arraySize; i++ )
			{
				if ( i != index )
				{
					string str = m_Object.FindProperty(string.Format(kArrayData, i)).stringValue;

					m_Object.FindProperty(string.Format(kArrayData, curIndex)).stringValue = str;
					curIndex++;
				}
			}

			m_Object.FindProperty(kArraySize).intValue--;
		}
	}
}
#endif