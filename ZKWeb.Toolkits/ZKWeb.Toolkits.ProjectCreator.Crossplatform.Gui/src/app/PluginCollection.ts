var fs = require('fs');
export class PluginCollection{
        /// <summary>
		/// Plugins prepend to plugin list
		/// </summary>
		public  PrependPlugins :Array<string>;
		/// <summary>
		/// Plugins append to plugin list
		/// </summary>
		public  AppendPlugins :Array<string>;
		/// <summary>
		/// Supported ORM list
		/// </summary>
		public  SupportedORM :Array<string>;

		/// <summary>
		/// Initialize
		/// </summary>
		public PluginCollection() {
			this.PrependPlugins = new Array<string>();
			this.AppendPlugins = new Array<string>();
			this.SupportedORM = new Array<string>();
		}

		/// <summary>
		/// Read plugin collection from file
		/// </summary>
		/// <param name="path">Json file path</param>
		/// <returns></returns>
		public static  FromFile(path:string) :PluginCollection {
		    var json =  fs.readFileSync(path, 'utf8');
            return json;
		}
}