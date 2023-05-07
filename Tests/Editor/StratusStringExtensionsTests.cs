using NUnit.Framework;

using Stratus.Extensions;
using Stratus.Unity.Data;
using Stratus.Unity.Extensions;
using Stratus.Utilities;

using UnityEngine;

namespace Stratus.Editor.Tests
{
	public class StratusStringExtensionsTests
	{
		[Test]
		public void IsRichTextValid()
		{
			RichText richText = null;
			Assert.False(richText.IsValid());
			richText = new RichText(null);
			Assert.False(richText.IsValid());
			richText = new RichText("");
			Assert.False(richText.IsValid());
			richText = new RichText("foo!");
			Assert.True(richText.IsValid());
		}

		[TestCase("foo")]
		[TestCase("bar!")]
		[TestCase("")]
		public void RichTextStoresOriginalText(string text)
		{
			RichText richText = new RichText(text);
			Assert.AreEqual(text, richText.text);
		}

		[TestCase("foo", FontStyle.Italic, "<i>foo</i>")]
		[TestCase("bar", FontStyle.Bold, "<b>bar</b>")]
		[TestCase("foobar", FontStyle.Normal, "foobar")]
		public void RichTextIsGeneratedCorrectly(string input, FontStyle fontStyle, string expected)
		{
			RichTextOptions options = new RichTextOptions()
			{
				style = fontStyle
			};
			RichText richText = new RichText(input, options);
			Assert.AreEqual(expected, richText.richText);
		}

		[TestCase("Hello there!")]
		public void RichTextIsApplied(string input)
		{
			string output;

			void compareCleanText()
			{
				output = output.StripRichText();
				Assert.AreEqual(output, input);
			}

			foreach (var style in EnumUtility.Values<FontStyle>())
			{
				output = input.ToRichText(style);
				compareCleanText();
			}

			output = input.ToRichText(Color.green);
			compareCleanText();

			output = input.ToRichText(34);
			compareCleanText();
		}

		[TestCase(0)]
		[TestCase(-1)]
		public void RichTextIgnoresInvalidSize(int size)
		{
			string input = "hello there!";
			string output = input.ToRichText(size);
			Assert.AreEqual(input, output);
		}		
	}
}