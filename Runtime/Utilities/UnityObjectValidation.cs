using Stratus.Extensions;
using Stratus.Unity.Reflection;

using System;
using System.Reflection;

using UnityEngine;

namespace Stratus.Unity.Utility
{
	public class UnityObjectValidation : StratusObjectValidation
	{
		public UnityObjectValidation(Level type, object target, Func<bool> onValidate = null) : base(type, target, onValidate)
		{
		}

		public UnityObjectValidation(string message, Level type, object target, Func<bool> onValidate = null) : base(message, type, target, onValidate)
		{
		}

		public UnityObjectValidation(string message, Level type, Action onSelect, Func<bool> onValidate = null) : base(message, type, onSelect, onValidate)
		{
		}

		public static StratusObjectValidation NullReference(Behaviour behaviour, string description = null)
		{
			FieldInfo[] nullFields = UnityReflection.GetFieldsWithNullReferences(behaviour);
			if (nullFields.IsNullOrEmpty())
				return null;

			string label = behaviour.GetType().Name;
			if (description != null)
				label += $" {description}";

			string msg = $"{label} has the following fields currently set to null:";
			foreach (var f in nullFields)
				msg += $"\n - <i>{f.Name}</i>";
			StratusObjectValidation validation = new StratusObjectValidation(msg, Level.Warning, behaviour);
			return validation;
		}
	}
}