const fs = require("fs");
export class CreateProjectParameters {

    public TemplatesDirectory: string;
    public ProjectType: string;
    public ProjectName: string;
    public ProjectDescription: string;
    public ORM: string;
    public Database: string;
    public ConnectionString: string;
    public UseDefaultPlugins: string;
    public OutputDirectory: string;
    public AvailableProductTypes: string[] = ["AspNetCore", "AspNet", "Owin"];
    public AvailableORM: string[] = ["Dapper", "EFCore", "MongoDB", "NHibernate"];
    public AvailableDatabases: any = {
        Dapper: ["MSSQL", "SQLite", "MySQL", "PostgreSQL"],
        EFCore: ["MSSQL", "SQLite", "MySQL", "PostgreSQL", "InMemory"],
        MongoDB: ["MongoDB"],
        NHibernate: ["PostgreSQL", "SQLite", "MySQL", "MSSQL"],
    };

    public Check(): any {
        if (-1 === this.AvailableProductTypes.indexOf(this.ProjectType)) {
            return {
                isSuccess: false,
                msgPrefix: "ProjectTypeMustBeOneOf",
                args: this.AvailableProductTypes.join(","),
            };
        } else if (!this.ProjectName) {
            return {
                isSuccess: false,
                msgPrefix: "ProjectNameCantBeEmpty",
            };
        } else if (-1 === this.AvailableORM.indexOf(this.ORM)) {
            return {
                isSuccess: false,
                msgPrefix: "ORMMustBeOneOf",
                args: this.AvailableORM.join(","),
            };
        } else if (-1 === this.AvailableDatabases[this.ORM].indexOf(this.Database)) {
            return {
                isSuccess: false,
                msgPrefix: "DatabaseMustBeOneOf",
                args: this.AvailableDatabases[this.ORM].join(","),
            };
        } else if (this.Database !== "InMemory" && !this.ConnectionString) {
            return {
                isSuccess: false,
                msgPrefix: "ConnectionStringCantBeEmpty",
            };
        } else if (!this.OutputDirectory) {
            return {
                isSuccess: false,
                msgPrefix: "OutputDirectoryCantBeEmpty",
            };
        }
        if (this.UseDefaultPlugins) {
            if(!fs.existsSync(this.UseDefaultPlugins)){
                return {
                    isSuccess: false,
                    msgPrefix: "DefaultPluginsFileNotFound",
                };
            }

            const json = fs.readFileSync(this.UseDefaultPlugins, "utf8");
            const pluginCollection =  JSON.parse(json);
            let isOrmExit = false;

            for (const orm of pluginCollection.SupportedORM) {
                if (orm === this.ORM) {
                    isOrmExit = true;
                }
            }

            if (!isOrmExit) {
                return {
                    isSuccess: false,
                    msgPrefix: "ORMMustBeOneOf",
                    args: pluginCollection.SupportedORM.join(","),
                };
            }
        }
        return {
            isSuccess: true,
        };
    }
}
