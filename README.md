Web framework based on System.Web supports
* dynamic plugins (supported, csscript + codedom)
* code first database auto migration (supported, nhibernate)
* ioc container (supported, dryioc)
* simple and extendable template sysetm (supported, dotliquid)
* multi language support (supported)
* multi timezone support (supported)
* form generation (supported by plugin)
* scaffolding (supported by plugin)
* api generation (planned)
* visual page editor (planned)

State: 0.9.1 alpha<br/>
Plugins: http://github.com/303248153/ZKWeb.Plugins<br/>
Document: http://zkweb-framework.github.io (Chinese only now)

Demo Website: http://zkwebsite.com/admin
Demo Account: demo 123456

This framework is inspired by django<br/>
All comments are written in chinese at this moment because main developers are chinese.<br/>

MIT License<br/>
Copyright © 2016 303248153@github<br/>
If you have any license issue please contact 303248153@qq.com.<br/>

--------------------------------------------------------------------

Fast Help:

a. How to run?

	Download ZKWeb and ZKWeb.Plugins from github,
	Put ZKWeb and ZKWeb.Plugins in same folder,
	Edit config.json under ZKWeb\App_Data change Database and ConnectionString
	Example for mssql
		"Database": "mssql",
		"ConnectionString": "Server=127.0.0.1;Database=test_db;User Id=test_user;Password=123456;",
	Example for postgresql
		"Database": "postgresql",
		"ConnectionString": "Server=127.0.0.1;Port=5432;Database=test_db;User Id=test_user;Password=123456;",
	Example for sqlite
		"Database": "sqlite",
		"ConnectionString": "Data Source={{App_Data}}\test.db;Version=3;New=True;",
	Example for mysql
		"Database": "mysql",
		"ConnectionString": "Server=127.0.0.1;Port=3306;Database=test_db;User Id=test_user;Password=123456;",
	Once database configuration completed, you can run ZKWeb project from visual studio.
	
b. How to add plugin?
	
	Add folder "Example" under "ZKWeb.Plugins" 
	Plugin folder struction:
		Example
			bin: compiled assembly
			src: source files for automatic compilation in developement
			static: static files
			template: html templates
	Create "ExampleController.cs" under "Example\src" 
		[ExportMany]
		public class ExampleController : IController {
				[Action("example")]
				public IActionResult Example() {
					return new TemplateResult("example/test.html", new { message = "hello world" });
				}
		}
	Create "test.html" under "Example\template\example"
		<div>{{ message }}</div>
	Edit "ZKWeb\App_Data\config.json", add "Example" to "Plugins" list
	Open browser and visit http://localhost:port/example
