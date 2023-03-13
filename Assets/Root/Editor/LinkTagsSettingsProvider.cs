using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace LRT.TMP_Lively.LinkTags.Editor
{
	public class LinkTagSettingsProvider : SettingsProvider
	{
		public LinkTagSettingsProvider(string path, SettingsScope scope = SettingsScope.User)
			: base(path, scope) { }

		private SerializedObject settings;

		public override void OnGUI(string searchContext)
		{
			settings.Update();

			EditorGUILayout.PropertyField(settings.FindProperty(nameof(LinkTagSettings.enable)), new GUIContent("Enable"));
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
			var provider = new LinkTagSettingsProvider("LRT/Link Tags", SettingsScope.User);

			// Automatically extract all keywords from the Styles.
			provider.keywords = new List<string>() { "LRT", "TMP", "TMPLively", "Lively", "Link", "LinkTags", "Tags", "RichText" };
			return provider;
		}
	}
}


