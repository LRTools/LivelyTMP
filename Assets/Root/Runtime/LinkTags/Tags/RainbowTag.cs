using System.Collections;
using TMPro;
using UnityEngine;

namespace LRT.TMP_Lively.LinkTags
{
	[CreateAssetMenu(fileName = "RainbowTag", menuName = "LRT/TMP_Lively/Rainbow")]
	public class RainbowTag : LinkTag
	{
		public override string tag => "rainbow";

		public float time = 1f;
		public float colorStep = 10;

		public override void Process(TMP_Text text, TMP_Lively.LinkInfo linkinfo)
		{
			text.StartCoroutine(RainbowText(text, linkinfo));
		}

		private IEnumerator RainbowText(TMP_Text text, TMP_Lively.LinkInfo linkinfo)
		{
			text.ForceMeshUpdate();

			TMP_TextInfo textInfo = text.textInfo;

			CharacterRainbowData[] cachedWobbleData = new CharacterRainbowData[linkinfo.wordSize];
			for (int i = 0; i < cachedWobbleData.Length; i++)
			{
				cachedWobbleData[i].colorOffset = i * colorStep;
			}

			while (LinkTagSettings.Instance.enable)
			{
				for (int i = linkinfo.start; i < linkinfo.end; i++)
				{
					if (!textInfo.characterInfo[i].isVisible)
						continue;

					ApplyColorChange(textInfo, textInfo.characterInfo[i]);
				}

				PushChangesIntoMeshes(text, textInfo);

				// Do the while on everyframe
				yield return new WaitForEndOfFrame();
			}
		}

		#region Helpers
		private void ApplyColorChange(TMP_TextInfo info, TMP_CharacterInfo characterInfo)
		{
			Color oldColor = info.meshInfo[characterInfo.materialReferenceIndex].colors32[0];
			HSVColor hsv = new HSVColor(oldColor);
			hsv.SetHue(hsv.hue + (time * Time.deltaTime));
			Color newColor = hsv.ToRGB();
			Debug.Log(newColor);

			for (int i = 0; i < 4; i++)
			{
				info.meshInfo[characterInfo.materialReferenceIndex].colors32[characterInfo.index + i] = newColor;
			}
		}
			
		private void PushChangesIntoMeshes(TMP_Text text, TMP_TextInfo textInfo)
		{
			for (int i = 0; i < textInfo.meshInfo.Length; i++)
			{
				textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
				text.UpdateVertexData();
			}
		}
		#endregion

		public struct CharacterRainbowData
		{
			public float colorOffset;
		}

		public struct HSVColor
		{
			public float hue, saturation, value;

			public HSVColor(Color color)
			{
				Color.RGBToHSV(color, out hue, out saturation, out value);
			}

			public HSVColor SetHue(float value)
			{
				hue = Mathf.Clamp01(hue + value);
				return this;
			}

			public Color ToRGB()
			{
				return Color.HSVToRGB(hue, saturation, value);
			}
		}
	}
}
