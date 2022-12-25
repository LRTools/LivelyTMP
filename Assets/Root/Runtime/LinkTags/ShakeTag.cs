using System.Collections;
using TMPro;
using UnityEngine;

namespace LRT.TMP_Lively.LinkTags
{
	public class ShakeTag : LinkTag
	{
		public override string tag => "shake";

		public int speed = 5;
		public int strenght = 12;

		public override void Process(TMP_Text text, TMP_Lively.LinkInfo linkinfo)
		{
			text.StartCoroutine(ShakeText(text, linkinfo));
		}

		private IEnumerator ShakeText(TMP_Text text, TMP_Lively.LinkInfo linkinfo)
		{
			TMP_TextInfo textInfo = text.textInfo;
			Matrix4x4 matrix;

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
					if (!linkinfo.Has(i))
						continue;

					int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;
					int vertexIndex = textInfo.characterInfo[i].vertexIndex;
					Vector3[] sourceVertices = cachedMeshInfo[materialIndex].vertices;

					// Need to translate all 4 vertices of each quad to aligned with middle of character / baseline.
					// This is needed so the matrix TRS is applied at the origin for each character.
					Vector3 offset = GetCharMidBaseline(vertexIndex, sourceVertices);
					Vector3[] destinationVertices = textInfo.meshInfo[materialIndex].vertices;
					ResetDestinationWithSource(vertexIndex, sourceVertices, offset, destinationVertices);

					matrix = Matrix4x4.TRS(GetShakeOffset(cachedShakeData[i - linkinfo.start]), Quaternion.identity, Vector3.one);

					ApplyMatrixChange(matrix, vertexIndex, offset, destinationVertices);
				}

				PushChangesIntoMeshes(text, textInfo);

				// Do the while on everyframe
				yield return new WaitForEndOfFrame();
			}
		}

		#region Helpers
		private static Vector2 GetCharMidBaseline(int vertexIndex, Vector3[] sourceVertices)
		{
			// Determine the center point of each character at the baseline.
			//Vector2 charMidBasline = new Vector2((sourceVertices[vertexIndex + 0].x + sourceVertices[vertexIndex + 2].x) / 2, charInfo.baseLine);
			// Determine the center point of each character.
			return (sourceVertices[vertexIndex + 0] + sourceVertices[vertexIndex + 2]) / 2;
		}

		private static void ResetDestinationWithSource(int vertexIndex, Vector3[] sourceVertices, Vector3 offset, Vector3[] destinationVertices)
		{
			destinationVertices[vertexIndex + 0] = sourceVertices[vertexIndex + 0] - offset;
			destinationVertices[vertexIndex + 1] = sourceVertices[vertexIndex + 1] - offset;
			destinationVertices[vertexIndex + 2] = sourceVertices[vertexIndex + 2] - offset;
			destinationVertices[vertexIndex + 3] = sourceVertices[vertexIndex + 3] - offset;
		}

		private Vector3 GetShakeOffset(CharacterShakeData shakeData)
		{
			float shakeSpeed = speed * (1 + shakeData.speedModifier); // +1 stand to multiply by 1.xx instead of 0.xx 
			float YShakeOffset = Mathf.PingPong((Time.time + shakeData.randomYOffset) * shakeSpeed, 2.0f) - 1.0f;
			YShakeOffset = YShakeOffset * strenght;

			return new Vector3(0, YShakeOffset, 0); ;
		}

		private void ApplyMatrixChange(Matrix4x4 matrix, int vertexIndex, Vector3 offset, Vector3[] destinationVertices)
		{
			destinationVertices[vertexIndex + 0] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 0]);
			destinationVertices[vertexIndex + 1] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 1]);
			destinationVertices[vertexIndex + 2] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 2]);
			destinationVertices[vertexIndex + 3] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 3]);

			destinationVertices[vertexIndex + 0] += offset;
			destinationVertices[vertexIndex + 1] += offset;
			destinationVertices[vertexIndex + 2] += offset;
			destinationVertices[vertexIndex + 3] += offset;
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
