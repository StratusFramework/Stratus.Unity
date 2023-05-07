using System;

using UnityEngine;

namespace Stratus.Unity.Data
{
	/// <summary>
	/// Used to select a target position
	/// </summary>
	[Serializable]
	public class PositionField
	{
		public enum Type
		{
			Vector,
			Transform,
		}

		[SerializeField] private Type type;
		[SerializeField] private Transform transform;
		[SerializeField] private Vector3 point;

		public const string transformFieldName = nameof(transform);
		public const string pointFieldName = nameof(point);

		public Vector3 value
		{
			get
			{
				if (type == Type.Transform && transform != null)
				{
					return transform.position;
				}
				return point;
			}
		}

		public PositionField()
		{
		}

		public PositionField(Vector3 point)
		{
			this.point = point;
			this.type = Type.Vector;
		}

		public PositionField(Transform transform)
		{
			this.transform = transform;
			this.type = Type.Transform;
		}

		public static implicit operator Vector3(PositionField positionField)
		{
			return positionField.value;
		}

		public void Set(Vector3 point)
		{
			this.point = point;
			this.type = Type.Vector;
		}

		public void Set(Transform transform)
		{
			this.transform = transform;
			this.type = Type.Transform;
		}

		public override string ToString()
		{
			if (type == Type.Transform && transform)
				return $"Position = {transform.name} {transform.position}";
			else if (type == Type.Vector)
				return $"Position = {point}";

			return base.ToString();
		}
	}

}