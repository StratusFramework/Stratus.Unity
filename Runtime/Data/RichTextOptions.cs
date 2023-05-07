using Stratus.Unity.Extensions;

using System;

using UnityEngine;

namespace Stratus.Unity.Data
{
	[Serializable]
	public struct RichTextOptions
	{
		public FontStyle style;
		public int size;

		[SerializeField]
		private Color color;

		public string hexColor
		{
			get
			{
				if (_hexColor == null && color != default)
				{
					_hexColor = color.ToHex();
				}
				return _hexColor;
			}
		}
		[NonSerialized]
		private string _hexColor;

		private static Lazy<RichTextOptions> _default = new Lazy<RichTextOptions>(() => new RichTextOptions());

		public RichTextOptions(FontStyle style, string hexColor, int size)
		{
			this.style = style;
			this._hexColor = hexColor;
			this.color = default;
			this.size = size;
		}

		public RichTextOptions(FontStyle style, Color color, int size)
			: this(style, color.ToHex(), size)
		{
		}

		public static RichTextOptions Default() => _default.Value;
	}

}