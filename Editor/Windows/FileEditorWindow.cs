using Stratus.Editor;

namespace Stratus.Unity.Editor
{
	public class FileEditorWindow : StratusEditorWindow<FileEditorWindow>
	{
		public static void Open()
		{
			OpenWindow("Stratus Assets", true);
		}


		protected override void OnWindowEnable()
		{

		}

		protected override void OnWindowGUI()
		{

		}
	}
}