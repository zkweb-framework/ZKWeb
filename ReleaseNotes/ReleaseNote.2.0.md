### 2.0 Release Note

### Changes

- Update framework
	- update netstandard 1.6 to netstandard 2.0
	- update netcoreapp1.1 to netcoreapp2.0
- Update packages
	- Microsoft.AspNetCore.Hosting.Abstractions 2.0.0-preview2-final
	- Microsoft.AspNetCore.Http.Abstractions 2.0.0-preview2-final
	- Microsoft.Data.Sqlite 2.0.0-preview2-final
	- Npgsql 3.2.4.1
	- Microsoft.EntityFrameworkCore 2.0.0-preview2-final
	- Microsoft.EntityFrameworkCore.Design 2.0.0-preview2-final
	- Microsoft.EntityFrameworkCore.InMemory 2.0.0-preview2-final
	- Microsoft.EntityFrameworkCore.Sqlite 2.0.0-preview2-final
	- Microsoft.EntityFrameworkCore.SqlServer 2.0.0-preview2-final
	- Npgsql.EntityFrameworkCore.PostgreSQL 2.0.0-preview2-final
	- Pomelo.EntityFrameworkCore.MySql 2.0.0-preview2-10046
	- MongoDB.Driver 2.4.4
	- ZKWeb.Repack.SQLite 1.0.104
	- Microsoft.CodeAnalysis.CSharp 2.3.0
	- Microsoft.Extensions.DependencyModel 2.0.0-preview2-25407-01
	- Newtonsoft.Json 10.0.3
	- ZKWeb.Fork.DotLiquid 2.2.0
	- ZKWeb.Fork.FastReflection 2.2.0
- Improve IoC container
	- Support scoped reuse
	- Support inject more wrapper types such as Func<T>, Lazy<T> and List<T> to constructor
	- Support register Implement<> to Service<>
	- Add Microsoft.Extensions.DependencyInjection integration
- Bug fixes
	- Support pass parameter's default value to constructor from IoC container
