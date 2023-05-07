using Stratus.Unity.Extensions;

using System;

using UnityEngine;

namespace Stratus.Unity.Data
{
	[Serializable]
	public class RichText
	{
		[SerializeField]
		private string _text;
		[SerializeField]
		private RichTextOptions _options;

		/// <summary>
		/// The raw, unformatted text
		/// </summary>
		public string text => _text;

		/// <summary>
		/// The formatted text
		/// </summary>
		public string richText
		{
			get
			{
				if (!generated)
				{
					_richText = text.ToRichText(_options);
					generated = true;
				}
				return _richText;
			}
		}
		private string _richText;
		private bool generated;

		public RichText()
		{
		}

		public RichText(string text, RichTextOptions options)
		{
			this._text = text;
			this._options = options;
		}

		public RichText(string text)
		{
			this._text = text;
			this._options = default;
		}

		public override string ToString() => text;

		public static implicit operator string(RichText richText) => richText.text;
		public static implicit operator RichText(string text) => new RichText(text);
	}

}