using Stratus.Unity.Behaviours;
using Stratus.Unity.Scenes;
using Stratus.Utilities;

using System;
using System.Collections.Generic;

using UnityEngine;

namespace Stratus.Unity.Rendering
{
	/// <summary>
	/// A programmatic overlay for debugging use. You can use the preset window
	/// for quick prototyping, or make your own windows.
	/// </summary>
	[StratusSingleton("Stratus Overlay", true, true)]
	public partial class StratusGUI : SingletonBehaviour<StratusGUI>
	{
		//------------------------------------------------------------------------/
		// Declarations
		//------------------------------------------------------------------------/
		public delegate void OnGUILayout(Rect rect);

		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/
		/// <summary>
		/// The current screen size of the game window
		/// </summary>
		public static Vector2 screenSize
		{
			get
			{
#if UNITY_EDITOR
				string[] res = UnityEditor.UnityStats.screenRes.Split('x');
				Vector2 screenSize = new Vector2(int.Parse(res[0]), int.Parse(res[1]));
#else
        Vector2 screenSize = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);
#endif
				return screenSize;
			}
		}

		//------------------------------------------------------------------------/
		// Fields
		//------------------------------------------------------------------------/
		/// <summary>
		/// All custom windows written by the user
		/// </summary>
		private Dictionary<string, Window> CustomWindows = new Dictionary<string, Window>();

		//------------------------------------------------------------------------/
		// Messages
		//------------------------------------------------------------------------/
		protected override void OnAwake()
		{
			this.Reset();
			StratusScene.onSceneChanged += this.OnSceneChanged;
		}

		private void OnGUI()
		{
			this.Draw();
		}

		//------------------------------------------------------------------------/
		// Methods: Static
		//------------------------------------------------------------------------/
		public static void GUILayoutArea(Anchor anchor, Vector2 size, System.Action<Rect> onGUI)
		{
			Rect rect = StratusGUI.CalculateAnchoredPositionOnScreen(anchor, size);
			GUILayout.BeginArea(rect);
			onGUI(rect);
			GUILayout.EndArea();
		}

		public static void GUILayoutArea(Anchor anchor, Vector2 size, GUIContent content, System.Action<Rect> onGUI)
		{
			Rect rect = StratusGUI.CalculateAnchoredPositionOnScreen(anchor, size);
			GUILayout.BeginArea(rect, content);
			onGUI(rect);
			UnityEngine.GUILayout.EndArea();
		}

		public static void GUILayoutArea(Anchor anchor, Vector2 size, GUIContent content, GUIStyle style, System.Action<Rect> onGUI)
		{
			Rect rect = StratusGUI.CalculateAnchoredPositionOnScreen(anchor, size);
			UnityEngine.GUILayout.BeginArea(rect, content, style);
			onGUI(rect);
			UnityEngine.GUILayout.EndArea();
		}

		public static void GUILayoutArea(Anchor anchor, Vector2 size, GUIStyle style, System.Action<Rect> onGUI)
		{
			Rect rect = StratusGUI.CalculateAnchoredPositionOnScreen(anchor, size);
			UnityEngine.GUILayout.BeginArea(rect, style);
			onGUI(rect);
			UnityEngine.GUILayout.EndArea();
		}

		public static void GUIBox(Rect rect, Color tint, GUIStyle style)
		{
			Color currentColor = GUI.color;
			GUI.color = tint;
			GUI.Box(rect, string.Empty, style);
			GUI.color = currentColor;
		}

		public static void GUIBox(Rect rect, Color tint)
		{
			Color currentColor = GUI.color;
			GUI.color = tint;
			GUI.Box(rect, string.Empty);
			GUI.color = currentColor;
		}

		//------------------------------------------------------------------------/
		// Methods: Private
		//------------------------------------------------------------------------/
		/// <summary>
		/// Resets all windows to their defaults
		/// </summary>
		private void Reset()
		{
		}

		/// <summary>
		/// When the scene changes, reset all windows!
		/// </summary>
		private void OnSceneChanged()
		{
			this.Reset();
		}

		/// <summary>
		/// Draws all overlay elements
		/// </summary>
		private void Draw()
		{
			// Draw all custom windows
			foreach (KeyValuePair<string, Window> window in CustomWindows)
			{
				window.Value.Draw();
			}
		}

		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/
		public static Vector2 half { get; } = new Vector2(0.5f, 0.5f);
		public static Vector2 quarter { get; } = new Vector2(0.25f, 0.25f);
		public static Vector2 quarterScreen => CalculateRelativeDimensions(quarter, screenSize);

