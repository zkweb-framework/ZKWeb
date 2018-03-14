import { PluginCollection } from "./PluginCollection";

export class CreateProjectParameters {
    /// <summary>
    /// Project template directory
    /// Automatic detect if empty
    /// </summary>
    /// <example>D:\\Project\\ZKWeb\\Tools\\Templates</example>
    public TemplatesDirectory: string;
    /// <summary>
    /// Project type
    /// </summary>
    /// <example>AspNet</example>
    public ProjectType: string;
    /// <summary>
    /// Project name
    /// </summary>
    /// <example>ZKWeb.Demo</example>
    public ProjectName: string;
    /// <summary>
    /// Project description
    /// </summary>
    /// <example>Some Description</example>
    public ProjectDescription: string;
    /// <summary>
    /// ORM
    /// </summary>
    /// <example>NHibernate</example>
    public ORM: string;
    /// <summary>
    /// Database
    /// </summary>
    /// <example>PostgreSQL</example>
    public Database: string;
    /// <summary>
    /// Connection string
    /// </summary>
    /// <example>Server=127.0.0.1;Port=5432;Database=zkweb;User Id=postgres;Password=123456;</example>
    public ConnectionString: string;
    /// <summary>
    /// If use default plugins, it should be the path of `plugin.collection.json`
    /// otherwise it should be null
    /// </summary>
    /// <example>D:\\Projects\\ZKWeb.Plugins\\plugin.collection.json</example>
    public UseDefaultPlugins: string;
    /// <summary>
    /// Project output directory
    /// </summary>
    public OutputDirectory: string;
    /// <summary>
    /// Available product types
    /// </summary>
    public AvailableProductTypes: Array<string> = ["AspNetCore", "AspNet", "Owin"];

    /// <summary>
    /// Available ORM
    /// InMemory should only use for test, so it's not here
    /// </summary>
    public AvailableORM: Array<string> = ["Dapper", "EFCore", "MongoDB", "NHibernate"];
    /// <summary>
    /// Available databases for specified ORM
    /// </summary>
    public AvailableDatabases: any = {
        "Dapper": ["MSSQL", "SQLite", "MySQL", "PostgreSQL"],
        "EFCore": ["MSSQL", "SQLite", "MySQL", "PostgreSQL", "InMemory"],
        "MongoDB": ["MongoDB"],
        "NHibernate": ["PostgreSQL", "SQLite", "MySQL", "MSSQL"]
    };

    /// <summary>
    /// Check parameters
    /// </summary>
    public Check(): string {
        if (-1 == this.AvailableProductTypes.indexOf(this.ProjectType)) {
            return "ProjectTypeMustBeOneOf"+this.AvailableProductTypes.join(",");
        } else if (!this.ProjectName) {
            return "ProjectNameCantBeEmpty";
        } else if (-1 == this.AvailableORM.indexOf(this.ORM)) {
            return "ORMMustBeOneOf"+this.AvailableORM.join(",");
        } else if (-1 == this.AvailableDatabases[this.ORM].indexOf(this.Database)) {
            return "DatabaseMustBeOneOf"+this.AvailableDatabases[this.ORM].join(",");
        } else if (!this.ConnectionString) {
           return "ConnectionStringCantBeEmpty";
        } else if (!this.OutputDirectory) {
            return "OutputDirectoryCantBeEmpty";
        }
        if (this.UseDefaultPlugins) {
            var pluginCollection = PluginCollection.FromFile(this.UseDefaultPlugins);

            var isOrmExit: boolean = false;
            for (var i = 0; i < pluginCollection.SupportedORM.length; i++) {
                if (pluginCollection.SupportedORM[i] == this.ORM) {
                    isOrmExit = true;
                }
            }
            if (!isOrmExit)
            return "ORMMustBeOneOf"+pluginCollection.SupportedORM.join(",");
        }
        return "";
    }
}