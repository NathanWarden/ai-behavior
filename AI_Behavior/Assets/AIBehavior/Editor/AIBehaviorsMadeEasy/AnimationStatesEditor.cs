#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using AIBehavior;
using System.Collections.Generic;


namespace AIBehaviorEditor
{
	[CustomEditor(typeof(AIAnimationStates))]
	public class AnimationStatesEditor : Editor
	{
		SerializedObject m_Object;
		SerializedProperty animationStatesProp;
		SerializedProperty m_AnimationStatesCount;

		const string kArraySize = "states.Array.size";
		const string kArrayData = "states.Array.data[{0}]";

		Transform transform;
		AIAnimationStates animStates;
		GameObject statesGameObject;

		AIBehaviorsStyles styles;


		void OnEnable()
		{
			styles = new AIBehaviorsStyles();

			m_Object = new SerializedObject(target);
			animationStatesProp = m_Object.FindProperty("states");
			m_AnimationStatesCount = m_Object.FindProperty(kArraySize);

			animStates = m_Object.targetObject as AIAnimationStates;
			transform = animStates.transform;

			InitStatesGameObject();
		}


		void InitStatesGameObject()
		{
			SerializedProperty m_Prop = m_Object.FindProperty("animationStatesGameObject");

			statesGameObject = m_Prop.objectReferenceValue as GameObject;

			if ( statesGameObject == null )
			{
				statesGameObject = new GameObject("AnimationStates");
				m_Prop.objectReferenceValue = statesGameObject;

				statesGameObject.transform.parent = transform;
				statesGameObject.transform.localPosition = Vector3.zero;

				m_Object.ApplyModifiedProperties();
			}
		}


		public override void OnInspectorGUI()
		{
			m_Object.Update();

			int arraySize = m_AnimationStatesCount.intValue;
			AIAnimationState[] states = new AIAnimationState[arraySize+1];
			SerializedProperty animationType = m_Object.FindProperty("animationType");

			EditorGUILayout.Separator();
			EditorGUILayout.PropertyField(animationType);

			for ( int i = 0; i < arraySize; i++ )
			{
				string stateNameLabel = "";
				bool oldEnabled = GUI.enabled;

				if ( m_Object.FindProperty(string.Format(kArrayData, i)) == null )
				{
					AIBehaviorsAssignableObjectArray.RemoveObjectAtIndex(m_Object, i, "states");
					continue;
				}

				states[i] = m_Object.FindProperty(string.Format(kArrayData, i)).objectReferenceValue as AIAnimationState;

				if ( states[i] == null )
				{
					AIBehaviorsAssignableObjectArray.RemoveObjectAtIndex(m_Object, i, "states");
					continue;
				}

				GUILayout.BeginHorizontal();

				if ( string.IsNullOrEmpty(states[i].name) )
					stateNameLabel = "Untitled animation";
				else
					stateNameLabel = states[i].name;

				states[i].foldoutOpen = EditorGUILayout.Foldout(states[i].foldoutOpen, stateNameLabel, EditorStyles.foldoutPreDrop);

				GUI.enabled = i > 0;
				if ( GUILayout.Button(styles.blankContent, styles.upStyle, GUILayout.MaxWidth(styles.arrowButtonWidths)) )
				{
					animationStatesProp.MoveArrayElement(i, i-1);
				}

				GUI.enabled = i < arraySize-1;
				if ( GUILayout.Button(styles.blankContent, styles.downStyle, GUILayout.MaxWidth(styles.arrowButtonWidths)) )
				{
					animationStatesProp.MoveArrayElement(i, i+1);
				}

				GUI.enabled = true;
				if ( GUILayout.Button(styles.blankContent, styles.addStyle, GUILayout.MaxWidth(styles.addRemoveButtonWidths)) )
				{
					animationStatesProp.InsertArrayElementAtIndex(i);
					animationStatesProp.GetArrayElementAtIndex(i+1).objectReferenceValue = statesGameObject.AddComponent<AIAnimationState>();
				}

				GUI.enabled = arraySize > 1;
				if ( GUILayout.Button(styles.blankContent, styles.removeStyle, GUILayout.MaxWidth(styles.addRemoveButtonWidths)) )
				{
					AIBehaviorsAssignableObjectArray.RemoveObjectAtIndex(m_Object, i, "states");
					DestroyImmediate(m_Object.targetObject as AIAnimationState);
					break;
				}
				GUI.enabled = oldEnabled;

				GUILayout.Space(10);

				GUILayout.EndHorizontal();

				GUILayout.Space(2);

				if ( states[i].foldoutOpen )
				{
					DrawAnimProperties(states[i], GetAnimationType((AnimationType)animationType.intValue));
				}
				else
				{
					SerializedObject serializedAnimState = new SerializedObject(states[i]);
					serializedAnimState.ApplyModifiedProperties();
				}
			}

			if ( arraySize == 0 )
			{
				m_Object.FindProperty(kArraySize).intValue++;
				animationStatesProp.GetArrayElementAtIndex(0).objectReferenceValue = statesGameObject.AddComponent<AIAnimationState>();
			}

			EditorGUILayout.Separator();

			m_Object.ApplyModifiedProperties();
		}


