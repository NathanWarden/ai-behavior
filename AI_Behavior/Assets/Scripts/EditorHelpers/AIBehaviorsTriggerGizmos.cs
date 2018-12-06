#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using AIBehavior;


namespace AIBehaviorEditor
{
	public class AIBehaviorsTriggerGizmos
	{
		public static void DrawGizmos(AIBehaviors fsm, int selectedState)
		{
			DrawGizmos(fsm, fsm.GetStateByIndex(selectedState).triggers);

			if ( GUI.changed )
			{
				EditorUtility.SetDirty(fsm.GetStateByIndex(selectedState));
			}
		}


		private static void DrawGizmos(AIBehaviors fsm, BaseTrigger[] triggers)
		{
			for ( int i = 0; i < triggers.Length; i++ )
			{
				Undo.RecordObject(triggers[i], "Changes to Trigger");

				triggers[i].DrawGizmos(fsm);

				DrawGizmos(fsm, triggers[i].subTriggers);
			}
		}


		static Color handlesColor = Color.yellow;

		public static void DrawVisionCone(Vector3 position, Vector3 direction, float sightDistance, float fov)
		{
			Color oldColor = Handles.color;
			float fovRadians = Mathf.Deg2Rad * fov;
			float radius = sightDistance * Mathf.Sin(fovRadians);
			Vector3 discPosition = position + direction * sightDistance;
			Vector3 relativeZPosition = position + direction * sightDistance;
			Vector3 sideVector = Vector3.Cross(direction, Vector3.up);

			Handles.color = handlesColor;

			Handles.DrawWireDisc(discPosition, direction, radius);

			Handles.DrawLine(position, position + direction * sightDistance);
			Handles.DrawLine(position, relativeZPosition + Vector3.up * radius);
			Handles.DrawLine(position, relativeZPosition + Vector3.down * radius);
			Handles.DrawLine(position, relativeZPosition + sideVector * radius);
			Handles.DrawLine(position, relativeZPosition + -sideVector * radius);


			Handles.color = oldColor;
		}
	}
}
#endif