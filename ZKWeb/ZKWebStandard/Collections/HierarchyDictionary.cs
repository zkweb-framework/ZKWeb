using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ZKWebStandard.Collections {
	/// <summary>
	/// Dictionary use to search hierarchy key(such as path)<br/>
	/// Not thread safe<br/>
	/// 用于搜索有层级关系的键(例如路径)的词典<br/>
	/// 非线程安全<br/>
	/// </summary>
	/// <typeparam name="TKeyElement"></typeparam>
	/// <typeparam name="TValue"></typeparam>
	public class HierarchyDictionary<TKeyElement, TValue> : IDictionary<IEnumerable<TKeyElement>, TValue> {
		/// <summary>
		/// Key of this node, default value if this is the root<br/>
		/// 这个节点对应的键, 如果是根节点则为默认值<br/>
		/// </summary>
		public TKeyElement KeyElement { get; protected set; }
		/// <summary>
		/// Parent node, null if this is the root<br/>
		/// 上级节点, 如果是根节点则为null<br/>
		/// </summary>
		public HierarchyDictionary<TKeyElement, TValue> Parent { get; protected set; }
		/// <summary>
		/// Is this node has value?<br/>
		/// 当前节点是否有值<br/>
		/// </summary>
		public bool HasValue { get; protected set; }
		/// <summary>
		/// Value of this node<br/>
		/// 当前节点的值<br/>
		/// </summary>
		public TValue Value { get; protected set; }
		/// <summary>
		/// Child nodes<br/>
		/// 子节点<br/>
		/// </summary>
		public IDictionary<TKeyElement, HierarchyDictionary<TKeyElement, TValue>> Childs { get; protected set; }

		/// <summary>
		/// Initialize<br/>
		/// 初始化<br/>
		/// </summary>
		public HierarchyDictionary() {
		}

		/// <summary>
		/// Initialize with parent node<br/>
		/// 指定上级节点的初始化<br/>
		/// </summary>
		public HierarchyDictionary(TKeyElement keyElement, HierarchyDictionary<TKeyElement, TValue> parent) {
			KeyElement = keyElement;
			Parent = parent;
		}

		/// <summary>
		/// Get key of this node<br/>
		/// 获取这个节点对应的键<br/>
		/// </summary>
		/// <returns></returns>
		public IEnumerable<TKeyElement> GetKey() {
			var key = new List<TKeyElement>();
			var node = this;
			while (node.Parent != null) {
				key.Add(node.KeyElement);
				node = node.Parent;
			}
			key.Reverse();
			return key;
		}

		/// <summary>
		/// Travel nodes by the key elements<br/>
		/// 根据键里的元素探索节点<br/>
		/// </summary>
		/// <param name="key">Key elements</param>
		/// <param name="createChildsIfNotExist">Create child node if not exist</param>
		/// <returns></returns>
		public IEnumerable<HierarchyDictionary<TKeyElement, TValue>> Travel(
			IEnumerable<TKeyElement> key, bool createChildsIfNotExist) {
			var node = this;
			yield return node;
			foreach (var element in key) {
				if (node.Childs == null || !node.Childs.TryGetValue(element, out var childNode)) {
					if (createChildsIfNotExist) {
						if (node.Childs == null) {
							node.Childs = new Dictionary<TKeyElement, HierarchyDictionary<TKeyElement, TValue>>();
						}
						childNode = new HierarchyDictionary<TKeyElement, TValue>(element, node);
						node.Childs[element] = childNode;
					} else {
						break;
					}
				}
				node = childNode;
				yield return node;
			}
		}

		/// <summary>
		/// Travel nodes by the key elements, return the last node if found or created<br/>
		/// 根据键里的元素探索节点, 返回找到或者创建的最后一个节点<br/>
		/// </summary>
		/// <param name="key">Key elements</param>
		/// <param name="createChildsIfNotExist">Create child node if not exist</param>
		/// <returns></returns>
		public HierarchyDictionary<TKeyElement, TValue> TravelLast(
			IEnumerable<TKeyElement> key, bool createChildsIfNotExist) {
			var node = this;
			foreach (var element in key) {
				if (node.Childs == null || !node.Childs.TryGetValue(element, out var childNode)) {
					if (createChildsIfNotExist) {
						if (node.Childs == null) {
							node.Childs = new Dictionary<TKeyElement, HierarchyDictionary<TKeyElement, TValue>>();
						}
						childNode = new HierarchyDictionary<TKeyElement, TValue>(element, node);
						node.Childs[element] = childNode;
					} else {
						return null;
					}
				}
				node = childNode;
			}
			return node;
		}

#pragma warning disable CS1591
		public TValue this[IEnumerable<TKeyElement> key] {
			get {
				var node = TravelLast(key, false);
				if (node == null || !node.HasValue) {
					throw new KeyNotFoundException($"key [{string.Join(", ", key)}] not found");
				}
				return node.Value;
			}
			set {
				var node = TravelLast(key, true);
				node.Value = value;
				node.HasValue = true;
			}
		}

		public ICollection<IEnumerable<TKeyElement>> Keys {
			get {
				return this.Select(pair => pair.Key).ToList();
			}
		}

		public ICollection<TValue> Values {
			get {
				return this.Select(pair => pair.Value).ToList();
			}
		}

		public int Count {
			get {
				return this.Select(pair => pair).Count();
			}
		}

		public bool IsReadOnly { get { return false; } }

		public void Add(IEnumerable<TKeyElement> key, TValue value) {
			var node = TravelLast(key, true);
			if (node.HasValue) {
				throw new ArgumentException($"key [{string.Join(", ", key)}] already exists");
			}
			node.Value = value;
			node.HasValue = true;
		}

		public void Add(KeyValuePair<IEnumerable<TKeyElement>, TValue> item) {
			Add(item.Key, item.Value);
		}

		public void Clear() {
			HasValue = false;
			Value = default(TValue);
			Childs = null;
		}

		public bool Contains(KeyValuePair<IEnumerable<TKeyElement>, TValue> item) {
			return ContainsKey(item.Key);
		}

		public bool ContainsKey(IEnumerable<TKeyElement> key) {
			var node = TravelLast(key, false);
			return node != null && node.HasValue;
		}

		public void CopyTo(KeyValuePair<IEnumerable<TKeyElement>, TValue>[] array, int arrayIndex) {
			foreach (var pair in this.Select(pair => pair)) {
				array[arrayIndex++] = pair;
			}
		}

		public IEnumerator<KeyValuePair<IEnumerable<TKeyElement>, TValue>> GetEnumerator() {
			var nodes = new[] { this }.AsEnumerable();
			var childs = nodes;
			while (true) {
				childs = childs.Where(a => a.Childs != null).SelectMany(a => a.Childs.Values);
				if (childs.Any()) {
					nodes = nodes.Concat(childs);
				} else {
					break;
				}
			}
			nodes = nodes.Where(a => a.HasValue);
			return nodes
				.Select(a => new KeyValuePair<IEnumerable<TKeyElement>, TValue>(a.GetKey(), a.Value))
				.GetEnumerator();
		}

		public bool Remove(IEnumerable<TKeyElement> key) {
			var node = this;
			foreach (var element in key) {
				if (node.Childs == null || !node.Childs.TryGetValue(element, out node)) {
					break;
				}
			}
			if (node != null) {
				node.HasValue = false;
				node.Value = default(TValue);
				// Clean up orphans
				while (!node.HasValue && (node.Childs == null || node.Childs.Count == 0)) {
					var parent = node.Parent;
					parent.Childs.Remove(node.KeyElement);
					node = parent;
				}
				return true;
			}
			return false;
		}

		public bool Remove(KeyValuePair<IEnumerable<TKeyElement>, TValue> item) {
			return Remove(item.Key);
		}

		public bool TryGetValue(IEnumerable<TKeyElement> key, out TValue value) {
			var node = TravelLast(key, false);
			if (node != null && node.HasValue) {
				value = node.Value;
				return true;
			} else {
				value = default(TValue);
				return false;
			}
		}

		IEnumerator IEnumerable.GetEnumerator() {
			var nodes = new[] { this }.AsEnumerable();
			nodes = nodes.Concat(nodes.Where(a => a.Childs != null).SelectMany(a => a.Childs.Values));
			return nodes.Where(a => a.HasValue).Select(a => a.Value).GetEnumerator();
		}
#pragma warning restore CS1591
	}
}
