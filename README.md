# ZKWeb [![Build status](https://ci.appveyor.com/api/projects/status/9teo6nnlodxonc3t?svg=true)](https://ci.appveyor.com/project/303248153/zkweb) [![NuGet](https://img.shields.io/nuget/vpre/ZKWeb.svg)](http://www.nuget.org/packages/ZKWeb)

ZKWeb is a flexible web framework support .Net Framework and .Net Core.<br/>
The goal of this framework is to increase code reusability and decrease dependence on frameworks and tools such as Asp.Net and Visual Studio.

Version: 1.4 final<br/>
Backward compatibility is provided as much as possible.

## Features

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
	- Support Dapper
	- Support EntityFramework Core
	- Support InMemory
	- Support MongoDB
	- Support NHibernate
		- No support for .Net Core yet
	- Support runtime database scheme migration
	- All ORM based on same abstraction layer
- Localization
	- Support multi-language
	- Support multi-timezone
	- Support gettext style translation
- Caching
	- Support policy based isolated cache
		- Isolated by device, request url, and more...
	- Provide abstraction layer for key-value cache
		- Easy to provider your own cache for distributed cache
- File Storage
	- Provide abstraction layer for file storage
		- Easy to provide your own storage for distributed file store
- Testing
	- Provide console and web test runner
	- Support IoC container overridden
	- Support http context overridden
	- Support temporary database
- Project Toolkits
	- Project Creator for creating ZKWeb project
	- Website Publisher for publishing ZKWeb project
- Linux support
	- These distributions are tested
	- Ubuntu 16.04 LTS 64bit
	- CentOS 7.2 64bit
	- Fedora 24 64bit

## Features from the default plugin collection

- Form generation and validation
- Ajax table generation
- CRUD page scaffolding
- Scheduled Tasks
- Captcha
- Admin Panel
- Automatic pesudo static support
- Multi-Currency and Region support
- And More...

## Links and License

Plugins: http://github.com/zkweb-framework/ZKWeb.Plugins<br/>
Documents: http://zkweb-framework.github.io (Chinese)<br/>
References: http://zkweb-framework.github.io/cn/references/zkweb/index.html<br/>

Demo Website: http://zkweb.org/admin<br/>
Demo Account: demo 123456

MIT License<br/>
Copyright © 2016~2017 303248153@github<br/>
If you have any license issue please contact 303248153@qq.com.<br/>

## Getting Started

Open "Tools\ProjectCreator.Gui.exe",<br/>
create your own project with specified hosting and ORM then open it.<br/>
For more information please see the documents.<br/>
