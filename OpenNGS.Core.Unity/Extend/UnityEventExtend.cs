using UnityEngine;
using UnityEngine.Events;

namespace SLG
{
	/// <summary>
	/// UnityEvent的扩展
	/// </summary>
	public static class UnityEventExtend
	{
		public static void SafeInvoke(this UnityEvent unityEvent)
		{
			if (unityEvent != null)
			{
				unityEvent.Invoke();
			}
		}
	}
}
