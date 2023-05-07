﻿using Stratus.Extensions;

using System;
using System.Collections.Generic;

using UnityEngine;

namespace Stratus.Unity.Reflection
{
	[Serializable]
	public class ComponentMemberWatchList
	{
		[SerializeField]
		private List<StratusComponentMemberWatchInfo> _members = new List<StratusComponentMemberWatchInfo>();
		public StratusComponentMemberWatchInfo[] members => _members.ToArray();
		private Dictionary<string, StratusComponentMemberWatchInfo> membersByPath
		{
			get
			{
				if (_membersByPath == null)
				{
					_membersByPath = this._members.ToDictionary(m => m.path);
				}
				return _membersByPath;
			}
		}
		private Dictionary<string, StratusComponentMemberWatchInfo> _membersByPath;

		public event Action onUpdated;

		/// <summary>
		/// Returns true if the member of the component is being watched
		/// </summary>
		public bool Contains(IComponentMemberInfo member)
		{
			return membersByPath.ContainsKey(member.path);
		}

		/// <summary>
		/// Adds a member to the watch list
		/// </summary>
		public void Add(ComponentMemberInfo member)
		{
			// Don't add duplicates since we only hold one and 
			// handle duplicate components
			if (!Contains(member))
			{
				var watch = member.ToWatch();
				_members.Add(watch);
				_membersByPath.Add(member.path, watch);
				onUpdated?.Invoke();
			}
		}

		/// <summary>
		/// Removes a member from the watch list
		/// </summary>
		/// <param name="member"></param>
		public void Remove(IComponentMemberInfo member)
		{
			if (Contains(member))
			{
				_members.RemoveAll(m => m.path == member.path);
				_membersByPath.Remove(member.path);
				onUpdated?.Invoke();
			}
		}

		/// <summary>
		/// Adds or removes the watch for the given member
		/// </summary>
		/// <param name="member"></param>
		public void Toggle(ComponentMemberInfo member)
		{
			if (Contains(member))
			{
				Remove(member);
			}
			else
			{
				Add(member);
			}
		}

		/// <summary>
		/// Clears the watch list
		/// </summary>
		public void Clear()
		{
			_members.Clear();
			_membersByPath.Clear();
			onUpdated?.Invoke();
		}
	}
}