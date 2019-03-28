using UnityEngine;
using System;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
using AIBehaviorEditor;
#endif

namespace AIBehavior
{
	[Serializable]
	public class TaggedObjectFinder
	{
		public bool useCustomTags = false;
		public string[] tags = new string[] { "Player" };
		private Transform[] taggedTransforms = new Transform[0];
		public CachePoint cachePoint = CachePoint.Awake;


		public Transform[] GetTransforms()
		{
			for ( int i = 0; i < taggedTransforms.Length; i++ )
			{
				if ( taggedTransforms[i] == null )
				{
					CleanupNullTransforms();
					break;
				}
			}

			for ( int i = 0; i < taggedTransforms.Length; i++ )
			{
				bool match = false;

				for ( int j = 0; j < tags.Length; j++ )
				{
					if ( taggedTransforms[i].tag.Equals(tags[j]) )
					{
						match = true;
					}
				}

				if ( !match )
				{
					CleanupMismatchedTransforms();
					break;
				}
			}

			return taggedTransforms;
		}


		public void CacheTransforms()
		{
			CacheTransforms(cachePoint);
		}


		public virtual void CacheTransforms(CachePoint potentialCachePoint)
		{
			if ( potentialCachePoint == cachePoint )
			{
				List<Transform> transforms = new List<Transform>();

				for ( int i = 0; i < tags.Length; i++ )
				{
					GameObject[] gos = GetGameObjectsWithTag(tags[i]);

					for ( int j = 0; j < gos.Length; j++ )
					{
						transforms.Add(gos[j].transform);
					}
				}

				taggedTransforms = transforms.ToArray();
			}
		}


		protected virtual GameObject[] GetGameObjectsWithTag (string tag)
		{
			return GameObject.FindGameObjectsWithTag(tag);
		}


		void CleanupNullTransforms ()
		{
			List<Transform> tfmList = new List<Transform>(taggedTransforms);

			for ( int i = 0; i < tfmList.Count; i++ )
			{
				if ( tfmList[i] == null )
				{
					tfmList.RemoveAt(i);
					i--;
				}
			}

			taggedTransforms = tfmList.ToArray();
		}


		void CleanupMismatchedTransforms ()
		{
			List<Transform> tfmList = new List<Transform>(taggedTransforms);

			for ( int i = 0; i < tfmList.Count; i++ )
			{
				bool match = false;

				for ( int j = 0; j < tags.Length; j++ )
				{
					if ( tfmList[i].tag.Equals(tags[j]) )
					{
						match = true;
					}
				}

				if ( !match )
				{
					tfmList.RemoveAt(i);
					i--;
				}
			}

			taggedTransforms = tfmList.ToArray();
		}


#if UNITY_EDITOR
		const string kTagsArraySize = "tags.Array.size";
		const string kTagsArrayData = "tags.Array.data[{0}]";


		public void DrawPlayerTagsSelection(AIBehaviors fsm, SerializedObject m_Object, string propertyName, bool isGlobal)
		{
			SerializedProperty useCustomTagsProperty = m_Object.FindProperty(propertyName + ".useCustomTags");
			SerializedProperty cachePointProperty = m_Object.FindProperty(propertyName + ".cachePoint");
			SerializedProperty m_TagsCount = m_Object.FindProperty(propertyName + "." + kTagsArraySize);
			int tagCount = m_TagsCount.intValue;

			GUILayout.BeginVertical(useCustomTags ? GUI.skin.box : GUIStyle.none);
			{
				if ( isGlobal )
				{
					if ( useCustomTagsProperty.boolValue != useCustomTags )
					{
						useCustomTagsProperty.boolValue = useCustomTags;
					}
				}
				else
				{
					EditorGUILayout.PropertyField(useCustomTagsProperty);
				}

				if ( useCustomTagsProperty.boolValue || isGlobal )
				{
					GUILayout.BeginVertical(isGlobal ? GUI.skin.box : GUIStyle.none);
					{
						GUILayout.Label ("AI Target Tags");
						
						EditorGUILayout.Separator();

						GUILayout.BeginHorizontal();
						{
							if ( GUILayout.Button("Add", GUILayout.MaxWidth(75)) )
							{
								m_TagsCount.intValue++;
							}
							GUILayout.Label("");
						}
						GUILayout.EndHorizontal();

						for ( int i = 0; i < tagCount; i++ )
						{
							SerializedProperty m_Prop = m_Object.FindProperty(string.Format(propertyName + "." + kTagsArrayData, i));
				
							GUILayout.BeginHorizontal();
							{
								string oldTag = m_Prop.stringValue;
								string newTag = EditorGUILayout.TagField(oldTag);
					
								if ( oldTag != newTag )
									m_Prop.stringValue = newTag;
					
								if ( GUILayout.Button("Remove") )
								{
									AIBehaviorsAssignableObjectArray.RemoveStringAtIndex(m_Object, i, propertyName + ".tags");
									break;
								}
							}
							GUILayout.EndHorizontal();
						}

						EditorGUILayout.Separator();
						EditorGUILayout.PropertyField(cachePointProperty);
					}
					GUILayout.EndVertical();
				}
				GUILayout.EndVertical();
			}

			EditorGUILayout.Separator();

			m_Object.ApplyModifiedProperties();
		}
#endif
	}
}