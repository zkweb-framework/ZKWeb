using System;

namespace ZKWeb.Toolkits.ProjectCreator.Model {
	/// <summary>
	/// 创建项目的参数
	/// </summary>
	public class CreateProjectParameters {
		/// <summary>
		/// 储存项目模板的文件夹
		/// 不指定时自动检测
		/// </summary>
		/// <example>D:\\Project\\ZKWeb\\Tools\\Templates</example>
		public string TemplatesDirectory { get; set; }
		/// <summary>
		/// 项目类型
		/// 可以是: AspNet, AspNetCore, Owin
		/// </summary>
		/// <example>AspNet</example>
		public string ProjectType { get; set; }
		/// <summary>
		/// 项目名称
		/// </summary>
		/// <example>ZKWeb.Demo</example>
		public string ProjectName { get; set; }
		/// <summary>
		/// 项目描述
		/// </summary>
		/// <example>Some Description</example>
		public string ProjectDescription { get; set; }
		/// <summary>
		/// 数据库
		/// </summary>
		/// <example>postgresql</example>
		public string Database { get; set; }
		/// <summary>
		/// 连接字符串
		/// </summary>
		/// <example>Server=127.0.0.1;Port=5432;Database=zkweb;User Id=postgres;Password=123456;</example>
		public string ConnectionString { get; set; }
		/// <summary>
		/// 使用默认插件集时，这里是plugin.collection.json的所在路径，否则是空
		/// </summary>
		/// <example>D:\\Projects\\ZKWeb.Plugins\\plugin.collection.json</example>
		public string UseDefaultPlugins { get; set; }
		/// <summary>
		/// 保存项目的文件夹
		/// </summary>
		public string OutputDirectory { get; set; }

		/// <summary>
		/// 检查参数
		/// </summary>
		public void Check() {
			if (ProjectType != "AspNet" && ProjectType != "AspNetCore" && ProjectType != "Owin") {
				throw new ArgumentException("Project Type must be one of AspNet, AspNetCore, Owin");
			} else if (string.IsNullOrEmpty(ProjectName)) {
				throw new ArgumentException("Project Name can't be empty");
			} else if (Database != "mssql" && Database != "mysql" &&
				Database != "postgresql" && Database != "sqlite") {
				throw new ArgumentException("Database must be one of mssql, mysql, postgresql, sqlite");
			} else if (string.IsNullOrEmpty(ConnectionString)) {
				throw new ArgumentException("Connection String can't be empty");
			} else if (string.IsNullOrEmpty(OutputDirectory)) {
				throw new ArgumentException("Output Directory can't be empty");
			}
		}
	}
}
