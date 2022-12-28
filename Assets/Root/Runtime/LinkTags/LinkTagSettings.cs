using LRT.Utilities;
using System.Collections.Generic;
using UnityEditor;

#if UNITY_EDITOR
using UnityEngine;
#endif

namespace LRT.TMP_Lively.LinkTags
{
	public class LinkTagSettings : LazySingleton<LinkTagSettings>
	{
		public bool enable = true;
		public List<LinkTag> tags = new List<LinkTag>();

		#region Editor creation
#if UNITY_EDITOR
		private static readonly string ResourcesPath = "Assets/Resources/";
		private static readonly string AssetExtenstion = ".asset";
		private static string FullSettingsPath => ResourcesPath + typeof(LinkTagSettings).Name + AssetExtenstion;

		private static LinkTagSettings GetOrCreateSettings()
		{
			LinkTagSettings settings = null;

			if (!AssetDatabase.IsValidFolder(ResourcesPath))
			{
				AssetDatabase.CreateFolder("Assets", "Resources");
				settings = CreateSettingsIfNull(FullSettingsPath);
			}

			if (settings == null)
				settings = CreateSettingsIfNull(FullSettingsPath);

			return settings;
		}

		private static LinkTagSettings CreateSettingsIfNull(string path)
		{
			LinkTagSettings settings = AssetDatabase.LoadAssetAtPath<LinkTagSettings>(path);
			if (settings == null)
			{
				settings = ScriptableObject.CreateInstance<LinkTagSettings>();

				AssetDatabase.CreateAsset(settings, path);
				AssetDatabase.SaveAssets();
			}
			return settings;
		}

		public static SerializedObject GetSerializedSettings()
		{
			return new SerializedObject(GetOrCreateSettings());
		}
#endif
		#endregion
	}
}