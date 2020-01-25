using UnityEngine;

namespace Lean.Transition.Method
{
	/// <summary>This will submit all transitions you added before this one. Any transitions you perform after this will begin immediately.</summary>
	[HelpURL(LeanTransition.HelpUrlPrefix + "LeanInsert")]
	[AddComponentMenu(LeanTransition.MethodsMenuPrefix + "Insert" + LeanTransition.MethodsMenuSuffix)]
	public class LeanInsert : LeanMethod
	{
		public Transform Target;

		public override void Register()
		{
			LeanTransition.InsertTransitions(Target);
		}
	}
}

namespace Lean.Transition
{
	public static partial class LeanExtensions
	{
		/// <summary>This will insert all transitions inside the specified GameObject, as if they were added manually.</summary>
		public static T InsertTransition<T>(this T target, UnityEngine.GameObject root)
			where T : Component
		{
			LeanTransition.InsertTransitions(root); return target;
		}

		/// <summary>This will insert all transitions inside the specified GameObject, as if they were added manually.</summary>
		public static T InsertTransition<T>(this T target, Transform root)
			where T : Component
		{
			LeanTransition.InsertTransitions(root); return target;
		}

		/// <summary>This will insert all transitions inside the specified GameObject, as if they were added manually.</summary>
		public static GameObject InsertTransition(this UnityEngine.GameObject target, UnityEngine.GameObject root)
		{
			LeanTransition.InsertTransitions(root); return target;
		}

		/// <summary>This will insert all transitions inside the specified GameObject, as if they were added manually.</summary>
		public static GameObject InsertTransition(this UnityEngine.GameObject target, Transform root)
		{
			LeanTransition.InsertTransitions(root); return target;
		}
	}
}