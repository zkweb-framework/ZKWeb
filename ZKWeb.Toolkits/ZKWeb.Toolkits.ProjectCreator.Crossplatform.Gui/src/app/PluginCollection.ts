const fs = require("fs");
export class PluginCollection {
	/// <summary>
	/// Plugins prepend to plugin list
	/// </summary>
	public PrependPlugins: string[];
	/// <summary>
	/// Plugins append to plugin list
	/// </summary>
	public AppendPlugins: string[];
	/// <summary>
	/// Supported ORM list
	/// </summary>
	public SupportedORM: string[];
	/// <summary>
	/// Read plugin collection from file
	/// </summary>
	/// <param name="path">Json file path</param>
	/// <returns></returns>
	public static FromFile(path: string): PluginCollection {
		const json = fs.readFileSync(path, "utf8");
		return JSON.parse(json);
	}
	/// <summary>
	/// Initialize
	/// </summary>
	public PluginCollection() {
		this.PrependPlugins = new Array<string>();
		this.AppendPlugins = new Array<string>();
		this.SupportedORM = new Array<string>();
	}
}
