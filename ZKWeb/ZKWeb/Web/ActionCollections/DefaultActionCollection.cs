using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using ZKWebStandard.Collections;
using ZKWebStandard.Web;

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
		/// { Path: Action Descriptor }
		/// </summary>
		protected HierarchyDictionary<char, List<RegexActionDescriptor>> RegexActions { get; set; }
		/// <summary>
		/// Lock for RegexActions<br/>
		/// RegexActions的线程锁<br/>
		/// </summary>
		protected ReaderWriterLockSlim RegexActionsLock { get; set; }

		/// <summary>
		/// Initialize<br/>
		/// 初始化<br/>
		/// </summary>
		public DefaultActionCollection() {
			Actions = new ConcurrentDictionary<Pair<string, string>, Func<IActionResult>>();
			RegexActions = new HierarchyDictionary<char, List<RegexActionDescriptor>>();
			RegexActionsLock = new ReaderWriterLockSlim();
		}

		/// <summary>
		/// Get base path and regular expression from full path<br/>
		/// If no expression in full path then 'regex' and 'parameters' would be null<br/>
		/// 根据完整路径获取基础路径和正则表达式<br/>
		/// 如果完整路径中无表达式则'regex'和'parameters'会等于null<br/>
		/// </summary>
		protected string ParsePath(string path, out string pattern, out List<string> parameters) {
			var index = path.IndexOf('{');
			if (index >= 0) {
				// Split base path
				var basePath = path.Substring(0, index);
				// Build regular expression
				var patternBuilder = new List<string>(3);
				parameters = new List<string>(3);
				index = 0;
				while (index < path.Length) {
					var braceBegin = path.IndexOf('{', index);
					if (braceBegin >= 0) {
						var braceFinish = path.IndexOf('}', braceBegin);
						if (braceFinish < 0) {
							throw new FormatException($"invalid path: {path}, no '}}' match to '{{'");
						}
						var parameter = path.Substring(braceBegin + 1, braceFinish - braceBegin - 1).Trim();
						if (parameter.Length == 0) {
							throw new FormatException($"invalid path: {path}, no identifier between '{{' and '}}'");
						}
						patternBuilder.Add(Regex.Escape(path.Substring(index, braceBegin - index)));
						patternBuilder.Add("(.+)");
						parameters.Add(parameter);
						index = braceFinish + 1;
					} else {
						patternBuilder.Add(Regex.Escape(path.Substring(index, path.Length - index)));
						index = path.Length;
					}
				}
				pattern = string.Join("", patternBuilder);
				return basePath;
			} else {
				pattern = null;
				parameters = null;
				return path;
			}
		}

		/// <summary>
		/// Associate action with path and method<br/>
		/// 设置路径和方法关联的Action函数<br/>
		/// </summary>
		public void Set(string path, string method, Func<IActionResult> action, bool overrideExists) {
			// Parse path
			var basePath = ParsePath(path, out var pattern, out var parameters);
			// Duplicate check
			var key = Pair.Create(basePath, method);
			if (!overrideExists) {
				bool duplicated = false;
				if (pattern == null) {
					// No expression found, just check the key
					duplicated = Actions.ContainsKey(key);
				} else {
					// Check if there any descriptor have same method and regex
					RegexActionsLock.EnterReadLock();
					try {
						duplicated = RegexActions.TryGetValue(basePath, out var descriptors) &&
							descriptors.Any(d => d.Method == method && d.Pattern == pattern);
					} finally {
						RegexActionsLock.ExitReadLock();
					}
				}
				if (duplicated) {
					throw new ArgumentException(
						$"Action for method '{method}' and path '{path}' already registered, " +
						"please use option `overrideExists` if you want to replace it");
				}
			}
			// Register action
			if (pattern == null) {
				// Normal action
				Actions[key] = action;
			} else {
				// Regex action
				RegexActionsLock.EnterWriteLock();
				try {
					if (!RegexActions.TryGetValue(basePath, out var descriptors)) {
						descriptors = new List<RegexActionDescriptor>();
						RegexActions[basePath] = descriptors;
					}
					// Sort descriptors by pattern length descending
					var regex = new Regex(pattern);
					descriptors.Add(new RegexActionDescriptor(method, pattern, regex, parameters, action));
					descriptors.Sort((x, y) => y.Pattern.Length - x.Pattern.Length);
				} finally {
					RegexActionsLock.ExitWriteLock();
				}
			}
		}

		/// <summary>
		/// Get action associated with path and method<br/>
		/// 获取路径和方法关联的Action函数<br/>
		/// </summary>
		public Func<IActionResult> Get(string path, string method) {
			// Search normal action
			var key = Pair.Create(path, method);
			if (Actions.TryGetValue(key, out var action)) {
				return action;
			}
			// Search regex action
			// Matching from the lonest base path, for example:	
			//	rules: "get/some/{id}", "get/{id}"
			//	"get/some/1" matches "get/some/{id}"
			//	"get/1" matches "get/{id}"
			RegexActionsLock.EnterReadLock();
			try {
				var nodes = RegexActions.Travel(path, false).Where(a => a.HasValue).Reverse();
				foreach (var node in nodes) {
					foreach (var descriptor in node.Value) {
						// Check method first because all methods are using the same list
						if (descriptor.Method != method) {
							continue;
						}
						// Match pattern, descriptors should be sorted by pattern length descennding
						var match = descriptor.Regex.Match(path);
						if (!match.Success) {
							continue;
						}
						// Set custom parameters to request
						var request = HttpManager.CurrentContext.Request;
						for (var i = 0; i < descriptor.Parameters.Count; ++i) {
							var name = descriptor.Parameters[i];
							var value = match.Groups[i + 1].Value;
							request.CustomParameters[name] = value;
						}
						return descriptor.Action;
					}
				}
			} finally {
				RegexActionsLock.ExitReadLock();
			}
			return null;
		}

		/// <summary>
		/// Remove action associated with path and method<br/>
		/// 删除路径和方法关联的Action函数<br/>
		/// </summary>
		public bool Remove(string path, string method) {
			var basePath = ParsePath(path, out var regex, out var parameters);
			if (regex == null) {
				// Normal action
				var key = Pair.Create(basePath, method);
				return Actions.TryRemove(key, out var _);
			} else {
				// Regex action
				RegexActionsLock.EnterReadLock();
				try {
					var pattern = regex.ToString();
					return RegexActions.TryGetValue(basePath, out var descriptors) &&
						descriptors.RemoveAll(d => d.Method == method && d.Regex.ToString() == pattern) > 0;
				} finally {
					RegexActionsLock.ExitReadLock();
				}
			}
		}

		/// <summary>
		/// Regex action descriptor<br/>
		/// 使用正则表达式的Action信息<br/>
		/// </summary>
		protected class RegexActionDescriptor {
			/// <summary>
			/// Http Method<br/>
			/// Http方法<br/>
			/// </summary>
			public string Method { get; }
			/// <summary>
			/// Pattern string used by regex<br/>
			/// 正则表达式使用的字符串<br/>
			/// </summary>
			public string Pattern { get; set; }
			/// <summary>
			/// Regex use to match request path<br/>
			/// Group name is not used, for duplicate detection<br/>
			/// 用于匹配请求路径的正则表达式<br/>
			/// 不使用分组名称, 以检测重复<br/>
			/// </summary>
			public Regex Regex { get; }
			/// <summary>
			/// Parameter name array<br/>
			/// 参数的名称数组<br/>
			/// </summary>
			public List<string> Parameters { get; }
			/// <summary>
			/// Action method<br/>
			/// Action函数<br/>
			/// </summary>
			public Func<IActionResult> Action { get; }

			/// <summary>
			/// Initialize<br/>
			/// 初始化<br/>
			/// </summary>
			public RegexActionDescriptor(
				string method, string pattern, Regex regex, List<string> parameters, Func<IActionResult> action) {
				Method = method;
				Pattern = pattern;
				Regex = regex;
				Parameters = parameters;
				Action = action;
			}
		}
	}
}
