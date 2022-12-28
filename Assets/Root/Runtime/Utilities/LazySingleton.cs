using UnityEngine;

namespace LRT.Utilities
{
	/// <summary>
	/// LazySingleton class load the the object in :<br/>
	/// <code>Assets/Resources/typeof(T).Name</code>
	/// </summary>
	public class LazySingleton<T> : ScriptableObject where T : class
    {
		private static T _instance;
        public static T Instance
		{
			get
			{
				if(_instance == null)
					_instance = Resources.Load(typeof(T).Name) as T;

				return _instance;
			}
		}
    }
}


