using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using ZKWebStandard.Extensions;

namespace ZKWebStandard.Utils {
	/// <summary>
	/// Tree utility functions<br/>
	/// <br/>
	/// </summary>
	public static class TreeUtils {
		/// <summary>
		/// Create tree from elements<br/>
		/// Return root node, root node doesn't have a value<br/>
		/// <br/>
		/// <br/>
		/// </summary>
		/// <typeparam name="T">Element type</typeparam>
		/// <typeparam name="TValue">Value type</typeparam>
		/// <param name="elements">Source elements</param>
		/// <param name="getValue">Method for get value from element</param>
		/// <param name="getParent">Method for get parent element, return null if no parent</param>
		/// <returns></returns>
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
		/// <br/>
		/// <br/>
		/// </summary>
		/// <typeparam name="T">Original type</typeparam>
		/// <typeparam name="U">Target type</typeparam>
		/// <param name="node">The node</param>
		/// <param name="convertor">Method for convert value from original to target</param>
		/// <returns></returns>
		public static ITreeNode<U> Transform<T, U>(ITreeNode<T> node, Func<T, U> convertor) {
			var unode = new TreeNode<U>(convertor(node.Value));
			foreach (var childNode in node.Childs) {
				unode.AddChildNode(Transform(childNode, convertor));
			}
			return unode;
		}

		/// <summary>
		/// Tree node<br/>
		/// <br/>
		/// </summary>
		/// <typeparam name="T">Value type</typeparam>
		class TreeNode<T> : ITreeNode<T> {
			/// <summary>
			/// Value<br/>
			/// <br/>
			/// </summary>
			public T Value { get; set; }
			/// <summary>
			/// Parent node, maybe null<br/>
			/// <br/>
			/// </summary>
			[JsonIgnore]
			public ITreeNode<T> Parent { get; set; }
			/// <summary>
			/// Child nodes<br/>
			/// <br/>
			/// </summary>
			[JsonIgnore]
			List<ITreeNode<T>> ChildList { get; set; }
			/// <summary>
			/// Child nodes, for interface<br/>
			/// <br/>
			/// </summary>
			public IEnumerable<ITreeNode<T>> Childs {
				get { return ChildList; }
			}

			/// <summary>
			/// Initialize<br/>
			/// <br/>
			/// </summary>
			/// <param name="value"></param>
			public TreeNode(T value) {
				Value = value;
				Parent = null;
				ChildList = new List<ITreeNode<T>>();
			}

			/// <summary>
			/// Add child node, throw exception if failed<br/>
			/// <br/>
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
			/// <br/>
			/// </summary>
			public void RemoveChildNode(ITreeNode<T> node) {
				if (node.Parent == this) {
					ChildList.Remove(node);
					node.Parent = null;
				}
			}

			/// <summary>
			/// Serialize tree to json<br/>
			/// <br/>
			/// </summary>
			/// <returns></returns>
			public override string ToString() {
				return JsonConvert.SerializeObject(this, Formatting.Indented);
			}
		}
	}

	/// <summary>
	/// Interface for tree node<br/>
	/// <br/>
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface ITreeNode<T> {
		/// <summary>
		/// Value<br/>
		/// <br/>
		/// </summary>
		T Value { get; set; }
		/// <summary>
		/// Parent node maybe null<br/>
		/// <br/>
		/// </summary>
		[JsonIgnore]
		ITreeNode<T> Parent { get; set; }
		/// <summary>
		/// Child nodes<br/>
		/// <br/>
		/// </summary>
		IEnumerable<ITreeNode<T>> Childs { get; }
		/// <summary>
		/// Add child node, throw exception if failed<br/>
		/// <br/>
		/// </summary>
		/// <param name="node">Node to add</param>
		void AddChildNode(ITreeNode<T> node);
		/// <summary>
		/// Remove child node, do nothing if not found<br/>
		/// <br/>
		/// </summary>
		/// <param name="node">Node to remove</param>
		void RemoveChildNode(ITreeNode<T> node);
	}
}
