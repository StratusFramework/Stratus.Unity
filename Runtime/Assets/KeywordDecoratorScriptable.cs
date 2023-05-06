using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Stratus.Unity
{
	[CreateAssetMenu(menuName = scriptablesMenu + "Keyword Decorator")]
	public class KeywordDecoratorScriptable : StratusAssetCollectionScriptable<StratusRichText>
	{
		protected override string GetKey(StratusRichText element) => element.text;

		public Dictionary<string, string> replacements
		{
			get
			{
				if (_replacements == null)
				{
					_replacements = new Dictionary<string, string>();
					foreach (var keyword in assets)
					{
						_replacements.Add(keyword, keyword.richText);
					}
				}
				return _replacements;
			}
		}
		private Dictionary<string, string> _replacements;
	}

}