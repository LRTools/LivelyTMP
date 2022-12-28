using LRT.TMP_Lively.LinkTags;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace LRT.TMP_Lively
{
	[RequireComponent(typeof(TMP_Text))]
	public class TMP_Lively : MonoBehaviour
	{
		TMP_Text text;

		private void Awake()
		{
			text = GetComponent<TMP_Text>();
		}

		void Start()
		{
			// We force an update of the text object since it would only be updated at the end of the frame. Ie. before this code is executed on the first frame.
			// Alternatively, we could yield and wait until the end of the frame when the text object will be generated.
			text.ForceMeshUpdate();

			List<LinkTagInfo> linksTagInfo = new List<LinkTagInfo>();

			foreach (TMP_LinkInfo linkInfo in text.textInfo.linkInfo)
			{
				foreach (LinkTag linkTag in LinkTagSettings.Instance.tags)
				{
					if (linkTag.tag == linkInfo.GetLinkID())
						linksTagInfo.Add(new LinkTagInfo(linkInfo, linkTag));
				}
			}

			foreach (LinkTagInfo lti in linksTagInfo)
			{
				lti.tag.Process(text, lti.info);
			}
		}

		private struct LinkTagInfo
		{
			public LinkTag tag;
			public LinkInfo info;

			public LinkTagInfo(TMP_LinkInfo linkInfo, LinkTag tag)
			{
				this.tag = tag;

				info = new LinkInfo(linkInfo);
			}
		}

		public struct LinkInfo
		{
			public int start;
			public int end;

			public int wordSize => end - start;

			public LinkInfo(TMP_LinkInfo info)
			{
				start = info.linkTextfirstCharacterIndex;
				end = start + info.linkTextLength;
			}
		}
	}

	
}


