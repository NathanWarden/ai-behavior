using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

#if UNITY_EDITOR
using System.Reflection;
#endif

using Random = UnityEngine.Random;


namespace AIBehavior
{
	/// <summary>
	/// This class is the brain of the AIBehavior system. It controls state changes and various callbacks.
	/// </summary>
	[RequireComponent(typeof(AIAnimationStates))]
	public class AIBehaviors : AIComponent
	{
		/// <summary>
		/// Is this AI active (Read Only)?
		/// </summary>
		public bool isActive { get; private set; }

		/// <summary>
		/// Is this AI in defensive mode?
		/// </summary>
		public bool isDefending = false;

		/// <summary>
		/// This is a multiplier for the amount of damage this AI will receive
		/// </summary>
		[SerializeField] protected float damageMultiplier = 1.0f;

		/// <summary>
		/// The number of possible states this AI has.
		/// </summary>
		public int stateCount { get { return states.Length; } }

		public Transform aiTransform;
		Vector3 thisPos;
		NavMeshAgent navMeshAgent;

		/// <summary>
		/// The use sight transform.
		/// </summary>
		public bool useSightTransform = false;

		/// <summary>
		/// The sight transform.
		/// </summary>
		public Transform sightTransform = null;

		/// <summary>
		/// This is the state the AI is in once the game is playing.
		/// </summary>
		public BaseState initialState;

		/// <summary>
		/// This is the state the AI is currently in (Read Only).
		/// </summary>
		public BaseState currentState { get; private set; }

		/// <summary>
		/// Gets the previous state the AI was in
		/// </summary>
		public BaseState previousState { get; private set; }

		/// <summary>
		/// An array of all the states that belong to this AI.
		/// </summary>
		public BaseState[] states = new BaseState[0];

		/// <summary>
		/// The array of global triggers.
		/// </summary>
		public BaseTrigger[] triggers = new BaseTrigger[0];

		// === General Properties === //

		public TaggedObjectFinder objectFinder;

		/// <summary>
		/// These are the layers that block the AI from seeing what it's looking for
		/// </summary>
		/// IE. If this AI is an enemy looking for a player, you should EXCLUDE the player layer from these layers
		public LayerMask raycastLayers = -1;

		/// <summary>
		/// If this is true the AI is less likely to see toward the edge of it's FOV
		/// </summary>
		public bool useSightFalloff = true;

		/// <summary>
		/// The field of view in which the AI can see
		/// </summary>
		public float sightFOV = 180.0f;

		/// <summary>
		/// The maximum distance the AI can see
		/// </summary>
		public float sightDistance = 50.0f;

		/// <summary>
		/// The position that the AI looks from
		/// </summary>
		public Vector3 eyePosition = new Vector3(0.0f, 1.5f, 0.0f);

		/// <summary>
		/// Should the agent's local y axis always point up?
		/// </summary>
		public bool keepAgentUpright = true;

		/// <summary>
		/// The base health the AI has
		/// </summary>
		public float health = 100.0f;

		/// <summary>
		/// The maximum health the AI has
		/// </summary>
		public float maxHealth = 100.0f;

		/// <summary>
		/// The object where the AI states and triggers are located
		/// </summary>
		public GameObject statesGameObject = null;

		// === Animation Callback Info === //

		/// <summary>
		/// The component that will be called when a new animation should play
		/// </summary>
		public Component animationCallbackComponent = null;

		/// <summary>
		/// The name of the method that should be called when a new animation should play.
		/// </summary>
		public string animationCallbackMethodName = "";

		/// <summary>
		/// This is the list of available animation states for the AI
		/// </summary>
		public AIAnimationStates animationStates;

		// === Callbacks === //

		/// <summary>
		/// Any time a state changes, this delegate is called.
		/// </summary>
		public Action<BaseState, BaseState> onStateChanged = null;

		/// <summary>
		/// This delegate is called anytime an animation should play.
		/// </summary>
		public Action<AIAnimationState> onPlayAnimation = null;

		/// <summary>
		/// This delegate is called whenever a custom or 3rd party navigation system needs new location data.
		/// Its parameters are Vector3 destination, moveSpeed, rotationSpeed.
		/// </summary>
		public Action<Vector3, float, float> externalMove = null;

