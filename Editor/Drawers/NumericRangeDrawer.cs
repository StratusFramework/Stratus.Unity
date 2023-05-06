using Stratus.Data;

using UnityEditor;

namespace Stratus.Unity.Editor
{
	[CustomPropertyDrawer(typeof(FloatRange))]
	public class FloatRangeDrawer : DualPropertyDrawer
	{
		protected override string firstProperty { get; } = nameof(FloatRange.minimum);
		protected override string secondProperty { get; } = nameof(FloatRange.maximum);
	}

	[CustomPropertyDrawer(typeof(StratusIntegerRange))]
	public class IntegerRangeDrawer : DualPropertyDrawer
	{
		protected override string firstProperty { get; } = nameof(StratusIntegerRange.minimum);
		protected override string secondProperty { get; } = nameof(StratusIntegerRange.maximum);
	}

}