		//------------------------------------------------------------------------/
		// Methods
		//------------------------------------------------------------------------/
		/// <summary>
		/// Calculate the dimensions relative to screen space, from a given percentage (0 to 1).
		/// So for example if you wanted to cover 10% of the screen's width and 20% of its height,
		/// you would pass in (0.1f, 0,2f)
		/// </summary>
		/// <param name="widthRatio">The relative width of the screen as a percentage (from 0 to 1)</param>
		/// <param name="heightRatio">The relative height of the screen as a percentage (from 0 to 1)</param>
		/// <returns></returns>
		public static Vector2 CalculateRelativeDimensions(Vector2 sizeRatio, Vector2 screenSize)
		{
			if (sizeRatio.x < 0f || sizeRatio.x > 1f || sizeRatio.y < 0f || sizeRatio.y > 1f)
				throw new ArgumentOutOfRangeException("Expected a value between 0 and 1!");
			return new Vector2(screenSize.x * sizeRatio.x, screenSize.y * sizeRatio.y);
		}

		/// <summary>
		/// Computes a proper rect from a given anchored position along with the width and height
		/// </summary>
		/// <param name="anchor">The relative position of the rect in screen space</param>
		/// <param name="size">The dimensions of the rect, relative to the given screen size</param>
		/// <returns></returns>
		public static Rect CalculateAnchoredPositionOnScreen(Anchor anchor, Vector2 size)
		{
			return CalculateAnchoredPositionOnScreen(anchor, size, screenSize);
		}

		/// <summary>
		/// Computes a proper rect from a given anchored position along with the width and height
		/// </summary>
		/// <param name="anchor">The relative position of the rect in screen space</param>
		/// <param name="size">The dimensions of the rect, relative to the given screen size</param>
		/// <param name="screenSize">The dimensions of the screen the rect will be in</param>
		/// <returns></returns>
		public static Rect CalculateAnchoredPositionOnScreen(Anchor anchor, Vector2 size, Vector2 screenSize)
		{
			var rect = new Rect();

			float width = size.x;
			float height = size.y;

			float screenWidth = screenSize.x;
			float screenHeight = screenSize.y;

			const float padding = 8f;
			// This is stupid. I couldn't figure out why it won't position properly otherwise if anchored to the bottom
			const float bottomMultiplier = 3f;

			// Find the x and y positions depending on the anchor
			float x = 0f;
			float y = 0f;

			switch (anchor)
			{
				case Anchor.Center:
					x = screenWidth / 2 - (width / 2);
					y = screenHeight / 2 - (height / 2);
					break;
				case Anchor.Top:
					x = screenWidth / 2 - (width / 2);
					y = padding;
					break;
				case Anchor.TopLeft:
					x = padding;
					y = padding;
					break;
				case Anchor.TopRight:
					x = screenWidth - width - padding;
					y = padding;
					break;
				case Anchor.Left:
					x = padding;
					y = screenHeight / 2 - ((height / 2) - padding);
					break;
				case Anchor.Right:
					x = screenWidth - width - padding;
					y = screenHeight / 2 - ((height / 2) - padding);
					break;
				case Anchor.Bottom:
					x = screenWidth / 2 - (width / 2);
					y = screenHeight - height - (padding * bottomMultiplier);
					break;
				case Anchor.BottomLeft:
					x = padding;
					y = screenHeight - height - (padding * bottomMultiplier);
					break;
				case Anchor.BottomRight:
					x = screenWidth - width - padding;
					y = screenHeight - height - (padding * bottomMultiplier);
					break;
			}

			// Set the values
			rect.x = x;
			rect.y = y;
			rect.width = width;
			rect.height = height;

			return rect;
		}

		/// <summary>
		/// Makes a texture
		/// </summary>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="textureColor"></param>
		/// <param name="border"></param>
		/// <param name="bordercolor"></param>
		/// <returns></returns>
		private static Texture2D MakeTexture(int width, int height, Color textureColor, RectOffset border, Color bordercolor)
		{
			int widthInner = width;
			width += border.left;
			width += border.right;

			Color[] pix = new Color[width * (height + border.top + border.bottom)];

			for (int i = 0; i < pix.Length; i++)
			{
				if (i < (border.bottom * width))
					pix[i] = bordercolor;
				else if (i >= ((border.bottom * width) + (height * width)))  //Border Top
					pix[i] = bordercolor;
				else
				{ //Center of Texture

					if ((i % width) < border.left) // Border left
						pix[i] = bordercolor;
					else if ((i % width) >= (border.left + widthInner)) //Border right
						pix[i] = bordercolor;
					else
						pix[i] = textureColor;    //Color texture
				}
			}

			Texture2D result = new Texture2D(width, height + border.top + border.bottom);
			result.SetPixels(pix);
			result.Apply();


			return result;
		}

		private static Texture2D MakeTexture(int width, int height, Color col)
		{
			Color[] pix = new Color[width * height];

			for (int i = 0; i < pix.Length; i++)
				pix[i] = col;

			Texture2D result = new Texture2D(width, height);
			result.SetPixels(pix);
			result.Apply();

			return result;
		}

	}
}