		// === Targetting and Rotation === //

		/// <summary>
		/// Gets the destination the AI is currently trying to reach
		/// </summary>
		public Vector3 currentDestination { get; private set; }
		protected Transform lastKnownRotationTarget;
		protected Vector3 targetRotationPoint;
		protected float rotationSpeed;

		public bool showDebugMessages = true;

		// AI Behaviors custom variables
		public AiBehaviorVariable[] userVariables = new AiBehaviorVariable[0];

		// === Methods === //

		public AIBehaviors()
		{
			objectFinder = CreateObjectFinder();
		}


		protected virtual TaggedObjectFinder CreateObjectFinder()
		{
			return new TaggedObjectFinder();
		}


		void Awake()
		{
			#if USE_ASTAR
			Debug.Log("Use Astar!");
			#endif

			aiTransform = GetTransform();
			animationStates = aiTransform.GetComponent<AIAnimationStates>();
			navMeshAgent = aiTransform.GetComponent<NavMeshAgent>();

			objectFinder.CacheTransforms(CachePoint.Awake);
			InitGlobalTriggers();

			if ( !useSightTransform || sightTransform == null )
			{
				sightTransform = new GameObject("Sight Transform").transform;
				sightTransform.parent = aiTransform;
				sightTransform.localRotation = Quaternion.identity;
				sightTransform.localPosition = eyePosition;
				useSightTransform = false;
			}

			currentDestination = aiTransform.position;

			SetActive(true);
		}


		void Start()
		{
			ChangeActiveState(initialState);
		}


		public void Update()
		{
			if ( isActive && Time.timeScale > 0 )
			{
				objectFinder.CacheTransforms(CachePoint.EveryFrame);

				if ( currentState.RotatesTowardTarget() )
				{
					RotateAgent();
				}

				thisPos = aiTransform.position;

				for ( int i = 0; i < triggers.Length; i++ )
				{
					if ( triggers[i].HandleEvaluate(this) && triggers[i].transitionState != null )
					{
						currentState = triggers[i].transitionState;
					}
				}

				// If the state remained the same, do the action
				if ( currentState.HandleReason(this) )
				{
					currentState.HandleAction(this);
				}
			}
		}


		Transform GetTransform()
		{
			if ( aiTransform == null )
			{
				aiTransform = transform;
			}

			return aiTransform;
		}


		void InitGlobalTriggers ()
		{
			for ( int i = 0; i < triggers.Length; i++ )
			{
				triggers[i].HandleInit(this, objectFinder);
			}
		}


		/// <summary>
		/// Sets whether this AI is actively evaluating its' behaviors
		/// </summary>
		public void SetActive(bool isActive)
		{
			this.isActive = isActive;
		}


		[System.Obsolete("Use Damage instead.")]
		public void GotHit(float damage)
		{
			Damage(damage);
		}


		/// <summary>
		/// Tells the AI that it got hit and it will subtract the amount of health passed from its' total health
		/// </summary>
		public void Damage(float damage)
		{
			// We don't want to enable this state if the FSM is dead
			GotHitState gotHitState = GetState<GotHitState>();

			if ( gotHitState != null && gotHitState.CanGetHit(this) )
			{
				float totalDamage = damage * GetDamageMultiplier ();
				SubtractHealthValue(totalDamage);
				Debug.Log ("Got " + totalDamage + " damage");

				if ( gotHitState.CoolDownFinished() )
				{
					ChangeActiveState(gotHitState);
				}
			}
		}


		public virtual void SetDamageMultiplier (float newDamageMultiplier)
		{
			damageMultiplier = newDamageMultiplier;
		}


		public virtual float GetDamageMultiplier ()
		{
			return damageMultiplier;
		}


		// === GetStates === //

		/// <summary>
		/// Returns all of the states that belong to this AI
		/// </summary>
		/// <returns>All of this AI's states.</returns>
		public BaseState[] GetAllStates()
		{
			return states;
		}


		/// <summary>
		/// Gets a state at a specific index.
		/// </summary>
		/// <returns>The state by index.</returns>
		/// <param name="stateIndex">State index.</param>
		public BaseState GetStateByIndex(int index)
		{
			if ( index < states.Length )
			{
				return states[index];
			}

			return null;
		}


