using System;
using System.Collections.Generic;
using ZKWeb.Cache;
using ZKWeb.Storage;

namespace ZKWeb.Server {
	/// <summary>
	/// This class should no longer be using
	/// Please use IFileStorage or LocalPathManager
	/// Obsleted in 1.0.2
	/// </summary>
	[Obsolete("This class should no longer be using, please use IFileStorage or LocalPathManager")]
	public class PathManager : ICacheCleaner {
#pragma warning disable CS1591
		private LocalPathManager Manager { get { return Application.Ioc.Resolve<LocalPathManager>(); } }
		public TimeSpan TemplatePathCacheTime { get { return Manager.TemplatePathCacheTime; } }
		public TimeSpan ResourcePathCacheTime { get { return Manager.ResourcePathCacheTime; } }
		public virtual IList<string> GetPluginDirectories() {
			return Manager.GetPluginDirectories();
		}
		public virtual IEnumerable<string> GetTemplateFullPathCandidates(string path) {
			return Manager.GetTemplateFullPathCandidates(path);
		}
		public virtual string GetTemplateFullPath(string path) {
			return Manager.GetTemplateFullPath(path);
		}
		public virtual IEnumerable<string> GetResourceFullPathCandidates(params string[] pathParts) {
			return Manager.GetResourceFullPathCandidates(pathParts);
		}
		public virtual string GetResourceFullPath(params string[] pathParts) {
			return Manager.GetResourceFullPath(pathParts);
		}
		public virtual string GetStorageFullPath(params string[] pathParts) {
			return Manager.GetStorageFullPath(pathParts);
		}
		public virtual void ClearCache() { Manager.ClearCache(); }
#pragma warning restore CS1591
	}
}
