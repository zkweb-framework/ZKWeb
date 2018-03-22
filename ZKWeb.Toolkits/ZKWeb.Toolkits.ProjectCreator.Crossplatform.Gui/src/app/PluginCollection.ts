export class PluginCollection {

    public PrependPlugins: string[];
    public AppendPlugins: string[];
    public SupportedORM: string[];

    public PluginCollection() {
        this.PrependPlugins = [];
        this.AppendPlugins = [];
        this.SupportedORM = [];
    }
}
