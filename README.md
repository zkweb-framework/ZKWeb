### Extendable .net web framework

Features:<br/>

- Dynamic plugins
	- Csscript + Codedom
	- Auto compile after modification
- Code first database auto migration
	- FluentNHibernate
	- Auto migration without any handwrite command
- Simple and high performance Ioc container
- Simple and extendable template sysetm
	- DotLiquid
	- Template overriding like django
	- Mobile specialized templates
	- Dynamic contents (area + widget pattern)
	- Per widget render result cache (perform extremely fast rending) 
- Multi language support
- Multi timezone support
- Testing support
	- Console and web test runner
	- Ioc container overridden
	- Http context overridden
	- Temporary database
- Form generation
	- Supported by plugin
- Scaffolding
	- Supported by plugin
- Pesudo static
	- Supported by plugin
- Visual page editor
	- Prepared and planned

Version: 0.9.7 testing (backward compatibility is not provided yet)<br/>

Plugins: http://github.com/zkweb-framework/ZKWeb.Plugins<br/>
Document: http://zkweb-framework.github.io (Chinese only)<br/>
References: http://zkweb-framework.github.io/cn/references/zkweb/html/annotated.html<br/>

Demo Website: http://zkwebsite.com/admin<br/>
Demo Account: demo 123456

This framework is inspired by django, all comments are written in chinese.<br/>

ZKWeb is running on Asp.Net Core now,<br/>
but it has it's abstraction layer to make sure plugins are independent from Asp.Net or Asp.Net Core.<br/>
That make move away from Asp.Net Core in the future without breaking the compatibility is possible.<br/>
The dependency to the following libraries is still required, but they are stable and solid. <br/>

- DotLiquid
- FluentNHibernate
- Newtonsoft.Json
- NSubstitute

MIT License<br/>
Copyright © 2016 303248153@github<br/>
If you have any license issue please contact 303248153@qq.com.<br/>

### Getting Started:

a. How to run this project?

- Download ZKWeb and ZKWeb.Plugins from github
- Put ZKWeb and ZKWeb.Plugins in same folder
- Edit config.json under ZKWeb\App_Data change Database and ConnectionString
	- Example for mssql
	```
	"Database": "mssql",
	"ConnectionString": "Server=127.0.0.1;Database=test_db;User Id=test_user;Password=123456;",
	```
	- Example for postgresql
	```
	"Database": "postgresql",
	"ConnectionString": "Server=127.0.0.1;Port=5432;Database=test_db;User Id=test_user;Password=123456;",
	```
	- Example for sqlite
	```
	"Database": "sqlite",
	"ConnectionString": "Data Source={{App_Data}}/test.db;Version=3;",
	```
	- Example for mysql
	```
	"Database": "mysql",
	"ConnectionString": "Server=127.0.0.1;Port=3306;Database=test_db;User Id=test_user;Password=123456;",
	```
-	Once database configuration completed, you can run ZKWeb project from visual studio.

b. How to add my own plugin?
	
- Add folder "Example" under "ZKWeb.Plugins" 
- Plugin folder struction:

	```
	Example
		bin: compiled assembly
		src: source files for automatic compilation in developement
		static: static files
		template: default html templates
		template.mobile: mobile specialized html templates
		template.desktop: desktop specialized html templates
	```
- Create "ExampleController.cs" under "Example\src"

	``` csharp
	[ExportMany]
	public class ExampleController : IController {
			[Action("example")]
			public IActionResult Example() {
				return new TemplateResult("example/test.html", new { message = "hello world" });
			}
	}
	```
- Create "test.html" under "Example\template\example"

	``` html
	<div>{{ message }}</div>
	```
- Edit "ZKWeb\App_Data\config.json", add "Example" to "Plugins" list
- Open browser and visit http://localhost:port/example
