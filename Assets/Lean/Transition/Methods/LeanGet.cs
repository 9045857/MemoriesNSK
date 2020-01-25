using UnityEngine;

namespace Lean.Transition
{
	public static partial class LeanExtensions
	{
		/// <summary>This will give you the previously registered transition state.</summary>
		public static LeanState GetTransition<T>(this T target)
			where T : Component
		{
			return LeanTransition.CurrentHead;
		}

		/// <summary>This will give you the previously registered transition state.</summary>
		public static LeanState GetTransition(this UnityEngine.GameObject target)
		{
			return LeanTransition.CurrentHead;
		}
	}
}