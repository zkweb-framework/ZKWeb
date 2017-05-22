using System;
using System.Collections.Generic;
using ZKWebStandard.Utils;

namespace ZKWebStandard.Extensions {
	/// <summary>
	/// ITreeNode extension methods<br/>
	/// 树节点接口的扩展函数<br/>
	/// </summary>
	public static class ITreeNodeExtensions {
		/// <summary>
		/// Get all parent nodes<br/>
		/// From bottom to top, return empty sequences if node is root<br/>
		/// 获取所有上级节点<br/>
		/// 从下到上, 返回空列表如果节点是根节点<br/>
		/// </summary>
		/// <typeparam name="T">Node value type</typeparam>
		/// <param name="node">The node</param>
		/// <returns></returns>
		/// <example>
		/// <code language="cs">
		/// var elements = new List&lt;TestData&gt;() {
		///		new TestData(1, 0, "A"),
		///		new TestData(2, 1, "AA"),
		///		new TestData(3, 1, "AB"),
		///		new TestData(4, 0, "B"),
		///		new TestData(5, 4, "BA"),
		///		new TestData(6, 4, "BB"),
		///		new TestData(7, 6, "BBB"),
		///	};
		///	var elementsMapping = elements.ToDictionary(e =&gt; e.Id, e =&gt; e);
		///	var tree = TreeUtils.CreateTree(elements,
		///		e =&gt; e, e =&gt; elementsMapping.GetOrDefault(e.ParentId));
		///	
		/// var nodeBBB = tree.EnumerateAllNodes().First(n =&gt; n.Value?.Name == "BBB");
		/// var nodeBBBParents = nodeBBB.GetParents().ToList(); // BB, B, Root
		/// </code>
		/// </example>
		public static IEnumerable<ITreeNode<T>> GetParents<T>(this ITreeNode<T> node) {
			var parent = node.Parent;
			while (parent != null) {
				yield return parent;
				parent = parent.Parent;
			}
		}

		/// <summary>
		/// Visit all nodes recursively<br/>
		/// From top to bottom (DFS)<br/>
		/// 递归访问所有节点<br/>
		/// 从上到下 (DFS)<br/>
		/// </summary>
		/// <typeparam name="T">Node value type</typeparam>
		/// <param name="node">The node</param>
		/// <param name="visit">Visit action</param>
		/// <example>
		/// <code language="cs">
		/// var elements = new List&lt;TestData&gt;() {
		///		new TestData(1, 0, "A"),
		///		new TestData(2, 1, "AA"),
		///		new TestData(3, 1, "AB"),
		///		new TestData(4, 0, "B"),
		///		new TestData(5, 4, "BA"),
		///		new TestData(6, 4, "BB"),
		///		new TestData(7, 6, "BBB"),
		///	};
		///	var elementsMapping = elements.ToDictionary(e =&gt; e.Id, e =&gt; e);
		///	var tree = TreeUtils.CreateTree(elements,
		///		e =&gt; e, e =&gt; elementsMapping.GetOrDefault(e.ParentId));
		/// 
		/// var visitedNames = new List&lt;string&gt;();
		/// tree.VisitAllNodes(n =&gt; visitedNames.Add(n.Value?.Name));
		/// // null, A, "AA", "AB", "B", "BA", "BB", "BBB"
		/// </code>
		/// </example>
		public static void VisitAllNodes<T>(
			this ITreeNode<T> node, Action<ITreeNode<T>> visit) {
			visit(node);
			foreach (var child in node.Childs) {
				child.VisitAllNodes(visit);
			}
		}

		/// <summary>
		/// Enumerate all nodes<br/>
		/// From top to bottom<br/>
		/// 枚举所有节点 (DFS)<br/>
		/// 从上到下 (DFS)<br/>
		/// </summary>
		/// <typeparam name="T">Node value type</typeparam>
		/// <param name="node">The node</param>
		/// <returns></returns>
		/// <example>
		/// <code language="cs">
		/// var elements = new List&lt;TestData&gt;() {
		///		new TestData(1, 0, "A"),
		///		new TestData(2, 1, "AA"),
		///		new TestData(3, 1, "AB"),
		///		new TestData(4, 0, "B"),
		///		new TestData(5, 4, "BA"),
		///		new TestData(6, 4, "BB"),
		///		new TestData(7, 6, "BBB"),
		///	};
		///	var elementsMapping = elements.ToDictionary(e =&gt; e.Id, e =&gt; e);
		///	var tree = TreeUtils.CreateTree(elements,
		///		e =&gt; e, e =&gt; elementsMapping.GetOrDefault(e.ParentId));
		/// 
		/// var allNames = tree.EnumerateAllNodes().Select(n =&gt; n.Value?.Name).ToList();
		/// // null, A, "AA", "AB", "B", "BA", "BB", "BBB"
		/// </code>
		/// </example>
		public static IEnumerable<ITreeNode<T>> EnumerateAllNodes<T>(
			this ITreeNode<T> node) {
			yield return node;
			foreach (var child in node.Childs) {
				foreach (var childOfChild in child.EnumerateAllNodes()) {
					yield return childOfChild;
				}
			}
		}
	}
}
