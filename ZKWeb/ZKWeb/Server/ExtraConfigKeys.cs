namespace ZKWeb.Server {
	/// <summary>
	/// Extra website configuration keys
	/// </summary>
	internal static class ExtraConfigKeys {
		/// <summary>
		/// ORM for temporary database, Default is "InMemory"
		/// </summary>
		public const string TemporaryDatabaseORM = "ZKWeb.TemporaryDatabaseORM";
		/// <summary>
		/// Database type for temporary database, Default is empty
		/// </summary>
		public const string TemporaryDatabaseType = "ZKWeb.TemporaryDatabaseType";
		/// <summary>
		/// Connection string for temporary database, Default is empty
		/// </summary>
		public const string TemporaryDatabaseConnectionString = "ZKWeb.TemporaryDatabaseConnectionString";
		/// <summary>
		/// 翻译的缓存时间，单位是秒
		/// </summary>
		public const string TranslateCacheTime = "ZKWeb.TranslateCacheTime";
		/// <summary>
		/// 模板路径缓存时间，单位是秒
		/// </summary>
		public const string TemplatePathCacheTime = "ZKWeb.TemplatePathCacheTime";
		/// <summary>
		/// 资源路径的缓存时间，单位是秒
		/// </summary>
		public const string ResourcePathCacheTime = "ZKWeb.ResourcePathCacheTime";
		/// <summary>
		/// 模块信息的缓存时间，单位是秒
		/// </summary>
		public const string WidgetInfoCacheTime = "ZKWeb.WidgetInfoCacheTime";
		/// <summary>
		/// 自定义模块列表的缓存时间，单位是秒
		/// </summary>
		public const string CustomWidgetsCacheTime = "ZKWeb.CustomWidgetsCacheTime";
		/// <summary>
		/// 模板的缓存时间，单位是秒
		/// </summary>
		public const string TemplateCacheTime = "ZKWeb.TemplateCacheTime";
		/// <summary>
		/// 是否在描画模板发生例外时显示完整信息
		/// </summary>
		public const string DisplayFullExceptionForTemplate = "ZKWeb.DisplayFullExceptionForTemplate";
		/// <summary>
		/// 是否在请求发生例外时显示完整信息
		/// </summary>
		public const string DisplayFullExceptionForRequest = "ZKWeb.DisplayFullExceptionForRequest";
		/// <summary>
		/// 内存占用超过此数值时自动清理缓存，单位是MB
		/// </summary>
		public const string ClearCacheAfterUsedMemoryMoreThan = "ZKWeb.ClearCacheAfterUsedMemoryMoreThan";
		/// <summary>
		/// 缓存自动清理器的检查间隔，单位是秒
		/// </summary>
		public const string CleanCacheCheckInterval = "ZKWeb.CleanCacheCheckInterval";
		/// <summary>
		/// 使用Release配置编译插件，启用时将不能调试插件
		/// </summary>
		public const string CompilePluginsWithReleaseConfiguration = "ZKWeb.CompilePluginsWithReleaseConfiguration";
	}
}
