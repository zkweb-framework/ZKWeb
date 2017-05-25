using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using ZKWebStandard.Extensions;

namespace ZKWebStandard.Utils {
	/// <summary>
	/// Tree utility functions<br/>
	/// 树的工具函数<br/>
	/// </summary>
	public static class TreeUtils {
		/// <summary>
		/// Create tree from elements<br/>
		/// Return root node, root node doesn't have a value<br/>
		/// 根据元素列表创建树<br/>
		/// 返回根节点, 根节点不包含值<br/>
		/// </summary>
		/// <typeparam name="T">Element type</typeparam>
		/// <typeparam name="TValue">Value type</typeparam>
		/// <param name="elements">Source elements</param>
		/// <param name="getValue">Method for get value from element</param>
		/// <param name="getParent">Method for get parent element, return null if no parent</param>
		/// <returns></returns>
		/// <example>
		/// <code language="cs">
		/// var elements = new List&lt;TestData&gt;() {
		/// 	new TestData(1, 0, "A"),
		/// 	new TestData(2, 1, "AA"),
		/// 	new TestData(3, 1, "AB"),
		/// 	new TestData(4, 0, "B"),
		/// 	new TestData(5, 4, "BA"),
		/// 	new TestData(6, 4, "BB"),
		/// 	new TestData(7, 6, "BBB"),
		/// };
		/// var elementsMapping = elements.ToDictionary(e =&gt; e.Id, e =&gt; e);
		/// var tree = TreeUtils.CreateTree(elements,
		/// 	e =&gt; e, e =&gt; elementsMapping.GetOrDefault(e.ParentId));
		/// </code>
		/// </example>
		public static ITreeNode<TValue> CreateTree<T, TValue>(
			IEnumerable<T> elements, Func<T, TValue> getValue, Func<T, T> getParent) {
			var root = new TreeNode<TValue>(default(TValue));
			var nodes = elements.ToDictionary(e => e, e => new TreeNode<TValue>(getValue(e)));
			foreach (var element in elements) {
				var node = nodes[element];
				var parent = nodes.GetOrDefault(getParent(element), root);
				parent.AddChildNode(node);
			}
			return root;
		}

		/// <summary>
		/// Transform tree<br/>
		/// If the given node isn't root node, it will lose it's parent<br/>
		/// 转换树<br/>
		/// 如果传入的节点不是根节点, 会失去它的所有上级节点<br/>
		/// </summary>
		/// <typeparam name="T">Original type</typeparam>
		/// <typeparam name="U">Target type</typeparam>
		/// <param name="node">The node</param>
		/// <param name="convertor">Method for convert value from original to target</param>
		/// <returns></returns>
		/// <example>
		/// <code language="cs">
		/// var elements = new List&lt;TestData&gt;() {
		/// 	new TestData(1, 0, "A"),
		/// 	new TestData(2, 1, "AA"),
		/// 	new TestData(3, 1, "AB"),
		/// 	new TestData(4, 0, "B"),
		/// 	new TestData(5, 4, "BA"),
		/// 	new TestData(6, 4, "BB"),
		/// 	new TestData(7, 6, "BBB"),
		/// };
		/// var elementsMapping = elements.ToDictionary(e =&gt; e.Id, e =&gt; e);
		/// var tree = TreeUtils.CreateTree(elements,
		/// 	e =&gt; e, e =&gt; elementsMapping.GetOrDefault(e.ParentId));
		/// var newTree = TreeUtils.Transform(TestData.GetTestTree(), d =&gt; d?.Name);
		/// </code>
		/// </example>
		public static ITreeNode<U> Transform<T, U>(ITreeNode<T> node, Func<T, U> convertor) {
			var unode = new TreeNode<U>(convertor(node.Value));
			foreach (var childNode in node.Childs) {
				unode.AddChildNode(Transform(childNode, convertor));
			}
			return unode;
		}

		/// <summary>
		/// Tree node<br/>
		/// 树节点<br/>
		/// </summary>
		/// <typeparam name="T">Value type</typeparam>
		class TreeNode<T> : ITreeNode<T> {
			/// <summary>
			/// Value<br/>
			/// 值<br/>
			/// </summary>
			public T Value { get; set; }
			/// <summary>
			/// Parent node, maybe null<br/>
			/// 上级节点, 可能是null<br/>
			/// </summary>
			[JsonIgnore]
			public ITreeNode<T> Parent { get; set; }
			/// <summary>
			/// Child nodes<br/>
			/// 子节点列表<br/>
			/// </summary>
			[JsonIgnore]
			List<ITreeNode<T>> ChildList { get; set; }
			/// <summary>
			/// Child nodes, for interface<br/>
			/// 子节点列表, 接口用<br/>
			/// </summary>
			public IEnumerable<ITreeNode<T>> Childs {
				get { return ChildList; }
			}

			/// <summary>
			/// Initialize<br/>
			/// 初始化<br/>
			/// </summary>
			/// <param name="value"></param>
			public TreeNode(T value) {
				Value = value;
				Parent = null;
				ChildList = new List<ITreeNode<T>>();
			}

			/// <summary>
			/// Add child node, throw exception if failed<br/>
			/// 添加子节点, 失败时抛出例外<br/>
			/// </summary>
			public void AddChildNode(ITreeNode<T> node) {
				if (node == null) {
					throw new ArgumentNullException("node can't be null");
				} else if (this == node) {
					throw new ArgumentException("can't add node to itself");
				} else if (this.GetParents().Contains(node)) {
					throw new ArgumentException("can't add node to it's child");
				} else if (node.Parent != null) {
					throw new ArgumentException("node already has a parent");
				}
				ChildList.Add(node);
				node.Parent = this;
			}

			/// <summary>
			/// Remove child node, do nothing if not found<br/>
			/// 删除子节点, 不存在时忽略<br/>
			/// </summary>
			public void RemoveChildNode(ITreeNode<T> node) {
				if (node.Parent == this) {
					ChildList.Remove(node);
					node.Parent = null;
				}
			}

			/// <summary>
			/// Serialize tree to json<br/>
			/// 序列化树到json<br/>
			/// </summary>
			/// <returns></returns>
			public override string ToString() {
				return JsonConvert.SerializeObject(this, Formatting.Indented);
			}
		}
	}

	/// <summary>
	/// Interface for tree node<br/>
	/// 树节点的接口<br/>
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface ITreeNode<T> {
		/// <summary>
		/// Value<br/>
		/// 值<br/>
		/// </summary>
		T Value { get; set; }
		/// <summary>
		/// Parent node, maybe null<br/>
		/// 上级节点, 可能是null<br/>
		/// </summary>
		[JsonIgnore]
		ITreeNode<T> Parent { get; set; }
		/// <summary>
		/// Child nodes<br/>
		/// 子节点列表<br/>
		/// </summary>
		IEnumerable<ITreeNode<T>> Childs { get; }
		/// <summary>
		/// Add child node, throw exception if failed<br/>
		/// 添加子节点, 失败时抛出例外<br/>
		/// </summary>
		/// <param name="node">Node to add</param>
		void AddChildNode(ITreeNode<T> node);
		/// <summary>
		/// Remove child node, do nothing if not found<br/>
		/// 删除子节点, 不存在时忽略<br/>
		/// </summary>
		/// <param name="node">Node to remove</param>
		void RemoveChildNode(ITreeNode<T> node);
	}
}
