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
