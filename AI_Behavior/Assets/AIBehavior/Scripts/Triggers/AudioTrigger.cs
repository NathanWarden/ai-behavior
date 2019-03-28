using UnityEngine;
using AIBehavior;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
using AIBehaviorEditor;
#endif

namespace AIBehavior
{
	public class AudioTrigger : BaseTrigger
	{
		public CachePoint audioCachePoint = CachePoint.StateChanged;
		public float volumeTriggerLevel = 0.01f;
		public bool setAudioTransformAsSeekTarget = false;
		public SeekState seekState = null;
		AudioSource[] audioSources = new AudioSource[0];


		protected override void Awake ()
		{
			base.Awake ();

			if ( audioCachePoint == CachePoint.Awake )
			{
				UpdateAudioSources();
			}
		}


		protected override void Init (AIBehaviors fsm)
		{
			if ( audioCachePoint == CachePoint.StateChanged )
			{
				UpdateAudioSources();
			}
		}


		protected override bool Evaluate(AIBehaviors fsm)
		{
			AudioSource targetAudioSource = null;
			float nearestAudioSource = Mathf.Infinity;

			if ( audioCachePoint == CachePoint.EveryFrame )
			{
				UpdateAudioSources();
			}

			for ( int i = 0; i < audioSources.Length; i++ )
			{
				AudioSource audioSource = audioSources[i];

				if ( audioSource.isPlaying && audioSource.clip != null )
				{
					float audioSourceSqrDistance = Vector3.SqrMagnitude(audioSource.transform.position - fsm.aiTransform.position);

					if ( audioSourceSqrDistance < nearestAudioSource )
					{
						if ( CheckAudioSource(audioSource, audioSourceSqrDistance) )
						{
							nearestAudioSource = audioSourceSqrDistance;
							targetAudioSource = audioSource;

							if ( setAudioTransformAsSeekTarget )
							{
								seekState.specifySeekTarget = true;
								seekState.seekTarget = audioSource.transform;
							}
						}
					}
				}
			}

			return targetAudioSource != null;
		}


		void UpdateAudioSources ()
		{
			Transform[] targetTransforms = objectFinder.GetTransforms();
			List<AudioSource> audioSourcesList = new List<AudioSource>();

			for ( int i = 0; i < targetTransforms.Length; i++ )
			{
				AudioSource[] newSources = targetTransforms[i].GetComponentsInChildren<AudioSource>(true);
				audioSourcesList.AddRange(newSources);
			}

			audioSources = audioSourcesList.ToArray();
		}


		bool CheckAudioSource (AudioSource audioSource, float sqrDistance)
		{
			float[] samples = new float[2];
			float distance = -1;
			AudioClip clip = audioSource.clip;

			if ( clip.samples >= audioSource.timeSamples + samples.Length )
			{
				clip.GetData(samples, audioSource.timeSamples);

				for ( int i = 0; i < samples.Length; i++ )
				{
					float sample = Mathf.Abs(samples[i]);

					// There's no point in doing complex calculations unless the volume level is already high enough
					if ( sample > volumeTriggerLevel )
					{
						if ( distance < 0 )
						{
							distance = Mathf.Sqrt(sqrDistance);
						}

						if ( CheckSample(sample, distance, audioSource) )
						{
							return true;
						}
					}
				}
			}

			return false;
		}


		protected virtual bool CheckSample(float sample, float distance, AudioSource audioSource)
		{
			float volumeMultiplier;

			if ( audioSource.rolloffMode == AudioRolloffMode.Linear )
			{
				volumeMultiplier = Mathf.InverseLerp(audioSource.minDistance, audioSource.maxDistance, distance);
				volumeMultiplier = 1.0f - volumeMultiplier;
			}
			else
			{
				float denominator = distance - 1.0f;

				if ( denominator > 1.0f )
				{
					volumeMultiplier = 1.0f/denominator;
				}
				else
				{
					volumeMultiplier = 1.0f;
				}
			}

			return (volumeMultiplier * sample) > volumeTriggerLevel;
		}
		
		
		public override string DefaultDisplayName()
		{
			return "Audio Trigger";
		}


#if UNITY_EDITOR
		public override void DrawInspectorProperties(AIBehaviors fsm, SerializedObject triggerObject)
		{
			SerializedProperty property;

			GUILayout.Label("Properties: ", EditorStyles.boldLabel);
			GUILayout.BeginVertical(GUI.skin.box);

			property = triggerObject.FindProperty("audioCachePoint");
			EditorGUILayout.PropertyField(property);

			property = triggerObject.FindProperty("volumeTriggerLevel");
			EditorGUILayout.PropertyField(property);

			property = triggerObject.FindProperty("setAudioTransformAsSeekTarget");
			EditorGUILayout.PropertyField(property);

			if ( property.boolValue )
			{
				property = triggerObject.FindProperty("seekState");
				property.objectReferenceValue = AIBehaviorsStatePopups.DrawEnabledStatePopup<SeekState>(fsm, property.objectReferenceValue as SeekState);
			}

			GUILayout.EndVertical();
		}
	#endif
	}
}