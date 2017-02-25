using System;
using System.Collections.Generic;
using System.Linq;

namespace ZKWeb.Toolkits.ProjectCreator.Model {
	/// <summary>
	/// Create project parameters
	/// </summary>
	public class CreateProjectParameters {
		/// <summary>
		/// Project template directory
		/// Automatic detect if empty
		/// </summary>
		/// <example>D:\\Project\\ZKWeb\\Tools\\Templates</example>
		public string TemplatesDirectory { get; set; }
		/// <summary>
		/// Project type
		/// </summary>
		/// <example>AspNet</example>
		public string ProjectType { get; set; }
		/// <summary>
		/// Project name
		/// </summary>
		/// <example>ZKWeb.Demo</example>
		public string ProjectName { get; set; }
		/// <summary>
		/// Project description
		/// </summary>
		/// <example>Some Description</example>
		public string ProjectDescription { get; set; }
		/// <summary>
		/// ORM
		/// </summary>
		/// <example>NHibernate</example>
		public string ORM { get; set; }
		/// <summary>
		/// Database
		/// </summary>
		/// <example>PostgreSQL</example>
		public string Database { get; set; }
		/// <summary>
		/// Connection string
		/// </summary>
		/// <example>Server=127.0.0.1;Port=5432;Database=zkweb;User Id=postgres;Password=123456;</example>
		public string ConnectionString { get; set; }
		/// <summary>
		/// If use default plugins, it should be the path of `plugin.collection.json`
		/// otherwise it should be null
		/// </summary>
		/// <example>D:\\Projects\\ZKWeb.Plugins\\plugin.collection.json</example>
		public string UseDefaultPlugins { get; set; }
		/// <summary>
		/// Project output directory
		/// </summary>
		public string OutputDirectory { get; set; }
		/// <summary>
		/// Available product types
		/// </summary>
		public readonly static string[] AvailableProductTypes = new[] {
			"AspNetCore", "AspNet", "Owin"
		};
		/// <summary>
		/// Available ORM
		/// InMemory should only use for test, so it's not here
		/// </summary>
		public readonly static string[] AvailableORM = new[] {
			"Dapper", "EFCore", "MongoDB", "NHibernate"
		};
		/// <summary>
		/// Available databases for specified ORM
		/// </summary>
		public readonly static IDictionary<string, string[]> AvailableDatabases =
			new Dictionary<string, string[]> {
				{ "Dapper", new[] { "MSSQL", "SQLite", "MySQL", "PostgreSQL" } },
				{ "EFCore", new[] { "MSSQL", "SQLite", "MySQL", "PostgreSQL", "InMemory" } },
				{ "MongoDB", new [] { "MongoDB" } },
				{ "NHibernate", new[] { "PostgreSQL", "SQLite", "MySQL", "MSSQL" } }
			};

		/// <summary>
		/// Check parameters
		/// </summary>
		public void Check() {
			if (!AvailableProductTypes.Contains(ProjectType)) {
				throw new ArgumentException(
					"Project type must be one of " +
					string.Join(", ", AvailableProductTypes));
			} else if (string.IsNullOrEmpty(ProjectName)) {
				throw new ArgumentException("Project name can't be empty");
			} else if (!AvailableORM.Contains(ORM)) {
				throw new ArgumentException(
					"ORM must be one of " +
					string.Join(", ", AvailableORM));
			} else if (!AvailableDatabases[ORM].Contains(Database)) {
				throw new ArgumentException(
					"Database must be one of " +
					string.Join(", ", AvailableDatabases[ORM]));
			} else if (string.IsNullOrEmpty(ConnectionString)) {
				throw new ArgumentException("Connection string can't be empty");
			} else if (string.IsNullOrEmpty(OutputDirectory)) {
				throw new ArgumentException("Output directory can't be empty");
			}
			if (!string.IsNullOrEmpty(UseDefaultPlugins)) {
				var pluginCollection = PluginCollection.FromFile(UseDefaultPlugins);
				if (pluginCollection.SupportedORM.Contains(ORM)) {
					throw new ArgumentException(
						"This plugin collection only support following ORM: " +
						string.Join(", ", pluginCollection.SupportedORM));
				}
			}
		}
	}
}
