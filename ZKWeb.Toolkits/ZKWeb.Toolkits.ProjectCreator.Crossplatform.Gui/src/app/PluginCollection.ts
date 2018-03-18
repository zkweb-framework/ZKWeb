const fs = require("fs");
export class PluginCollection {

    public PrependPlugins: string[];
    public AppendPlugins: string[];
    public SupportedORM: string[];

    public static FromFile(path: string): PluginCollection {
        const json = fs.readFileSync(path, "utf8");
        return JSON.parse(json);
    }

    public PluginCollection() {
        this.PrependPlugins = [];
        this.AppendPlugins = [];
        this.SupportedORM = [];
    }
}
