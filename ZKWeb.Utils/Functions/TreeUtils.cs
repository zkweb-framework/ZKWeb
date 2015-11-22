using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZKWeb.Utils.Extensions;

namespace ZKWeb.Utils.Functions {
	/// <summary>
	/// 树形结构的帮助类
	/// </summary>
	public static class TreeUtils {
		/// <summary>
		/// 创建树形结构
		/// 返回根节点，根节点会自动创建且值是类型的默认值
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="elements">用于构建树的元素列表</param>
		/// <param name="getValue">获取储存到节点中的数据的函数</param>
		/// <param name="getParent">获取上级元素的函数，返回null时上级元素是根节点</param>
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
		/// 树节点
		/// </summary>
		/// <typeparam name="T"></typeparam>
		class TreeNode<T> : ITreeNode<T> {
			/// <summary>
			/// 树中储存的值
			/// </summary>
			public T Value { get; set; }
			/// <summary>
			/// 上级节点，根节点时等于null
			/// </summary>
			[JsonIgnore]
			public ITreeNode<T> Parent { get; set; }
			/// <summary>
			/// 下级节点的列表
			/// </summary>
			[JsonIgnore]
			List<ITreeNode<T>> ChildList { get; set; }
			/// <summary>
			/// 下级节点的列表，不能修改
			/// </summary>
			public IEnumerable<ITreeNode<T>> Childs {
				get { return this.ChildList; }
			}

			/// <summary>
			/// 初始化
			/// </summary>
			/// <param name="value"></param>
			public TreeNode(T value) {
				this.Value = value;
				this.Parent = null;
				this.ChildList = new List<ITreeNode<T>>();
			}

			/// <summary>
			/// 添加子节点，失败时抛出例外
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
				this.ChildList.Add(node);
				node.Parent = this;
			}

			/// <summary>
			/// 删除子节点，没有找到对应的子节点时不抛出例外
			/// </summary>
			public void RemoveChildNode(ITreeNode<T> node) {
				if (node.Parent == this) {
					this.ChildList.Remove(node);
					node.Parent = null;
				}
			}

			/// <summary>
			/// 返回显示树结构的字符串
			/// </summary>
			/// <returns></returns>
			public override string ToString() {
				return JsonConvert.SerializeObject(this, Formatting.Indented);
			}
		}
	}

	/// <summary>
	/// 树节点
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface ITreeNode<T> {
		/// <summary>
		/// 树中储存的值
		/// </summary>
		T Value { get; set; }
		/// <summary>
		/// 上级节点，根节点时等于null
		/// 注意必须标记JsonIgnore否则会导致序列化时出现死循环
		/// </summary>
		[JsonIgnore]
		ITreeNode<T> Parent { get; set; }
		/// <summary>
		/// 子节点的列表
		/// </summary>
		IEnumerable<ITreeNode<T>> Childs { get; }
		/// <summary>
		/// 添加子节点，失败时抛出例外
		/// </summary>
		/// <param name="node">添加的节点</param>
		void AddChildNode(ITreeNode<T> node);
		/// <summary>
		/// 删除子节点，没有找到对应的子节点时不抛出例外
		/// </summary>
		/// <param name="node">删除的节点</param>
		void RemoveChildNode(ITreeNode<T> node);
	}
}
