using LRT.TMP_Lively.LinkTags;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace LRT.LinkTags.Editor
{
	public class LinkTagSettings : ScriptableObject
	{
		private static readonly string LinkTagSettingsPath = "Assets/Root/Editor/";
		private static readonly string AssetExtenstion = ".asset";
		public static string FullSettingsPath => LinkTagSettingsPath + typeof(LinkTagSettings).Name + "Asset" + AssetExtenstion;

		public List<LinkTag> tags = new List<LinkTag>();

		internal static LinkTagSettings GetOrCreateSettings()
		{
			var settings = AssetDatabase.LoadAssetAtPath<LinkTagSettings>(FullSettingsPath);
			if (settings == null)
			{
				settings = ScriptableObject.CreateInstance<LinkTagSettings>();

				AssetDatabase.CreateAsset(settings, FullSettingsPath);
				AssetDatabase.SaveAssets();
			}
			return settings;
		}

		internal static SerializedObject GetSerializedSettings()
		{
			return new SerializedObject(GetOrCreateSettings());
		}
	}

	// Register a SettingsProvider using IMGUI for the drawing framework:
	public class LinkTagSettingsProvider : SettingsProvider
	{
		public LinkTagSettingsProvider(string path, SettingsScope scope = SettingsScope.Project)
			: base(path, scope) { }

		private SerializedObject settings;

		public override void OnGUI(string searchContext)
		{
			settings.Update();

			EditorGUILayout.PropertyField(settings.FindProperty(nameof(LinkTagSettings.tags)), new GUIContent("Tags"));

			settings.ApplyModifiedProperties();
		}

		public override void OnActivate(string searchContext, VisualElement rootElement)
		{
			// This function is called when the user clicks on the LRT/Link Tags element in the Settings window.
			settings = LinkTagSettings.GetSerializedSettings();
		}

		// Register the SettingsProvider
		[SettingsProvider]
		public static SettingsProvider CreateLinkTagsSettingProvider()
		{
			var provider = new LinkTagSettingsProvider("Project/LRT/Link Tags", SettingsScope.Project);

			// Automatically extract all keywords from the Styles.
			provider.keywords = new List<string>() { "LRT", "TMP", "TMPLively", "Lively", "Link", "LinkTags", "Tags", "RichText" };
			return provider;
		}
	}
}