		/// <summary>
		/// Gets a state according to it's user provided name. IE "Secondary Attack", "Idle" or "Seek Health"
		/// </summary>
		/// <returns>The state by name.</returns>
		/// <param name="stateName">State name.</param>
		public BaseState GetStateByName(string stateName)
		{
			foreach ( BaseState state in states )
			{
				if ( state.name.Equals(stateName) )
				{
					return state;
				}
			}

			return null;
		}


		/// <summary>
		/// Gets the first state of a certain Type. IE IdleState result = ai.GetState<IdleState>(); or BaseState result = ai.GetState<IdleState>();
		/// </summary>
		/// <returns>The state.</returns>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public T GetState<T> () where T : BaseState
		{
			foreach ( BaseState state in states )
			{
				if ( state is T )
				{
					return state as T;
				}
			}

			return null;
		}


		/// <summary>
		/// Gets the first state of a certain Type. IE IdleState result = ai.GetState(typeof(IdleState)) as IdleState; or BaseState result = ai.GetState(typeof(IdleState)) as BaseState;
		/// </summary>
		/// <returns>The state.</returns>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public BaseState GetState(System.Type type)
		{
			foreach ( BaseState state in states )
			{
				if ( state.GetType() == type )
				{
					return state;
				}
			}

			return null;
		}


		// === GetStates === //

		/// <summary>
		/// Gets all states of a certain Type. IE IdleState[] result = ai.GetStates<IdleState>();
		/// </summary>
		/// <returns>The states.</returns>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public T[] GetStates<T> () where T : BaseState
		{
			List<T> stateList = new List<T>();

			foreach ( BaseState state in states )
			{
				if ( state is T )
				{
					stateList.Add(state as T);
				}
			}

			return stateList.ToArray();
		}


		/// <summary>
		/// Gets all states of a certain Type. IE BaseState[] result = ai.GetStates(typeof(IdleState));
		/// </summary>
		/// <returns>The states.</returns>
		/// <param name="type">Type.</param>
		public BaseState[] GetStates(System.Type type)
		{
			List<BaseState> stateList = new List<BaseState>();

			foreach ( BaseState state in states )
			{
				if ( state.GetType() == type )
				{
					stateList.Add(state);
				}
			}

			return stateList.ToArray();
		}


		// === Replace States === //

		/// <summary>
		/// Replaces all states for this AI with the specified array.
		/// </summary>
		/// <param name="newStates">New states.</param>
		public void ReplaceAllStates(BaseState[] newStates)
		{
			states = newStates;
		}


		/// <summary>
		/// Replaces the state at the specified index
		/// </summary>
		/// <param name="newState">New state.</param>
		/// <param name="index">Index.</param>
		public void ReplaceStateAtIndex(BaseState newState, int index)
		{
			states[index] = newState;
		}


		// === Change Current Active State === //

		/// <summary>
		/// Force the AI to the specified state. Note that this state does NOT have to exist in the states array
		/// </summary>
		/// <param name="newState">New state.</param>
		/// The 'onStateChanged' delegate is called at the end of this method
		public void ChangeActiveState(BaseState newState)
		{
			if ( newState == null || !newState.CanSwitchToState())
			{
				return;
			}

			objectFinder.CacheTransforms(CachePoint.StateChanged);
			InitGlobalTriggers();

			previousState = currentState;
			previousState?.EndState(this);

			currentState = newState;
			newState.InitState(this);

			onStateChanged?.Invoke(newState, previousState);
		}


		/// <summary>
		/// Force the AI to change to the user named state. IE If an IdleState is named "Second Idle" you can force AIBehaviors to go to that state by calling ChangeActiveStateByName("Second Idle")
		/// </summary>
		/// <param name="stateName">State name.</param>
		public void ChangeActiveStateByName(string stateName)
		{
			foreach ( BaseState state in states )
			{
				if ( state.name.Equals(stateName) )
				{
					ChangeActiveState(state);
					return;
				}
			}
		}


		/// <summary>
		/// Force the AI to go to the state at the specified index
		/// </summary>
		/// <param name="index">Index.</param>
		public void ChangeActiveStateByIndex(int index)
		{
			if ( index < states.Length )
			{
				ChangeActiveState(states[index]);
			}
		}


