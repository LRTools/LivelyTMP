using System.Collections;
using TMPro;
using UnityEngine;

namespace LRT.TMP_Lively.LinkTags
{
	[CreateAssetMenu(fileName = "WobbleTag", menuName = "LRT/TMP_Lively/Wobble")]
	public class WobbleTag : LinkTag
	{
		public override string tag => "wobble";

		public float speed = 1.2f;
		public float strenght = 60;
		public float frequency = 0.25f;

		public override void Process(TMP_Text text, TMP_Lively.LinkInfo linkinfo)
		{
			text.StartCoroutine(WobbleText(text, linkinfo));
		}

		private IEnumerator WobbleText(TMP_Text text, TMP_Lively.LinkInfo linkinfo)
		{
			// I don't know why, but if we don't do that the source vertices change 
			// the y range will not be the same for each character.
			yield return new WaitForEndOfFrame();

			TMP_TextInfo textInfo = text.textInfo;

			// Cache the vertex data of the text object as the FX is applied to the original position of the characters.
			TMP_MeshInfo[] cachedMeshInfo = textInfo.CopyMeshInfoVertexData();

			CharacterWobbleData[] cachedWobbleData = new CharacterWobbleData[linkinfo.wordSize];
			for (int i = 0; i < cachedWobbleData.Length; i++)
			{
				cachedWobbleData[i].yShiftOffset = i * frequency;
			}

			while (LinkTagSettings.Instance.enable)
			{
				for (int i = linkinfo.start; i < linkinfo.end; i++)
				{
					if (!textInfo.characterInfo[i].isVisible)
						continue;

					int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;
					int vertexIndex = textInfo.characterInfo[i].vertexIndex;
					Vector3[] sourceVertices = cachedMeshInfo[materialIndex].vertices;
					Vector3[] destinationVertices = textInfo.meshInfo[materialIndex].vertices;

					float yOffset = GetWobbleOffset(cachedWobbleData[i - linkinfo.start]).y;

					ApplyDestinationOffsetChange(vertexIndex, destinationVertices, sourceVertices, yOffset);
				}

				PushChangesIntoMeshes(text, textInfo);

				// Do the while on everyframe
				yield return new WaitForEndOfFrame();
			}
		}

		#region Helpers
		private Vector3 GetWobbleOffset(CharacterWobbleData wobbleData)
		{
			float yWobbleOffset = Mathf.PingPong((Time.time + wobbleData.yShiftOffset) * speed, 1);

			yWobbleOffset = Map(yWobbleOffset, 0, 1, -(strenght / 2), strenght / 2);

			return new Vector3(0, yWobbleOffset, 0);
		}

		private float Map(float value, float from1, float to1, float from2, float to2)
		{
			return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
		}

		private void ApplyDestinationOffsetChange(int vertexIndex, Vector3[] destinationVertices, Vector3[] sourceVertices, float yOffset)
		{
			destinationVertices[vertexIndex + 0] = sourceVertices[vertexIndex + 0] + Vector3.up * yOffset;
			destinationVertices[vertexIndex + 1] = sourceVertices[vertexIndex + 1] + Vector3.up * yOffset;
			destinationVertices[vertexIndex + 2] = sourceVertices[vertexIndex + 2] + Vector3.up * yOffset;
			destinationVertices[vertexIndex + 3] = sourceVertices[vertexIndex + 3] + Vector3.up * yOffset;
		}

		private void PushChangesIntoMeshes(TMP_Text text, TMP_TextInfo textInfo)
		{
			for (int i = 0; i < textInfo.meshInfo.Length; i++)
			{
				textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
				text.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
			}
		}
		#endregion

		public struct CharacterWobbleData
		{
			public float yShiftOffset;
		}
	}
}
