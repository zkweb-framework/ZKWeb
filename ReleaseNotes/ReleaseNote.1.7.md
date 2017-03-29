### 1.7 Release Note

### Changes

- Update project format to new csproj, you will need vs2017 to open newly created Asp.Net Core project
- Update project templates
	- Add [SingletonReuse] to project template's Plugin class
- Add utility classes
	- Add NetworkUtils for getting ip address
- IoC container improvement
	- Add Container.UnregisterImplementation
	- Add InjectAttribute for manually choose constructor to inject
- Entity framework core improvement
	- Support save detached entity that key is not empty and not exists in database
- Wesite stopper improvement
	- Wait for requests finished before stop website up to 3 seconds
- Support publish to other platform
	- publish tool support framework option, default is net461
- Update packages
	Dapper.FluentMap 1.5.3
	Dapper.FluentMap.Dommel 1.4.5
	Dommel 1.8.1
	Npgsql 3.2.2
	MongoDB.Driver 2.4.3
	NSubstitute 2.0.2
	Newtonsoft.Json 10.0.1
	Microsoft.CodeAnalysis.CSharp 2.0.0
	Microsoft.Extensions.DependencyModel 1.1.1
	Microsoft.DiaSymReader.Native 1.5.0
