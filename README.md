Web framework based on System.Web supports
* dynamic plugins (supported, csscript + codedom)
* code first database auto migration (supported, nhibernate)
* ioc container (supported, dryioc)
* simple and extendable template sysetm (supported, dotliquid)
* multi language support (supported)
* multi timezone support (supported)
* auto api generation (planned)
* auto form generation (supported)
* scaffold (supported)
* visual page editor (planned)

This framework is inspired by django.<br />
All comments are written in chinese at this moment because main developers are chinese.

MIT License<br />
Copyright © 2016 303248153@github<br />
If you have any license issue please contact 303248153@qq.com.

Fast Help:

a. How to run this project?

	Download ZKWeb and ZKWeb.Plugins from github,
	Put ZKWeb and ZKWeb.Plugins in same folder,
	Edit config.json under ZKWeb\App_Data change Database and ConnectionString
	Here is the example for mssql
		"Database": "mssql",
		"ConnectionString": "Server=127.0.0.1;Database=test_db;User Id=test_user;Password=123456;",
	Here is the example for postgresql
		"Database": "postgresql",
		"ConnectionString": "Server=127.0.0.1;Port=5432;Database=test_db;User Id=test_user;Password=123456;",
	Once database configuration is completed,
	you can run ZKWeb project from visual studio.
	
b. How to add plugin?
	
	Add a folder, assume named "Example" under ZKWeb.Plugins
	Plugin folder struction:
		Example
			bin
				{compiled assembly}
			src
				{source files for automatic compilation}
			static
				{static files}
			template
				{html templates}
	Create a new file under src folder, named ExampleController.cs, content is
		[ExportMany]
		public class ExampleController : IController {
				[Action("example")]
				public IActionResult Example() {
					return new TemplateResult("example/test.html", new { message = "hello world" });
				}
		}
	Create a new file under template\example folder, named test.html, content is
		<div>{{ message }}</div>
	Edit ZKWeb\App_Data\config.json, add "Example" to "Plugins" list
	Open browser and visit http://localhost:port/example

c. How this project going?

	This project was design for online shopping and extendable, rapid development.
	But able to add more features through plugin system.
	At this moment only few features are implemented, so it need man power.
	If you're intrested please join tecent qq group 522083886 for discussing.
