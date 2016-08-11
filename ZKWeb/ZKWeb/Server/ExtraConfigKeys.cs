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
		/// Translated text cache time, in seconds
		/// </summary>
		public const string TranslateCacheTime = "ZKWeb.TranslateCacheTime";
		/// <summary>
		/// Template path cache time, in seconds
		/// </summary>
		public const string TemplatePathCacheTime = "ZKWeb.TemplatePathCacheTime";
		/// <summary>
		/// Resource file path cache time, in seconds
		/// </summary>
		public const string ResourcePathCacheTime = "ZKWeb.ResourcePathCacheTime";
		/// <summary>
		/// Widget information cache time, in seconds
		/// </summary>
		public const string WidgetInfoCacheTime = "ZKWeb.WidgetInfoCacheTime";
		/// <summary>
		/// Custom widget list cache time, in seconds
		/// </summary>
		public const string CustomWidgetsCacheTime = "ZKWeb.CustomWidgetsCacheTime";
		/// <summary>
		/// Parsed template object cache time, in seconds
		/// </summary>
		public const string TemplateCacheTime = "ZKWeb.TemplateCacheTime";
		/// <summary>
		/// Display full exception for template rendering
		/// </summary>
		public const string DisplayFullExceptionForTemplate = "ZKWeb.DisplayFullExceptionForTemplate";
		/// <summary>
		/// Dispay full exception for http request
		/// </summary>
		public const string DisplayFullExceptionForRequest = "ZKWeb.DisplayFullExceptionForRequest";
		/// <summary>
		/// Clear cache after used memory more than this value, in MB
		/// </summary>
		public const string ClearCacheAfterUsedMemoryMoreThan = "ZKWeb.ClearCacheAfterUsedMemoryMoreThan";
		/// <summary>
		/// Automatic cache cleaner check interval, in seconds
		/// </summary>
		public const string CleanCacheCheckInterval = "ZKWeb.CleanCacheCheckInterval";
		/// <summary>
		/// Compile plugins with release configuration
		/// Sometimes it will make debug with plugins not work
		/// </summary>
		public const string CompilePluginsWithReleaseConfiguration = "ZKWeb.CompilePluginsWithReleaseConfiguration";
	}
}
