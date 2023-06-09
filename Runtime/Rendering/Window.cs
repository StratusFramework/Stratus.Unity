using System;
using System.Collections.Generic;

using UnityEngine;

namespace Stratus.Unity.Rendering
{
	/// <summary>
	/// Where should the window be anchored to
	/// </summary>
	public enum Anchor
	{
		Center,

		Top,
		Bottom,
		Left,
		Right,

		TopLeft,
		TopRight,
		BottomLeft,
		BottomRight
	}

	/// <summary>
	/// The base class for all windows
	/// </summary>
	public abstract class AbstractWindow : Element
	{
		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/
		/// <summary>
		/// Whether this window is currently active
		/// </summary>
		public abstract bool active { get; }

		/// <summary>
		/// The anchored position of the overlay
		/// </summary>
		public Anchor anchor;

		/// <summary>
		/// The dimensions of this window as ratios between 0 and 1
		/// </summary>
		[Range(0f, 1f)]
		public Vector2 dimensions;

		/// <summary>
		/// The background texture
		/// </summary>
		public Texture2D background;

		/// <summary>
		/// The current size of the screen
		/// </summary>
		public Vector2 screenSize => new Vector2(Screen.width, Screen.height);

		//------------------------------------------------------------------------/
		// Fields
		//------------------------------------------------------------------------/        
		/// <summary>
		/// The style of this window's background
		/// </summary>
		protected GUIStyle backgroundStyle = new GUIStyle();

		//------------------------------------------------------------------------/
		// Interface
		//------------------------------------------------------------------------/
		protected abstract void OnDrawWindow();

		//------------------------------------------------------------------------/
		// Methods
		//------------------------------------------------------------------------/
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="title">The title of this window</param>
		/// <param name="relativeDimensions">The relative dimensions of this window</param>
		/// <param name="color"></param>
		/// <param name="position"></param>
		public AbstractWindow(string title, Vector2 relativeDimensions, Color color, Anchor position = Anchor.TopLeft) : base(title)
		{
			anchor = position;
			dimensions = StratusGUI.CalculateRelativeDimensions(relativeDimensions, screenSize);
		}

		/// <summary>
		/// Draws this window
		/// </summary>
		protected override void OnDraw()
		{
			if (!active)
				return;

			var screenSize = new Vector2(Screen.width, Screen.height);
			UnityEngine.GUILayout.BeginArea(StratusGUI.CalculateAnchoredPositionOnScreen(anchor, dimensions, screenSize), name, GUI.skin.window);
			OnDrawWindow();
			UnityEngine.GUILayout.EndArea();
		}

	}

	/// <summary>
	/// An UI element that contains other elements, which are parented to it
	/// </summary>
	public class Window : AbstractWindow
	{
		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/
		/// <summary>
		/// This window is active whenever there are elements on it
		/// </summary>
		public override bool active { get { return Elements.Count != 0; } }

		//------------------------------------------------------------------------/
		// Fields
		//------------------------------------------------------------------------/
		/// <summary>
		/// All child elements
		/// </summary>
		private List<Element> Elements = new List<Element>();

		/// <summary>
		/// The position of the scrolling bar
		/// </summary>
		protected Vector2 ScrollPosition;

		//------------------------------------------------------------------------/
		// Methods
		//------------------------------------------------------------------------/
		public Window(string name, Vector2 relativeDimensions, Color color, Anchor position = Anchor.TopLeft)
		  : base(name, relativeDimensions, color, position)
		{
		}

		/// <summary>
		/// Adds an element to be drawn inside the window
		/// </summary>
		/// <param name="element"></param>
		public void Add(Element element)
		{
			Elements.Add(element);
		}

		/// <summary>
		/// Removes an element from the window
		/// </summary>
		/// <param name="element"></param>
		public void Remove(Element element)
		{
			Elements.Remove(element);
		}

		/// <summary>
		/// Draws all active elements in the window
		/// </summary>
		protected override void OnDrawWindow()
		{
			//GUILayout.BeginVertical();
			ScrollPosition = UnityEngine.GUILayout.BeginScrollView(ScrollPosition);//, GUILayout.Width(Dimensions.x), GUILayout.Height(Dimensions.y));
			DrawElements();
			UnityEngine.GUILayout.EndScrollView();
			//GUILayout.EndVertical();
		}

		/// <summary>
		/// Draws the elements of the window
		/// </summary>
		protected virtual void DrawElements()
		{
			//var elements = Elements;
			foreach (var element in this.Elements)
				element.Draw();
		}

	}
}