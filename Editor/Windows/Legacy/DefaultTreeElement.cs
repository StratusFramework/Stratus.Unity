using Stratus.Models.Graph;
using Stratus.Models;

namespace Stratus.Unity.Editor
{
	public class DefaultTreeElement : TreeElement
	{
		public string value;
		public LabeledAction[] actions;

		public DefaultTreeElement()
		{
		}

		public DefaultTreeElement(string name, string value, params LabeledAction[] actions)
		{
			this.name = name;
			this.value = value;
			this.actions = actions;
		}

		public DefaultTreeElement(string name, string value, int depth, int id)
			: this(name, depth, id)
		{
			this.value = value;
		}

		public DefaultTreeElement(string name, int depth, int id) : base(name, depth, id)
		{
		}
	}

}