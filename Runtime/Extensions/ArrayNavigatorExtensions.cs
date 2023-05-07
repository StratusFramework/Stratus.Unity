﻿using Stratus.Collections;

using UnityEngine;

namespace Stratus.Unity.Extensions
{
	public static class ArrayNavigatorExtensions
	{
		/// <summary>
		/// Navigates using a vector2 where DOWN/RIGHT leads to the next element,
		/// and UP/LEFT to the previous.
		/// </summary>
		/// <param name="dir"></param>
		/// <returns></returns>
		public static T Navigate<T>(this ArrayNavigator<T> navigator, Vector2 dir)
		{
			bool up = dir.y > 0;
			bool down = dir.y < 0;
			bool left = dir.x < 0;
			bool right = dir.x > 0;

			if (down || right)
			{
				if (!left && !up)
				{
					return navigator.Next();
				}
			}
			else if (up || left)
			{
				if (!down && !right)
				{
					return navigator.Previous();
				}
			}
			return navigator.current;
		}
	}
}