		/// <summary>
		/// Sets the movement and rotation targets and values
		/// </summary>
		/// <param name="target">Target.</param>
		/// <param name="targetSpeed">Target speed.</param>
		/// <param name="rotationSpeed">Rotation speed.</param>
		public void MoveAgent(Transform target, float targetSpeed, float rotationSpeed)
		{
			if ( target != null )
			{
				RotateAgent(target, rotationSpeed);
			}

			MoveAgent(target.position, targetSpeed, rotationSpeed);
		}


		/// <summary>
		/// Sets the movement and rotation targets and values
		/// </summary>
		/// <param name="targetPoint">Target point.</param>
		/// <param name="targetSpeed">Target speed.</param>
		/// <param name="rotationSpeed">Rotation speed.</param>
		/// If there is a NavMeshAgent component its' speed, roation and destination will be handled automatically
		/// If the 'externalMove' delegate is assigned it will be called at the end of the method
		/// If both a NavMeshAgent component exists and the 'externalMove' delegate is assigned, both will be called
		public void MoveAgent(Vector3 targetPoint, float targetSpeed, float rotationSpeed, Transform rotationTargetOverride = null)
		{
			bool isNavAgent = navMeshAgent != null;

			currentDestination = targetPoint;
			targetRotationPoint = targetPoint;
			this.rotationSpeed = rotationSpeed;

			if ( isNavAgent && navMeshAgent.enabled )
			{
				Vector3 velocity = navMeshAgent.velocity;
				float velocityMagnitude = velocity.magnitude;
				float rotationMultiplier = Mathf.InverseLerp(0, targetSpeed, velocityMagnitude);

				navMeshAgent.speed = targetSpeed;
				navMeshAgent.angularSpeed = 0;
				navMeshAgent.destination = targetPoint;

				if ( rotationTargetOverride != null )
				{
					velocity = (rotationTargetOverride.position - aiTransform.position).normalized;
				}

				if ( keepAgentUpright )
				{
					velocity.y = 0.0f;
					velocity = velocity.normalized * velocityMagnitude;
				}

				aiTransform.Rotate(Vector3.Cross(aiTransform.forward, velocity) * Time.deltaTime * rotationSpeed * rotationMultiplier);
			}

			externalMove?.Invoke(targetPoint, targetSpeed, rotationSpeed);
		}


		/// <summary>
		/// Sets the rotation properties
		/// </summary>
		/// <param name="target">Target.</param>
		/// <param name="rotationSpeed">Rotation speed.</param>
		public void RotateAgent(Transform target, float rotationSpeed)
		{
			targetRotationPoint = target.position;
			lastKnownRotationTarget = target;
			this.rotationSpeed = rotationSpeed;
		}


		protected internal virtual void RotateAgent()
		{
			bool isNavAgent = navMeshAgent != null;
			bool updateRot = false;

			Vector3 pos = aiTransform.position;
			Quaternion curRotation = aiTransform.rotation;
			Vector3 sightPosition = GetSightPosition(sightTransform);
			Vector3 targetPos;

			if ( lastKnownRotationTarget != null )
			{
				RaycastHit hit;

				targetPos = lastKnownRotationTarget.position;

				if ( Physics.Linecast(sightPosition, targetPos, out hit, raycastLayers) && hit.transform != lastKnownRotationTarget )
				{
					if ( showDebugMessages )
					{
						Debug.LogWarning("Can't rotate toward target. The object '" + hit.transform.name + "' is in the way of '" + lastKnownRotationTarget.name + "'.");
					}

					lastKnownRotationTarget = null;
				}
				else
				{
					targetRotationPoint = targetPos;
				}
			}

			targetRotationPoint.y = pos.y;
			aiTransform.LookAt(targetRotationPoint);

			if ( isNavAgent )
			{
				updateRot = navMeshAgent.updateRotation;
				navMeshAgent.updateRotation = false;
			}

			if ( rotationSpeed > 0.0f && Time.deltaTime > 0.0f )
			{
				//thisTFM.rotation = Quaternion.RotateTowards(curRotation, thisTFM.rotation, rotationSpeed * Time.deltaTime);
			}

			if ( isNavAgent )
			{
				navMeshAgent.updateRotation = updateRot;
			}
		}


