using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static LRT.TMP_Lively.TMP_Lively;

namespace LRT.TMP_Lively.LinkTags
{
	public abstract class LinkTag : ScriptableObject
	{
		public abstract string tag { get; }

		public abstract void Process(TMP_Text text, LinkInfo info);
	}
}

