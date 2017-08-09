using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text.RegularExpressions;
using ZKWebStandard.Collections;

namespace ZKWeb.Web.ActionCollections {
	/// <summary>
	/// Default action collection<br/>
	/// Support ordinary path and expression path (eg "get/{id}")<br/>
	/// 默认的Action集合<br/>
	/// 支持普通路径和表达式路径(例如"get/{id}")<br/>
	/// </summary>
	public class DefaultActionCollection : IActionCollection {
		/// <summary>
		/// { (Path, Method): Action }
		/// </summary>
		protected ConcurrentDictionary<Pair<string, string>, Func<IActionResult>> Actions { get; set; }

		/// <summary>
		/// Initialize<br/>
		/// 初始化<br/>
		/// </summary>
		public DefaultActionCollection() {
			Actions = new ConcurrentDictionary<Pair<string, string>, Func<IActionResult>>();
		}

		/// <summary>
		/// "get/{index}" => ("get", "get/(?&lt;index&gt;.+)")
		/// </summary>
		protected Pair<string, Regex> ParsePath(string path) {
			var index = path.IndexOf('{');
			Regex regex = null;
			if (index >= 0) {
				// Split base path
				if (index > 0 && path[index - 1] == '/') {
					--index; // get/{index} => 3
				}
				path = path.Substring(index);
				// Build regex
				throw new NotImplementedException();
			}
			return Pair.Create(path, regex);
		}

		/// <summary>
		/// Associate action with path and method<br/>
		/// 设置路径和方法关联的Action函数<br/>
		/// </summary>
		public void Set(string path, string method, Func<IActionResult> action, bool overrideExists) {
			// Parse path
			var (basePath, regex) = ParsePath(path);
			// Duplicate check
			var key = Pair.Create(basePath, method);
			var pattern = regex?.ToString();
			if (!overrideExists) {
				bool duplicated = false;
				if (regex == null) {
					// No expression found, just check the key
					duplicated = Actions.ContainsKey(key);
				} else {
					// Check if there some descriptor have same regex
					// duplicated = RegexActions.TryGetValue(key, out var descriptors) &&
					// 	descriptors.Any(d => d.Regex.ToString() == pattern);
				}
				if (duplicated) {
					throw new ArgumentException(
						$"Action for method '{method}' and path '{path}' already registered, " +
						"please use option `overrideExists` if you want to replace it");
				}
			}
			// Register base path
			Actions[key] = action;
			// Register regex descriptor

		}

		/// <summary>
		/// Get action associated with path and method<br/>
		/// 获取路径和方法关联的Action函数<br/>
		/// </summary>
		public Func<IActionResult> Get(string path, string method) {
			// Search full path
			var key = Pair.Create(path, method);
			if (Actions.TryGetValue(key, out var action)) {
				return action;
			}
			// Search base path, fast way
			// TODO
			// Search base path, slow way
			// TODO
			return null;
		}

		/// <summary>
		/// Remove action associated with path and method<br/>
		/// 删除路径和方法关联的Action函数<br/>
		/// </summary>
		public bool Remove(string path, string method) {
			var (basePath, regex) = ParsePath(path);
			var key = Pair.Create(basePath, method);
			// TODO, check regex
			return Actions.TryRemove(key, out var _);
		}

		/// <summary>
		/// Regex action descriptor
		/// </summary>
		protected class RegexActionDescriptor {
			/// <summary>
			/// Regex use to match request string
			/// </summary>
			public Regex Regex { get; set; }
			/// <summary>
			/// Parameter name array
			/// </summary>
			public string[] Parameters { get; set; }
			/// <summary>
			/// Action method
			/// </summary>
			public Func<IActionResult> Action { get; set; }
		}
	}
}