		/// <summary>
		/// Tells the system to play an AIAnimationState
		/// </summary>
		/// <param name="animState">Animation state.</param>
		/// If the 'onPlayAnimation' delegate is defined it will call that.\n
		/// If 'onPlayAnimation' is not defined it will attempt to call the method 'animationCallbackMethodName' defined on the 'animationCallbackComponent'
		public void PlayAnimation(AIAnimationState animState)
		{
			if ( onPlayAnimation != null )
			{
				onPlayAnimation?.Invoke(animState);
			}
			else if ( animationCallbackComponent != null && animState != null )
			{
				if ( !string.IsNullOrEmpty(animationCallbackMethodName) )
				{
					animationCallbackComponent.SendMessage(animationCallbackMethodName, animState);
				}
			}
		}


		/// <summary>
		/// Gets the health value.
		/// </summary>
		/// <returns>The health value.</returns>
		public float GetHealthValue()
		{
			return health;
		}


		/// <summary>
		/// Sets the health value.
		/// </summary>
		/// <param name="healthAmount">Health amount.</param>
		public void SetHealthValue(float healthAmount)
		{
			health = healthAmount;
		}


		/// <summary>
		/// Adds to the health value.
		/// </summary>
		/// <param name="healthAmount">Health amount.</param>
		public void AddHealthValue(float healthAmount)
		{
			health += healthAmount;
		}


		/// <summary>
		/// Subtracts from the health value.
		/// </summary>
		/// <param name="healthAmount">Health amount.</param>
		public void SubtractHealthValue(float healthAmount)
		{
			health -= healthAmount;
		}


		/// <summary>
		/// Gets the closest player found that is tagged with a tag in the playerTags array.
		/// </summary>
		/// <returns>The closest player.</returns>
		/// Note that if the tagged GameObject was added after this AI was initialized it may not find it
		/// - One solution is to call the CheckForPlayers method to refresh this list
		/// - Another solution is to set the checkForPlayersInterval to something other than 0
		/// - Idealy you want to call CheckForPlayers as little as possible as it has some performance implications.
		public Transform GetClosestPlayer(Transform[] playerTransforms)
		{
			float sqrDist;
			return GetClosestPlayer(playerTransforms, out sqrDist);
		}


		/// <summary>
		/// This method is the same as the main GetClosestPlayer method, only that it allows you to get the squareDistance to the closest player in the out parameter
		/// </summary>
		/// <returns>The closest player.</returns>
		/// <param name="squareDistance">Square distance.</param>
		public Transform GetClosestPlayer(Transform[] playerTransforms, out float squareDistance)
		{
			int closestPlayerIndex = -1;

			squareDistance = Mathf.Infinity;

			for ( int i = 0; i < playerTransforms.Length; i++ )
			{
				Vector3 playerPosition = playerTransforms[i].position;
				Vector3 targetDifference = playerPosition - thisPos;

				// is the target within distance?
				if ( targetDifference.sqrMagnitude < squareDistance )
				{
					squareDistance = targetDifference.sqrMagnitude;
					closestPlayerIndex = i;
				}
			}

			if ( closestPlayerIndex == -1 )
				return null;
			else
				return playerTransforms[closestPlayerIndex];
		}


		/// <summary>
		/// This returns the closest player within the AI's sight. Sight falloff is NOT used.
		/// </summary>
		/// <returns>The closest player within sight.</returns>
		public Transform GetClosestPlayerWithinSight(Transform[] playerTransforms)
		{
			return GetClosestPlayerWithinSight(playerTransforms, false);
		}


		/// <summary>
		/// This returns the closest player within the AI's sight. Sight falloff can be specified.
		/// </summary>
		/// <returns>The closest player within sight.</returns>
		/// <param name="includeSightFalloff">If set to <c>true</c> include sight falloff.</param>
		public Transform GetClosestPlayerWithinSight(Transform[] playerTransforms, bool includeSightFalloff, Transform sightTransformOverride = null)
		{
			float dist = 0.0f;
			return GetClosestPlayerWithinSight(playerTransforms, out dist, includeSightFalloff, sightTransformOverride);
		}


