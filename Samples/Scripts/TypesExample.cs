using Stratus.Data;
using Stratus.Unity;
using Stratus.Unity.Data;
using Stratus.Unity.Scenes;

using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Stratus
{
	namespace Examples
	{
		/// <summary>
		/// A simple component showcasing the most useful types provided by the Stratus library
		/// </summary>
		public class TypesExample : StratusBehaviour
		{
			[Header("Stratus Field Types")]
			public SceneField scene = new SceneField();
			public TagField tagField = new TagField();
			public FloatRange floatRange = new FloatRange();
			public StratusIntegerRange intRange = new StratusIntegerRange();
			public BoundedFloat variable = new BoundedFloat();
			public KeyCode enumDrawer;

			public LayerField layer = new LayerField();

			void TryLoadingScene()
			{
				scene.Load(UnityEngine.SceneManagement.LoadSceneMode.Single);
			}

			void TryTag()
			{
				if (gameObject.CompareTag(tagField))
					StratusDebug.Log("The GameObject's tag and selected tag field match! (" + tagField + ")");
			}

			[InvokeMethodAttribute("TryLayer")]
			void TryLayer()
			{
				if (layer.Matches(this.gameObject))
					StratusDebug.Log("The GameObject's layer and selected layer field are a match! (" + layer + ")");
			}


		}
	}

}