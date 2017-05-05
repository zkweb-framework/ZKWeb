### 1.8 Release Note

### Breaking changes

- Upgrade ZKWeb.System.Drawing to 3.0.0
	- Please renamed `System.Drawing` to `System.DrawingCore` in your code
	- We can throw `DisableImplicitFrameworkReferences` option away and no longer need to face errors given by vs2017

### Changes

- Add IActionParameterProvider
	- Can be used to customize the method of getting action parameters
- Add `UseZKWeb` for Asp.Net Core and Owin
- Allow provide custom Application class
	- The initialize process can be customized now
	- Please see `IApplication` and `DefaultApplication`
- Improve assembly dependency resolving by preload referenced assemblies
- Improve publish tool
	- Support publish project with netcoreapp1.1
- Bug fixes
	- Fix deserialize `string` to `ZKWeb.Localize.T` failed
	- Support `dynamic` keyword in plugin compilation
- Upgrade packages
	- NSubstitute 2.0.3
	- Newtonsoft.Json 10.0.2
	- Microsoft.CodeAnalysis.CSharp 2.1.0
	- Microsoft.Owin 3.1.0
	- Pomole.EntityFrameworkCore.MySql 1.1.1
