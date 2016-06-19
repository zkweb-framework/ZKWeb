### Extendable .net web framework support running on Asp.Net, Asp.Net Core and Owin

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

Version: 1.0.0 beta 1 (Backward compatibility is provided as much as possible)<br/>

ZKWeb support running on Asp.Net, Asp.Net Core and Owin.<br/>
All version compatible with same plugins.<br/>
Plugins should use the abstraction layer ZKWeb provided, and shouldn't dependent on Asp.Net (Core).<br/>

Plugins: http://github.com/zkweb-framework/ZKWeb.Plugins<br/>
Document: http://zkweb-framework.github.io (Chinese only)<br/>
References: http://zkweb-framework.github.io/cn/references/zkweb/index.html<br/>

Demo Website: http://zkwebsite.com/admin<br/>
Demo Account: demo 123456

This framework is inspired by django, all comments are written in chinese.<br/>

MIT License<br/>
Copyright © 2016 303248153@github<br/>
If you have any license issue please contact 303248153@qq.com.<br/>

### Getting Started:

a. How to run this project?

- Download **ZKWeb** and **ZKWeb.Plugins** from github
- Put **ZKWeb** and **ZKWeb.Plugins** in same folder
- Decide which version you want to run, Asp.Net or Asp.Net Core or Owin.
- Edit **config.json** under **App_Data**, change Database and ConnectionString
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
-	Execute from visual studio or dotnet command.

b. How to add my own plugin?
	
- Add folder **Example** under **ZKWeb.Plugins** 
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
- Create **ExampleController.cs** under **Example\src**

	``` csharp
	[ExportMany]
	public class ExampleController : IController {
			[Action("example")]
			public IActionResult Example() {
				return new TemplateResult("example/test.html", new { message = "hello world" });
			}
	}
	```
- Create **test.html** under **Example\template\example**

	``` html
	<div>{{ message }}</div>
	```
- Edit **config.json** under **App_Data**, add **Example** to **Plugins** list
- Open browser and visit http://localhost:8765/example<br/>
  or visit http://localhost:5000/example if you're running on kestrel
