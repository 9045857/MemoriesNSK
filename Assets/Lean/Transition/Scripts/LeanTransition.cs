﻿using UnityEngine;
using Lean.Common;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Lean.Transition
{
	/// <summary>This component updates all active transition methods, both in game, and in the editor.</summary>
	[ExecuteInEditMode]
	[HelpURL(HelpUrlPrefix + "LeanTransition")]
	[AddComponentMenu(ComponentMenuPrefix + "Lean Transition")]
	public class LeanTransition : MonoBehaviour
	{
		public const string ComponentMenuPrefix = "Lean/Transition/";

		public const string MethodsMenuPrefix = "Lean/Transition/Methods/";

		public const string MethodsMenuSuffix = " Transition";

		public const string HelpUrlPrefix = "http://carloswilkes.github.io/Documentation/LeanTransition#";

		/// <summary>This allows you to set where in the game loop animations are updated when timing = LeanTime.Default.</summary>
		public LeanTime Timing = LeanTime.UnscaledUpdate;

		public static List<LeanTransition> Instances = new List<LeanTransition>();

		private static List<LeanState> unscaledUpdateStates = new List<LeanState>();

		private static List<LeanState> unscaledLateUpdateStates = new List<LeanState>();

		private static List<LeanState> unscaledFixedUpdateStates = new List<LeanState>();

		private static List<LeanState> updateStates = new List<LeanState>();

		private static List<LeanState> lateUpdateStates = new List<LeanState>();

		private static List<LeanState> fixedUpdateStates = new List<LeanState>();

		private static List<LeanMethod> tempBaseMethods = new List<LeanMethod>();

		private static List<LeanMethod> baseMethodStack = new List<LeanMethod>();

		private static Dictionary<string, System.Type> aliasTypePairs = new Dictionary<string, System.Type>();

		private static LeanState currentHead;

		private static LeanState currentQueue;

		private static LeanTime currentTiming;

		private static float currentSpeed = 1.0f;

		private static bool started;

		private static Dictionary<string, Object> currentAliases = new Dictionary<string, Object>();

		/// <summary>This tells you where in the game loop new transitions will be registered to.</summary>
		public static LeanTime CurrentUpdateMode
		{
			get
			{
				if (Instances.Count > 0)
				{
					return Instances[0].Timing;
				}

				return default(LeanTime);
			}
		}

		/// <summary>This tells you how many transitions are currently running.</summary>
		public static int Count
		{
			get
			{
				return unscaledUpdateStates.Count + unscaledLateUpdateStates.Count + unscaledFixedUpdateStates.Count + updateStates.Count + lateUpdateStates.Count + fixedUpdateStates.Count;
			}
		}

		/// <summary>After a transition state is registered, it will be stored here. This allows you to copy it out for later use.</summary>
		public static LeanState CurrentHead
		{
			get
			{
				return currentHead;
			}
		}

		/// <summary>If you want the next registered transition state to automatically begin after an existing transition state, then specify it here.</summary>
		public static LeanState CurrentQueue
		{
			set
			{
				currentQueue = value;
			}
		}

		/// <summary>This allows you to change where in the game loop all future transitions in the current animation will be updated.</summary>
		public static LeanTime CurrentUpdate
		{
			set
			{
				currentTiming = value;
			}
		}

		/// <summary>This allows you to change the transition speed multiplier of all future transitions in the current animation.</summary>
		public static float CurrentSpeed
		{
			set
			{
				currentSpeed = value;
			}

			get
			{
				return currentSpeed;
			}
		}

		/// <summary>This allows you to change the alias name to UnityEngine.Object association of all future transitions in the current animation.</summary>
		public static Dictionary<string, Object> CurrentAliases
		{
			get
			{
				return currentAliases;
			}
		}

		/// <summary>This method will return the specified timing, unless it's set to <b>Default</b>, then it will return <b>UnscaledTime</b>.</summary>
		public static LeanTime GetTiming(LeanTime current = LeanTime.Default)
		{
			if (current == LeanTime.Default)
			{
				current = LeanTime.UnscaledUpdate;
			}

			return current;
		}

		/// <summary>This method works like <b>GetTiming</b>, but it won't return any unscaled times.</summary>
		public static LeanTime GetTimingAbs(LeanTime current)
		{
			return (LeanTime)System.Math.Abs((int)current);
		}

		/// <summary>If you failed to submit a previous transition then this will throw an error, and then submit them.</summary>
		public static void RequireSubmitted()
		{
			if (currentQueue != null)
			{
				Debug.LogError("You forgot to submit the last transition! " + currentQueue.GetType() + " - " + currentQueue.GetTarget());

				Submit();
			}

			if (baseMethodStack.Count > 0)
			{
				Debug.LogError("Failed to submit all methods.");

				Submit();
			}
		}

		/// <summary>This will reset any previously called TimeTransition calls.</summary>
		public static void ResetTiming()
		{
			currentTiming = LeanTime.Default;
		}

		/// <summary>This will submit any previously registered transitions, and reset the timing.</summary>
		public static void Submit()
		{
			currentTiming = LeanTime.Default;
			currentQueue  = null;
			currentSpeed  = 1.0f;

			baseMethodStack.Clear();
		}

		public static void BeginTransitions(List<Transform> roots, float speed = 1.0f)
		{
			currentSpeed = 1.0f;

			if (roots != null)
			{
				RequireSubmitted();

				for (var i = 0; i < roots.Count; i++)
				{
					InsertTransitions(roots[i], speed);

					Submit();
				}
			}
		}

		/// <summary>This will begin all transitions on the specified GameObject, all its children, and then submit them.
		/// If you failed to submit a previous transitions then this will also throw an error.</summary>
		public static void BeginAllTransitions(Transform root, float speed = 1.0f)
		{
			currentSpeed = 1.0f;

			if (root != null)
			{
				RequireSubmitted();

				InsertTransitions(root, speed);

				Submit();
			}
		}

		/// <summary>This will begin all transitions on the specified GameObject, and all its children.</summary>
		public static void InsertTransitions(UnityEngine.GameObject root, float speed = 1.0f)
		{
			if (root != null)
			{
				InsertTransitions(root.transform, speed);
			}
		}

		/// <summary>This will begin all transitions on the specified Transform, and all its children.</summary>
		public static void InsertTransitions(Transform root, float speed = 1.0f)
		{
			if (root != null)
			{
				var spd = currentSpeed;
				var min = baseMethodStack.Count; root.GetComponents(tempBaseMethods); baseMethodStack.AddRange(tempBaseMethods); tempBaseMethods.Clear();
				var max = baseMethodStack.Count;

				currentSpeed *= speed;

				for (var i = min; i < max; i++)
				{
					baseMethodStack[i].Register();
				}

				baseMethodStack.RemoveRange(min, max - min);

				var head = currentHead;

				for (var i = 0; i < root.childCount; i++)
				{
					currentQueue = head;

					InsertTransitions(root.GetChild(i));
				}

				currentSpeed = spd;
			}
		}

		/// <summary>This method returns all TargetAliases on all transitions on the specified Transforms.</summary>
		public static Dictionary<string,System.Type> FindAllAliasTypePairs(List<Transform> roots)
		{
			aliasTypePairs.Clear();

			if (roots != null)
			{
				for (var i = 0; i < roots.Count; i++)
				{
					AddAliasTypePairs(roots[i]);
				}
			}

			return aliasTypePairs;
		}

		/// <summary>This method returns all TargetAliases on all transitions on the specified Transform.</summary>
		public static Dictionary<string,System.Type> FindAllAliasTypePairs(Transform root)
		{
			aliasTypePairs.Clear();

			AddAliasTypePairs(root);

			return aliasTypePairs;
		}

		private static void AddAliasTypePairs(Transform root)
		{
			if (root != null)
			{
				root.GetComponents(tempBaseMethods);

				for (var i = 0; i < tempBaseMethods.Count; i++)
				{
					var baseMethod = tempBaseMethods[i] as LeanMethodWithStateAndTarget;

					if (baseMethod != null)
					{
						var targetType = baseMethod.GetTargetType();
						var alias      = baseMethod.Alias;

						if (string.IsNullOrEmpty(alias) == false)
						{
							var existingType = default(System.Type);

							// Exists?
							if (aliasTypePairs.TryGetValue(alias, out existingType) == true)
							{
								// Clashing types?
								if (existingType != targetType)
								{
									// If both are components then the clash can be resolved by using GameObject
									if (targetType.IsSubclassOf(typeof(Component)) == true)
									{
										// If it's already a GameObject, skip
										if (existingType == typeof(UnityEngine.GameObject))
										{
											continue;
										}
										// Change existing type to GameObject?
										else if (existingType.IsSubclassOf(typeof(Component)) == true)
										{
                                            aliasTypePairs[alias] = typeof(UnityEngine.GameObject);

											continue;
										}
									}

									// If the clash cannot be resolved, throw an error
									Debug.LogError("The (" + root.name + ") GameObject contains multiple transitions that define a target alias of (" + alias + "), but these transitions use different types (" + existingType + ") + (" + targetType + "). You must give them different aliases.", root);
								}
							}
							// Add new?
							else
							{
								aliasTypePairs.Add(alias, targetType);
							}
						}
					}
				}
			}
		}

		public static T RegisterWithTarget<T, U>(Stack<T> pool, float duration, U target)
				where T : LeanStateWithTarget<U>, new()
				where U : Object
		{
			var data = Register(pool, duration);

			data.Target = target;

			return data;
		}

		public static T Register<T>(Stack<T> pool, float duration)
				where T : LeanState, new()
		{
			var state       = pool.Count > 0 ? pool.Pop() : new T();
			var finalUpdate = GetTiming(currentTiming);

			// Setup initial data
			state.Age      = -1.0f;
			state.Duration = duration;
			state.Skip     = false;

			if (currentSpeed > 0.0f)
			{
				state.Duration /= currentSpeed;
			}

			state.Prev.Clear();
			state.Next.Clear();

			if (currentQueue != null)
			{
				state.BeginAfter(currentQueue);
			}

			currentQueue = null;
			currentHead  = state;

			// Register data with the correct list
			switch (finalUpdate)
			{
				case LeanTime.UnscaledFixedUpdate: unscaledFixedUpdateStates.Add(state); break;
				case LeanTime.UnscaledLateUpdate:   unscaledLateUpdateStates.Add(state); break;
				case LeanTime.UnscaledUpdate:           unscaledUpdateStates.Add(state); break;
				case LeanTime.Update:                           updateStates.Add(state); break;
				case LeanTime.LateUpdate:                   lateUpdateStates.Add(state); break;
				case LeanTime.FixedUpdate:                 fixedUpdateStates.Add(state); break;
			}

			// Make sure the transition manager exists
			if (Instances.Count == 0)
			{
				new UnityEngine.GameObject("LeanTransition").AddComponent<LeanTransition>();
			}

			return state;
		}

		protected virtual void OnEnable()
		{
			Instances.Add(this);
#if UNITY_EDITOR
			EditorApplication.delayCall -= DelayCall;
			EditorApplication.delayCall += DelayCall;
#endif
		}

		protected virtual void OnDisable()
		{
			Instances.Remove(this);
#if UNITY_EDITOR
			EditorApplication.delayCall -= DelayCall;
#endif
			if (Instances.Count == 0)
			{
				unscaledFixedUpdateStates.Clear();
				 unscaledLateUpdateStates.Clear();
				     unscaledUpdateStates.Clear();
				             updateStates.Clear();
				         lateUpdateStates.Clear();
				        fixedUpdateStates.Clear();
			}
		}

#if UNITY_EDITOR
		private void DelayCall()
		{
			EditorApplication.delayCall -= DelayCall;
			EditorApplication.delayCall += DelayCall;

			var delta = Time.deltaTime;

			if (Application.isPlaying == false)
			{
				UpdateAll(unscaledFixedUpdateStates, delta);
				UpdateAll( unscaledLateUpdateStates, delta);
				UpdateAll(     unscaledUpdateStates, delta);
				UpdateAll(             updateStates, delta);
				UpdateAll(         lateUpdateStates, delta);
				UpdateAll(        fixedUpdateStates, delta);
			}
		}
#endif
		protected virtual void Update()
		{
			if (this == Instances[0] && Application.isPlaying == true && started == true)
			{
				UpdateAll(unscaledUpdateStates, Time.unscaledDeltaTime);
				UpdateAll(        updateStates, Time.deltaTime        );
			}
		}

		protected virtual void LateUpdate()
		{
			if (this == Instances[0] && Application.isPlaying == true)
			{
				if (started == true)
				{
					UpdateAll(unscaledLateUpdateStates, Time.unscaledDeltaTime);
					UpdateAll(        lateUpdateStates, Time.deltaTime        );
				}
				else
				{
					started = true;
				}
			}
		}

		protected virtual void FixedUpdate()
		{
			if (this == Instances[0] && Application.isPlaying == true && started == true)
			{
				UpdateAll(unscaledFixedUpdateStates, Time.fixedUnscaledDeltaTime);
				UpdateAll(        fixedUpdateStates, Time.fixedDeltaTime        );
			}
		}

		/// <summary>This method will mark all tranisitions as Conflict = true if they match the transition type and target object of the specified transition.</summary>
		private void RemoveConflictsBefore(List<LeanState> states, LeanState currentState, int currentIndex)
		{
			var currentConflict = currentState.Conflict;

			if (currentConflict != LeanState.ConflictType.None)
			{
				var currentType   = currentState.GetType();
				var currentTarget = currentState.GetTarget();

				for (var i = 0; i < currentIndex; i++)
				{
					var transition = states[i];

					if (transition.Skip == false && transition.GetType() == currentType && transition.GetTarget() == currentTarget)
					{
						transition.Skip = true;

						if (currentConflict == LeanState.ConflictType.Complete)
						{
							transition.Update(1.0f);
						}
					}
				}
			}
		}

		private void UpdateAll(List<LeanState> states, float delta)
		{
			currentHead = null;

			for (var i = states.Count - 1; i >= 0; i--)
			{
				var state = states[i];

				// Only update if the previous transitions have finished
				if (state.Prev.Count == 0)
				{
					// If the transition age is negative, it hasn't started yet
					if (state.Age < 0.0f)
					{
						state.Age = 0.0f;

						// If this newly beginning transition is identical to an already registered one, mark the existing one as conflicting so it doesn't get updated
						RemoveConflictsBefore(states, state, i);

						// Begin the transition (this will often copy the current state of the variable that is being transitioned)
						state.Begin();
					}

					// Age
					state.Age += delta;

					// Finished?
					if (state.Age >= state.Duration)
					{
						// Activate all chained states and clear them
						for (var j = state.Next.Count - 1; j >= 0; j--)
						{
							state.Next[j].Prev.Remove(state);
						}

						state.Next.Clear();

						// Make sure we call update one final time with a progress value of exactly 1.0
						if (state.Skip == false)
						{
							state.Update(1.0f);
						}
#if UNITY_EDITOR
						DirtyTarget(state);
#endif
						state.Despawn();

						states.RemoveAt(i);
					}
					// Update
					else
					{
						if (state.Skip == false)
						{
							state.Update(state.Age / state.Duration);
						}
#if UNITY_EDITOR
						DirtyTarget(state);
#endif
					}
				}
			}
		}
#if UNITY_EDITOR
		/// <summary>If a transition is being animated in the editor, then the target object may not update, so this method will automatically dirty it so that it will.</summary>
		private static void DirtyTarget(LeanState transition)
		{
			if (Application.isPlaying == false)
			{
				var targetField = transition.GetType().GetField("Target");

				if (targetField != null)
				{
					var target = targetField.GetValue(transition) as Object;

					if (target != null)
					{
						EditorUtility.SetDirty(target);
					}
				}
			}
		}
#endif
	}
}

#if UNITY_EDITOR
namespace Lean.Transition
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(LeanTransition))]
	public class LeanTransition_Inspector : LeanInspector<LeanTransition>
	{
		protected override void DrawInspector()
		{
			Draw("Timing", "This allows you to set where in the game loop animations are updated when timing = LeanTime.Default.");

			EditorGUILayout.Separator();

			EditorGUI.BeginDisabledGroup(true);
				EditorGUILayout.IntField("Transition Count", LeanTransition.Count);
			EditorGUI.EndDisabledGroup();
		}

		[MenuItem("GameObject/Lean/Transition", false, 1)]
		private static void CreateLocalization()
		{
			var gameObject = new UnityEngine.GameObject(typeof(LeanTransition).Name);

			Undo.RegisterCreatedObjectUndo(gameObject, "Create LeanTransition");

			gameObject.AddComponent<LeanTransition>();

			Selection.activeGameObject = gameObject;
		}
	}
}
#endif