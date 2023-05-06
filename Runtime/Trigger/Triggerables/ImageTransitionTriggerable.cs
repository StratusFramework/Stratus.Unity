using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

namespace Stratus.Unity.Triggers
{
	public class ImageTransitionTriggerable : TriggerableBehaviour
	{
		public enum DurationType { Each, Total }
		public enum ImageSourceType { Scene, Asset }

		//--------------------------------------------------------------------------------------------/
		// Fields
		//--------------------------------------------------------------------------------------------/
		[Header("Image")]
		public ImageSourceType sourceType = ImageSourceType.Asset;
		public Transform imagesParent;

		public Image display;
		public List<Texture2D> images;

		[Header("Control")]
		public float duration = 1.5f;
		[Tooltip("Whether this is the total duration or for each slide")]
		public DurationType type = DurationType.Total;


		private Queue<Transform> queue = new Queue<Transform>();

		//--------------------------------------------------------------------------------------------/
		// Messages
		//--------------------------------------------------------------------------------------------/
		protected override void OnAwake()
		{
			switch (sourceType)
			{
				case ImageSourceType.Scene:
					break;
				case ImageSourceType.Asset:
					break;
				default:
					break;
			}
		}

		protected override void OnReset()
		{

		}

		protected override void OnTrigger(object data = null)
		{

		}

		private void Advance()
		{
			var nextSlide = queue.Dequeue();
			nextSlide.gameObject.SetActive(true);

		}

		private void AddImages()
		{
			// Add all children
			foreach (var child in imagesParent.Children())
			{
				queue.Enqueue(child.transform);
				var graphical = child.gameObject.GetComponent<Graphic>();
				graphical.color = Color.clear;
			}
		}



	}

}