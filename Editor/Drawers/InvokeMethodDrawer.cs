//using UnityEngine;
//using Stratus.Unity;

//using UnityEditor;
//using System.Reflection;

//namespace Stratus
//{
//	[CustomPropertyDrawer(typeof(InvokeMethodAttribute))]
//	public class InvokeButtonDrawer : PropertyDrawer
//	{
//		private float padding => 2f;

//		//public static Rect CalculateAligned(float width, height)

//		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//		{
//			InvokeMethodAttribute buttonAttribute = property. (this.attribute as InvokeMethodAttribute);

//			// Check if the button is supposed to be enabled right now
//			if (EditorApplication.isPlaying && !buttonAttribute.isActiveAtRuntime)
//			{
//				GUI.enabled = false;
//			}

//			if (!EditorApplication.isPlaying && !buttonAttribute.isActiveInEditor)
//			{
//				GUI.enabled = false;
//			}

//			// Figure out where were drawing the button
//			////var buttonText = "Invoke"; // '" + buttonAttribute.label + "'";
//			float width = position.width; // * 0.65f;
//			float x = position.x; // + (position.width - width);
//			float height = position.height - EditorGUIUtility.standardVerticalSpacing - this.padding * 2;
//			float y = position.y + this.padding;
//			Rect pos = new Rect(x, y, width, height);

//			// A label to the left of the button
//			//string buttonLabel = $"Invoke [" + buttonAttribute.label + "]";
//			//GUI.Label(new Rect(position.x, y, position.width, height), buttonLabel);

//			//GUILayout.Label(buttonAttribute.label);
//			if (GUI.Button(pos, $"Invoke {buttonAttribute.methodName}", EditorStyles.miniButton))
//			//if (GUI.Button(pos, buttonText, EditorStyles.miniButton))
//			{
//				this.attribute.
//				// Look for the method in the specified class
//				MethodInfo mInfo = buttonAttribute.type.GetMethod(buttonAttribute.methodName,
//				  BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
//				if (mInfo == null)
//				{
//					throw new System.Exception(buttonAttribute.missingMethodMessage);
//				}

//				// Make sure that the right component is present
//				Component component = Selection.activeGameObject.GetComponent(buttonAttribute.type);
//				if (component == null)
//				{
//					throw new System.Exception("The component of type " + buttonAttribute.type.Name + " is missing from the selected GameObject");
//				}

//				// We can now safely invoke the method on the component
//				mInfo.Invoke(component, null);
//			}

//			// make sure the GUI is enabled when were done!
//			GUI.enabled = true;
//		}

//		public override float GetHeight()
//		{
//			return (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing * 2) + this.padding;
//		}
//	}
//}