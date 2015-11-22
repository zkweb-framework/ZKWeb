using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZKWeb.Utils.Functions;

namespace ZKWeb.Utils.Extensions {
	/// <summary>
	/// 树节点的扩展函数
	/// </summary>
	public static class ITreeNodeExtensions {
		/// <summary>
		/// 获取节点的所有上级节点
		/// 顺序从下到上，根节点时返回空序列
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="node">获取的节点</param>
		/// <returns></returns>
		public static IEnumerable<ITreeNode<T>> GetParents<T>(this ITreeNode<T> node) {
			var parent = node.Parent;
			while (parent != null) {
				yield return parent;
				parent = parent.Parent;
			}
		}

		/// <summary>
		/// 递归访问所有节点
		/// 保证顺序由根节点开始，从上到下
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="node">根节点</param>
		/// <param name="visit">访问函数</param>
		public static void VisitAllNodes<T>(
			this ITreeNode<T> node, Action<ITreeNode<T>> visit) {
			visit(node);
			foreach (var child in node.Childs) {
				child.VisitAllNodes(visit);
			}
		}

		/// <summary>
		/// 枚举返回所有节点
		/// 保证顺序由根节点开始，从上到下
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="node">根节点</param>
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