		private AnimationType GetAnimationType(AnimationType animationType)
		{
			if ( animationType == AnimationType.Auto )
			{
				bool isMecanim = transform.GetComponentInChildren<Animator>();

				if ( isMecanim )
				{
					return AnimationType.Mecanim;
				}
				else
				{
					return AnimationType.Legacy;
				}
			}

			return animationType;
		}


		public static void DrawAnimProperties(AIAnimationState animState, AnimationType animationType)
		{
			SerializedObject m_animState = new SerializedObject(animState);
			string speedTooltip = "";

			m_animState.Update();

			SerializedProperty m_animName = m_animState.FindProperty("name");
			SerializedProperty m_speed = m_animState.FindProperty("speed");
			SerializedProperty m_wrapMode = m_animState.FindProperty("animationWrapMode");

			EditorGUILayout.Separator();

			EditorGUILayout.PropertyField(m_animName);

			if ( animationType == AnimationType.TwoD )
			{
				SerializedProperty m_startFrame = m_animState.FindProperty("startFrame");
				SerializedProperty m_endFrame = m_animState.FindProperty("endFrame");
				EditorGUILayout.PropertyField(m_startFrame);
				EditorGUILayout.PropertyField(m_endFrame);

				if ( m_startFrame.intValue < 0 )
					m_startFrame.intValue = 0;

				if ( m_endFrame.intValue < 0 )
					m_endFrame.intValue = 0;

				speedTooltip = "This is a frames persecond value, IE: 30";
			}
			else
			{
				if ( animationType == AnimationType.Mecanim )
				{
					SerializedProperty useGoToState = m_animState.FindProperty("crossFadeIn");
					SerializedProperty exitWithGoToState = m_animState.FindProperty("crossFadeOut");

					EditorGUILayout.PropertyField(useGoToState);
					EditorGUILayout.PropertyField(exitWithGoToState);
				}

				SerializedProperty transitionTime = m_animState.FindProperty("transitionTime");
				EditorGUILayout.PropertyField(transitionTime);

				speedTooltip = "This is a normalized value\n\n0.5 = half speed\n1.0 = normal speed\n2.0 = double speed";
			}


			if ( animationType != AnimationType.Mecanim )
			{
				EditorGUILayout.PropertyField(m_speed, new GUIContent("Speed " + (animationType == AnimationType.TwoD ? "(FPS)" : ""), speedTooltip));
				EditorGUILayout.PropertyField(m_wrapMode);
			}

			EditorGUILayout.Separator();

			m_animState.ApplyModifiedProperties();
		}
	}
}
#endif