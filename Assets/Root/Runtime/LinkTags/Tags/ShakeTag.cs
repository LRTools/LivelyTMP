using System.Collections;
using TMPro;
using UnityEngine;

namespace LRT.TMP_Lively.LinkTags
{
	[CreateAssetMenu(fileName = "ShakeTag", menuName = "LRT/TMP_Lively/Shake")]
	public class ShakeTag : LinkTag
	{
		public override string tag => "shake";

		public int speed = 24;
		public int strenght = 12;

		public override void Process(TMP_Text text, TMP_Lively.LinkInfo linkinfo)
		{
			text.StartCoroutine(ShakeText(text, linkinfo));
		}

		private IEnumerator ShakeText(TMP_Text text, TMP_Lively.LinkInfo linkinfo)
		{
			// I don't know why, but if we don't do that the source vertices change 
			// the y range will not be the same for each character.
			yield return new WaitForEndOfFrame();

			TMP_TextInfo textInfo = text.textInfo;

			// Cache the vertex data of the text object as the FX is applied to the original position of the characters.
			TMP_MeshInfo[] cachedMeshInfo = textInfo.CopyMeshInfoVertexData();

			CharacterShakeData[] cachedShakeData = new CharacterShakeData[linkinfo.wordSize];
			for (int i = 0; i < cachedShakeData.Length; i++)
			{
				cachedShakeData[i].randomYOffset = UnityEngine.Random.Range(0, 2f);
				cachedShakeData[i].speedModifier = UnityEngine.Random.Range(0, 0.5f);
			}

			while (true)
			{
				for (int i = linkinfo.start; i < linkinfo.end; i++)
				{
					if (!textInfo.characterInfo[i].isVisible)
						continue;

					int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;
					int vertexIndex = textInfo.characterInfo[i].vertexIndex;
					Vector3[] sourceVertices = cachedMeshInfo[materialIndex].vertices;
					Vector3[] destinationVertices = textInfo.meshInfo[materialIndex].vertices;

					float yOffset = GetShakeOffset(cachedShakeData[i - linkinfo.start]).y;

					ApplyDestinationOffsetChange(vertexIndex, destinationVertices, sourceVertices, yOffset);
				}

				PushChangesIntoMeshes(text, textInfo);

				// Do the while on everyframe
				yield return new WaitForEndOfFrame();
			}
		}

		#region Helpers
		private Vector3 GetShakeOffset(CharacterShakeData shakeData)
		{
			float shakeSpeed = speed * (1 + shakeData.speedModifier); // +1 stand to multiply by 1.xx instead of 0.xx 
			float YShakeOffset = Mathf.PingPong((Time.time + shakeData.randomYOffset) * shakeSpeed, 2.0f) - 1.0f;
			YShakeOffset = YShakeOffset * strenght;

			return new Vector3(0, YShakeOffset, 0); ;
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

		public struct CharacterShakeData
		{
			public float randomYOffset;
			public float speedModifier;
		}
	}
}