		/// <summary>
		/// Gets the closest player within sight. Sight falloff can be specified and the squareDistance can be retrieved with the out parameter.
		/// </summary>
		/// <returns>The closest player within sight.</returns>
		/// <param name="squareDistance">Square distance.</param>
		/// <param name="includeSightFalloff">If set to <c>true</c> include sight falloff.</param>
		// TODO: Refactor this method to not use random values.
		public Transform GetClosestPlayerWithinSight(Transform[] playerTransforms, out float squareDistance, bool includeSightFalloff, Transform sightTransformOverride = null)
		{
			float sqrSightDistance = sightDistance * sightDistance;
			int closestPlayerIndex = -1;

			squareDistance = Mathf.Infinity;

			if ( sightTransformOverride == null )
			{
				sightTransformOverride = sightTransform;
			}

			for ( int i = 0; i < playerTransforms.Length; i++ )
			{
				Vector3 playerPosition = playerTransforms[i].position;
				Vector3 targetDifference = playerPosition - thisPos;
				float targetSqrMagnitude = targetDifference.sqrMagnitude;
				float angle = Vector3.Angle(targetDifference, sightTransformOverride.forward);
				Vector3 sightPosition = GetSightPosition(sightTransformOverride);

				// If we already have a player in sight
				if ( closestPlayerIndex != -1 )
				{
					// Is the new one closer than the previous closest one?
					if ( targetSqrMagnitude > squareDistance )
					{
						continue;
					}
				}

				// is the target within distance?
				if ( targetSqrMagnitude < sqrSightDistance )
				{
					float halfFOV = sightFOV / 2.0f;

					if ( angle < halfFOV )
					{
						if ( useSightFalloff && includeSightFalloff )
						{
							float anglePercentage = Mathf.InverseLerp(0.0f, halfFOV, angle);

							if ( Random.value < anglePercentage )
							{
								continue;
							}
						}

						RaycastHit hit;

						// Make sure there isn't anything between the player
						if ( !Physics.Linecast(sightPosition, playerPosition, out hit, raycastLayers) || hit.transform == playerTransforms[i] || hit.transform == aiTransform )
						{
							squareDistance = targetSqrMagnitude;
							closestPlayerIndex = i;
						}
					}
				}
			}

			if ( closestPlayerIndex == -1 )
				return null;
			else
				return playerTransforms[closestPlayerIndex];
		}


		public Vector3 GetSightPosition (Transform sightTransformOverride = null)
		{
			if (sightTransformOverride == null)
			{
				if (sightTransform == null)
				{
					return GetTransform().TransformPoint(eyePosition);
				}
				else
				{
					sightTransformOverride = sightTransform;
				}
			}

			return sightTransformOverride.position;
		}


		public Vector3 GetSightDirection (Transform sightTransformOverride = null)
		{
			if (sightTransformOverride == null)
			{
				if (sightTransform == null)
				{
					return GetTransform().forward;
				}
				else
				{
					sightTransformOverride = sightTransform;
				}
			}

			return sightTransformOverride.forward;
		}


		/// <summary>
		/// Tells the AI to play the audio clip contained within the current active state.
		/// </summary>
		public void PlayAudio()
		{
			AudioSource audioSource;

			if ( currentState.sounds.Length == 0 )
			{
				return;
			}

			audioSource = GetComponent<AudioSource>();

			if ( audioSource == null )
			{
				audioSource = gameObject.AddComponent<AudioSource>();
			}

			int index = Random.Range (0, currentState.sounds.Length);
			audioSource.loop = currentState.sounds[index].loopAudio;
			audioSource.volume = currentState.sounds[index].audioVolume;
			audioSource.pitch = currentState.sounds[index].audioPitch + ((Random.value * currentState.sounds[index].audioPitchRandomness) - (currentState.sounds[index].audioPitchRandomness / 2.0f)) * 2.0f;
			audioSource.clip = currentState.sounds[index].audioClip;
			audioSource.Play();
		}

		public string[] GetVariableNames()
		{
			List<string> varNames = new List<string>();
			for(int i = 0; i < userVariables.Length; i++)
			{
				varNames.Add(userVariables[i].name);
			}
			return varNames.ToArray();
		}
	}
}