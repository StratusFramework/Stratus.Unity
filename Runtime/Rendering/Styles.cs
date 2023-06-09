using UnityEngine;
using System.Collections.Generic;

namespace Stratus.Unity.Rendering
{
	/// <summary>
	/// Contains styles used by the Stratus framework
	/// </summary>
	public static class Styles
	{
		//------------------------------------------------------------------------/
		// Declarations
		//------------------------------------------------------------------------/
		public enum Border
		{
			Full,
			Left,
			Right
		}

		public class EditorStyles
		{
			public GUIStyle button { get; private set; }
			public GUIStyle toolbarButton { get; private set; }

			public EditorStyles()
			{
				button = new GUIStyle(GUI.skin.button);
				button.alignment = TextAnchor.MiddleCenter;

#if UNITY_EDITOR
				toolbarButton = new GUIStyle(UnityEditor.EditorStyles.toolbarButton);
				toolbarButton.alignment = TextAnchor.MiddleCenter;
#endif
			}

		}

		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/
		/// <summary>
		/// The default skin used by the Stratus framework
		/// </summary>
		public static GUISkin skin { get; private set; }
		/// <summary>
		/// Variations on Unity's EditorStyles. Invoke only during an OnGUI event!
		/// </summary>
		public static EditorStyles editorStyles
		{
			get
			{
				if (_editorStyles == null)
					_editorStyles = new EditorStyles();
				return _editorStyles;
			}
		}
		/// <summary>
		/// Common style for labels
		/// </summary>
		public static GUIStyle label { get; private set; }
		/// <summary>
		/// A thin line for used within an editor window
		/// </summary>
		public static GUIStyle editorLine { get; private set; }
		/// <summary>
		/// A tintable background
		/// </summary>
		public static GUIStyle tintable { get; private set; }
		/// <summary>
		/// A map of all custom textures used by the framework
		/// </summary>
		public static Dictionary<string, Texture2D> textures { get; set; } = new Dictionary<string, Texture2D>();
		/// <summary>
		/// A map of all custom textures used by the framework
		/// </summary>
		public static Dictionary<string, Font> fonts { get; set; } = new Dictionary<string, Font>();
		/// <summary>
		/// A map of all colored backgrounds generated 
		/// </summary>
		private static Dictionary<Color, GUIStyle> coloredBackgrounds = new Dictionary<Color, GUIStyle>();
		/// <summary>
		/// A map of all colored backgrounds generated 
		/// </summary>
		private static Dictionary<Color, Texture2D> coloredTextures = new Dictionary<Color, Texture2D>();

		// Styles    
		public static GUIStyle box => skin.box;
		public static GUIStyle button => skin.button;

		public static GUIStyle miniButton => skin.FindStyle("Mini Button");
		public static GUIStyle outlineBox => skin.FindStyle("Outline Box");
		public static GUIStyle circleButton => skin.FindStyle("Circle Button");
		public static GUIStyle tickButton => skin.FindStyle("Tick Button");
		public static GUIStyle header { get; set; }
		public static GUIStyle headerWhite => skin.FindStyle("Header White");
		public static GUIStyle headerBlack => skin.FindStyle("Header");
		public static GUIStyle miniText => skin.FindStyle("Mini Text");
		public static GUIStyle background => skin.FindStyle("Background");
		public static GUIStyle backgroundLight => skin.FindStyle("Background Light");
		public static GUIStyle whiteBorder => skin.FindStyle("White Border");
		public static GUIStyle whiteRightBorder => skin.FindStyle("White Right Border");
		public static GUIStyle whiteLeftBorder => skin.FindStyle("White Left Border");
		public static GUIStyle listViewLabel { get; private set; }
		public static GUIStyle listViewToggle { get; private set; }
		public static GUIStyle textField { get; private set; }
		public static GUIStyle popup { get; private set; }

		public static Font unityDefaultFont { get; private set; }
		public static Font defaultFont { get; private set; }
		public static Font boldFont { get; private set; }
		public static Font lightFont { get; private set; }

		//public static GUIStyle currentHeader

		public static bool isProSkin { get; set; }

		// Icons
		public static Texture2D optionsIcon => textures["cog"];
		public static Texture2D addIcon => textures["plus"];
		public static Texture2D messageIcon => textures["talk"];
		public static Texture2D trashIcon => textures["trash-can"];
		public static Texture2D validateIcon => textures["magnifying-glass"];
		public static Texture2D starIcon => textures["round-star"];
		public static Texture2D starStackIcon => textures["stars-stack"];
		public static Texture2D positionMarker => textures["position-marker"];
		// Layouts

		public static GUILayoutOption[] smallLayout { get; private set; }
		public static GUILayoutOption[] singleLineLayout { get; private set; }

		//------------------------------------------------------------------------/
		// Fields
		//------------------------------------------------------------------------/
		private static EditorStyles _editorStyles;

		//------------------------------------------------------------------------/
		// Methods
		//------------------------------------------------------------------------/
		static Styles()
		{
			// Load custom assets
			LoadGUIAssets();
			// Set icons
			// Store default options
			CacheDefaultOptions();
		}

