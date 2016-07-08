### Extendable .net web framework support running on Asp.Net, Asp.Net Core and Owin

### Features

- Dynamic plugins
	- Roslyn
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

Version: 1.0.0 beta 4 (Backward compatibility is provided as much as possible)<br/>

### Hosting Environment

ZKWeb support running on Asp.Net, Asp.Net Core and Owin.<br/>
All version compatible with same plugins.<br/>
Plugins should use the abstraction layer ZKWeb provided, and shouldn't dependent on Asp.Net (Core).<br/>

### .Net Core Support

ZKWeb provided basic support for .net core now.<br/>
But because NHibernate and Substitite doesn't have .net core support yet,<br/>
functions like database access are unavailable.<br/>

### Links and License

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

Open "Tools\ProjectCreator.Gui.exe", create your own project then open it.<br/>
For more information please see the document.<br/>
