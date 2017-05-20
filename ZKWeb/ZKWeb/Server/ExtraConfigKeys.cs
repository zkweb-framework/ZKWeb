namespace ZKWeb.Server {
	/// <summary>
	/// Extra website configuration keys<br/>
	/// 网站的附加配置键<br/>
	/// </summary>
	internal static class ExtraConfigKeys {
		/// <summary>
		/// ORM for temporary database, Default is "InMemory"<br/>
		/// 临时数据库使用的ORM, 默认是"InMemory"<br/>
		/// </summary>
		public const string TemporaryDatabaseORM = "ZKWeb.TemporaryDatabaseORM";
		/// <summary>
		/// Database type for temporary database, Default is empty<br/>
		/// 临时数据库使用的数据库类型, 默认是空白<br/>
		/// </summary>
		public const string TemporaryDatabaseType = "ZKWeb.TemporaryDatabaseType";
		/// <summary>
		/// Connection string for temporary database, Default is empty<br/>
		/// 临时数据库试用的连接字符串, 默认是空白<br/>
		/// </summary>
		public const string TemporaryDatabaseConnectionString = "ZKWeb.TemporaryDatabaseConnectionString";
		/// <summary>
		/// Translated text cache time, in seconds<br/>
		/// 翻译文本的缓存时间, 单位是秒<br/>
		/// </summary>
		public const string TranslateCacheTime = "ZKWeb.TranslateCacheTime";
		/// <summary>
		/// Template path cache time, in seconds<br/>
		/// 模板路径的缓存时间, 单位是秒<br/>
		/// </summary>
		public const string TemplatePathCacheTime = "ZKWeb.TemplatePathCacheTime";
		/// <summary>
		/// Resource file path cache time, in seconds<br/>
		/// 资源路径的缓存时间, 单位是秒<br/>
		/// </summary>
		public const string ResourcePathCacheTime = "ZKWeb.ResourcePathCacheTime";
		/// <summary>
		/// Widget information cache time, in seconds<br/>
		/// 模板模块的信息的缓存时间, 单位是秒<br/>
		/// </summary>
		public const string WidgetInfoCacheTime = "ZKWeb.WidgetInfoCacheTime";
		/// <summary>
		/// Custom widget list cache time, in seconds<br/>
		/// 自定义模块模块列表的缓存时间, 单位是秒<br/>
		/// </summary>
		public const string CustomWidgetsCacheTime = "ZKWeb.CustomWidgetsCacheTime";
		/// <summary>
		/// Parsed template object cache time, in seconds<br/>
		/// 已解析的模板对象的缓存时间, 单位是秒<br/>
		/// </summary>
		public const string TemplateCacheTime = "ZKWeb.TemplateCacheTime";
		/// <summary>
		/// Display full exception for template rendering<br/>
		/// 是否在模板描画发生错误时显示完整的例外信息<br/>
		/// </summary>
		public const string DisplayFullExceptionForTemplate = "ZKWeb.DisplayFullExceptionForTemplate";
		/// <summary>
		/// Dispay full exception for http request<br/>
		/// 是否在http请求发生错误时显示完整的例外信息<br/>
		/// </summary>
		public const string DisplayFullExceptionForRequest = "ZKWeb.DisplayFullExceptionForRequest";
		/// <summary>
		/// Clear cache after used memory more than this value, in MB<br/>
		/// 在使用内存超过这个值时自动清理缓存, 单位是MB<br/>
		/// </summary>
		public const string ClearCacheAfterUsedMemoryMoreThan = "ZKWeb.ClearCacheAfterUsedMemoryMoreThan";
		/// <summary>
		/// Automatic cache cleaner check interval, in seconds<br/>
		/// 自动清理缓存的检查时间间隔, 单位是秒<br/>
		/// </summary>
		public const string CleanCacheCheckInterval = "ZKWeb.CleanCacheCheckInterval";
		/// <summary>
		/// Compile plugins with release configuration<br/>
		/// Sometimes it will make debugging with plugins not work<br/>
		/// 使用发布模式编译插件<br/>
		/// 它会导致有时候不能对插件进行调试<br/>
		/// </summary>
		public const string CompilePluginsWithReleaseConfiguration = "ZKWeb.CompilePluginsWithReleaseConfiguration";
	}
}
