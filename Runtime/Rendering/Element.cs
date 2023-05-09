namespace Stratus.Unity.Rendering
{
	/// <summary>
	/// An UI element of the overlay
	/// </summary>         
	public abstract class Element
	{
		/// <summary>
		/// Whether this element is currently enabled
		/// </summary>
		public bool enabled = true;

		/// <summary>
		/// The name of this element
		/// </summary>
		public string name;

		/// <summary>
		/// Draws this element
		/// </summary>
		protected abstract void OnDraw();

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="name"></param>
		public Element(string name)
		{
			this.name = name;
		}

		/// <summary>
		/// Draws this element
		/// </summary>
		public void Draw()
		{
			if (!enabled)
				return;

			this.OnDraw();
		}
	}
}
