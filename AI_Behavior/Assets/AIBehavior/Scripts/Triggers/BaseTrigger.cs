using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
using AIBehaviorEditor;
using System.Reflection;
#endif


namespace AIBehavior
{
	public abstract class BaseTrigger : AIComponent
	{
		public BaseState transitionState;
		public BaseTrigger parentTrigger = null;
		public BaseTrigger[] subTriggers = new BaseTrigger[0];
		public bool ownsObjectFinder = false;
		public TaggedObjectFinder objectFinder;
		public bool invertResult = false;

		// === Trigger Methods === //

		public BaseTrigger()
		{
			objectFinder = CreateObjectFinder();
		}
		
		
		protected virtual TaggedObjectFinder CreateObjectFinder()
		{
			return new TaggedObjectFinder();
		}


		protected virtual void Init(AIBehaviors fsm) {}
		protected abstract bool Evaluate(AIBehaviors fsm);


		protected virtual void Awake()
		{
			objectFinder.CacheTransforms(CachePoint.Awake);

			for ( int i = 0; i < subTriggers.Length; i++ )
			{
				subTriggers[i].parentTrigger = this;
			}
		}


		public void HandleInit(AIBehaviors fsm, TaggedObjectFinder parentObjectFinder)
		{
			if ( !objectFinder.useCustomTags )
			{
				objectFinder = parentObjectFinder;
			}
			else
			{
				ownsObjectFinder = true;
			}

			objectFinder.CacheTransforms(CachePoint.StateChanged);

			Init(fsm);

			foreach ( BaseTrigger subTrigger in subTriggers )
			{
				subTrigger.HandleInit(fsm, objectFinder);
			}
		}


		public bool HandleEvaluate(AIBehaviors fsm)
		{
			bool invert = CanInvertResult() && invertResult;
			bool triggerResult = this.enabled && (invert ? !Evaluate(fsm) : Evaluate(fsm));
			bool result = triggerResult && CheckSubTriggers(fsm);

			objectFinder.CacheTransforms(CachePoint.EveryFrame);

			if ( result )
			{
				OnTriggered();
				ChangeToTransitionState(fsm);
			}

			return result;
		}


		public virtual bool CanInvertResult()
		{
			return true;
		}


		protected virtual void ChangeToTransitionState(AIBehaviors fsm)
		{
			if (transitionState != null && transitionState.CanSwitchToState()) 
			{
				fsm.ChangeActiveState(transitionState);
			}
		}


		private bool CheckSubTriggers (AIBehaviors fsm)
		{
			if ( subTriggers.Length == 0 )
			{
				return true;
			}

			foreach ( BaseTrigger trigger in subTriggers )
			{
				if ( trigger.HandleEvaluate(fsm) )
				{
					return true;
				}
			}

			return false;
		}


		public void AddSubTrigger(BaseTrigger subTrigger)
		{
			BaseTrigger[] newSubTriggers = new BaseTrigger[subTriggers.Length+1];

			for ( int i = 0; i < subTriggers.Length; i++ )
			{
				newSubTriggers[i] = subTriggers[i];
			}

			subTrigger.parentTrigger = this;
			newSubTriggers[subTriggers.Length] = subTrigger;

			subTriggers = newSubTriggers;
		}


		protected virtual void OnTriggered() {}


#if UNITY_EDITOR
		public void DrawInspectorGUI(AIBehaviors fsm)
		{
			SerializedObject triggerObject = new SerializedObject(this);
			SerializedProperty property = triggerObject.FindProperty("invertResult");

			DrawInspectorProperties(fsm, triggerObject);

			if ( CanInvertResult() )
			{
				EditorGUILayout.Separator();
				EditorGUILayout.PropertyField(property);
			}

			EditorGUILayout.Separator();
			objectFinder.DrawPlayerTagsSelection(fsm, triggerObject, "objectFinder", false);

			triggerObject.ApplyModifiedProperties();
		}


		public void DrawTransitionState(AIBehaviors fsm)
		{
			SerializedObject sObject = new SerializedObject(this);
			SerializedProperty property;

			GUILayout.BeginHorizontal();
			GUILayout.Label("Change to State:");
			property = sObject.FindProperty("transitionState");
			property.objectReferenceValue = AIBehaviorsStatePopups.DrawEnabledStatePopup(fsm, property.objectReferenceValue as BaseState);
			GUILayout.EndHorizontal();

			sObject.ApplyModifiedProperties();
		}
		
		
		public virtual void DrawInspectorProperties(AIBehaviors fsm, SerializedObject triggerObject)
		{
			InspectorHelper.DrawInspector(triggerObject);
		}
		

		public virtual void DrawGizmos(AIBehaviors fsm) {}
#endif
	}
}