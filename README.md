### An Extendable .Net Web Framework

Version: 1.0.0 rc 1
<h5>Backward compatibility is provided as much as possible.</h5>

[![Build status](https://ci.appveyor.com/api/projects/status/9teo6nnlodxonc3t?svg=true)](https://ci.appveyor.com/project/303248153/zkweb)
[![NuGet](https://buildstats.info/nuget/ZKWeb)](http://www.nuget.org/packages/ZKWeb)

### Features

- .Net Core Support
	- Support both .Net Framework and .Net Core
- Plugin System
	- Using Roslyn
	- Support runtime plugin reload
	- Support automatic compile and reload when plugin source code changed
- Template System
	- Using DotLiquid
	- Support django style template overriding
	- Support mobile specialized templates
	- Support dynamic contents (by area tag)
	- Support per-widget render cache (for extremely fast rending)
- IoC Container
	- Using custom lightweight IoC container
	- Support register by MEF style attributes
- Multiple Host Environment
	- Support running on Asp.Net
	- Support running on Asp.Net Core
	- Support running on Owin
	- All host environment based on same abstraction layer
- Multiple ORM
	- ~~Support NHibernate (Not ready, Not available for .Net Core)~~
	- ~~Support InMemory (Not ready)~~
	- ~~Support EntityFramework Core (Not ready)~~
	- Support runtime database scheme migration
	- All ORM based on same abstraction layer
- Localization
	- Support multi-language
	- Support multi-timezone
	- Support gettext style translation
- Testing
	- Provide console and web test runner
	- Support IoC container overridden
	- Support http context overridden
	- Support temporary database
- Project Toolkits
	- Project Creator for creating ZKWeb project
	- Website Publisher for publishing ZKWeb project

### Features from the default plugin collection

- Form generation and validation
- Ajax table generation
- CRUD page scaffolding
- Scheduled Tasks
- Captcha
- Admin Panel
- Automatic pesudo static support
- Multi-Currency and Region support
- And More...

### Links and License

Plugins: http://github.com/zkweb-framework/ZKWeb.Plugins<br/>
Documents: http://zkweb-framework.github.io (Chinese)<br/>
References: http://zkweb-framework.github.io/cn/references/zkweb/index.html<br/>

Demo Website: http://zkwebsite.com/admin<br/>
Demo Account: demo 123456

MIT License<br/>
Copyright © 2016 303248153@github<br/>
If you have any license issue please contact 303248153@qq.com.<br/>

### Getting Started:

Open "Tools\ProjectCreator.Gui.exe", create your own project then open it.<br/>
For more information please see the documents.<br/>
