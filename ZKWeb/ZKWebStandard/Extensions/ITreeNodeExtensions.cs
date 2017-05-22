using System;
using System.Collections.Generic;
using ZKWebStandard.Utils;

namespace ZKWebStandard.Extensions {
	/// <summary>
	/// ITreeNode extension methods<br/>
	/// <br/>
	/// </summary>
	public static class ITreeNodeExtensions {
		/// <summary>
		/// Get all parent nodes<br/>
		/// From bottom to top, return empty sequences if node is root<br/>
		/// <br/>
		/// <br/>
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
		/// From top to bottom<br/>
		/// <br/>
		/// <br/>
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
		/// <br/>
		/// <br/>
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