		private static void LoadGUIAssets()
		{
			// Load the default skin
			skin = Resources.Load<GUISkin>("Stratus Skin");

			// Load gui assets
			Texture2D[] assets = Resources.LoadAll<Texture2D>(Constants.guiFolder);
			foreach (var asset in assets)
			{
				textures.Add(asset.name, asset);
			}

			// Load fonts
			Font[] fontAssets = Resources.LoadAll<Font>(Constants.fontFolder);
			foreach (var font in fontAssets)
			{
				fonts.Add(font.name, font);
			}

			defaultFont = fonts["OpenSans-Regular"];
			boldFont = fonts["OpenSans-Bold"];
			lightFont = fonts["OpenSans-Light"];

			// Set defaults
			label = skin.label;
			textField = skin.textField;
			listViewLabel = skin.FindStyle("List Label");
			listViewLabel.wordWrap = true;
			listViewLabel.stretchHeight = false;
			listViewToggle = new GUIStyle(skin.toggle);
			listViewLabel.margin = textField.margin;
			listViewLabel.border = textField.border;
			listViewLabel.padding = textField.padding;


#if UNITY_EDITOR
			//popup = new GUIStyle(UnityEditor.EditorStyles.popup);
			//popup.richText = true;
			//popup.font = boldFont;
			//popup.fontSize = 10;
#endif


		}

		public static void OverrideDefaultFont()
		{
			if (!unityDefaultFont)
				unityDefaultFont = GUI.skin.font;
			GUI.skin.font = defaultFont;
		}

		public static void RevertDefaultFont()
		{
			GUI.skin.font = unityDefaultFont;
		}

		private static void CacheDefaultOptions()
		{
			tintable = new GUIStyle();
			tintable.normal.background = Texture2D.whiteTexture;
			tintable.stretchWidth = tintable.stretchHeight = true;
			smallLayout = new GUILayoutOption[] { GUILayout.Width(25f), GUILayout.Height(25f) };

#if UNITY_EDITOR
			//UnityEditor.EditorStyles.helpBox.richText = true;
			isProSkin = UnityEditor.EditorGUIUtility.isProSkin;
			header = isProSkin ? headerWhite : headerBlack;
			singleLineLayout = new GUILayoutOption[] { GUILayout.Width(UnityEditor.EditorGUIUtility.singleLineHeight), GUILayout.Height(UnityEditor.EditorGUIUtility.singleLineHeight) };
#endif
		}

		/// <summary>
		/// Draws the selected background color inside the given rect
		/// </summary>
		/// <param name="position"></param>
		/// <param name="color"></param>
		public static void DrawBackgroundColor(Rect position, Color color)
		{
			if (Event.current.type == EventType.Repaint)
			{
				var prevColor = GUI.color;
				GUI.color = color;
				tintable.Draw(position, false, false, false, false);
				GUI.color = prevColor;
			}
		}

		/// <summary>
		/// Draws an outline around the given rect
		/// </summary>
		/// <param name="position"></param>
		/// <param name="color"></param>
		public static void DrawOutline(Rect position, Color color, Border border = Border.Full)
		{
			if (Event.current.type == EventType.Repaint)
			{
				var prevColor = GUI.color;
				GUI.color = color;
				switch (border)
				{
					case Border.Full:
						whiteBorder.Draw(position, false, false, false, false);
						break;

					case Border.Left:
						whiteLeftBorder.Draw(position, false, false, false, false);
						break;

					case Border.Right:
						whiteRightBorder.Draw(position, false, false, false, false);
						break;
				}
				GUI.color = prevColor;
			}
		}

		/// <summary>
		/// Generates a 2D texture given another texture to use for dimensions and the color
		/// </summary>
		/// <param name="other"></param>
		/// <param name="color"></param>
		/// <returns></returns>
		public static Texture2D MakeTexture(Texture2D other, Color color)
		{
			return MakeTexture(other.width, other.height, color);
		}

		/// <summary>
		/// Generates a 2D texture
		/// </summary>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="col"></param>
		/// <returns></returns>
		public static Texture2D MakeTexture(int width, int height, Color col)
		{
			Color[] pix = new Color[width * height];

			for (int i = 0; i < pix.Length; i++)
				pix[i] = col;

			Texture2D result = new Texture2D(width, height);
			result.SetPixels(pix);
			result.Apply();

			return result;
		}
		public static GUIStyle ColoredBackground(Color color)
		{
			if (!coloredBackgrounds.ContainsKey(color))
			{
				var cb = new GUIStyle();
				cb.normal.background = MakeTexture(button.normal.background, color);
				coloredBackgrounds.Add(color, cb);
			}

			return coloredBackgrounds[color];
		}

		public static Texture2D GetColorTexture(Color color)
		{
			if (!coloredTextures.ContainsKey(color))
			{
				coloredTextures.Add(color, MakeTexture(Texture2D.whiteTexture, color));
			}

			return coloredTextures[color];
		}		
	}
}