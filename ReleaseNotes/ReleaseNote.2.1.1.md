### 2.1.1 Release Note

### Changes

- Update packages
	- Npgsql 4.0.0
	- MySqlConnector 0.42.0
	- Microsoft.Data.Sqlite 2.1.0
	- Dapper 1.50.5
	- MongoDB.Driver 2.6.1
	- MySql.Data 6.10.7
	- NHibernate 5.1.3
	- Microsoft.EntityFrameworkCore 2.1.0
	- Npgsql.EntityFrameworkCore.PostgreSQL 2.1.0
	- Microsoft.EntityFrameworkCore.Sqlite 2.1.0
	- Microsoft.EntityFrameworkCore.Design 2.1.0
	- Microsoft.EntityFrameworkCore.SqlServer 2.1.0
	- Microsoft.EntityFrameworkCore.InMemory 2.1.0
	- Microsoft.Extensions.DependencyModel 2.1.0
	- Microsoft.Extensions.DependencyInjection 2.1.0
	- Microsoft.Extensions.DependencyInjection.Abstractions 2.1.0
	- Microsoft.AspNetCore.Http.Abstractions 2.1.0
	- Microsoft.AspNetCore.Hosting.Abstractions 2.1.0
	- Newtonsoft.Json 11.0.2
	- Microsoft.CodeAnalysis.CSharp 2.8.2
- Recompile all plugins when zkweb version changed

### Break Changes

- Switch to official FluentnHibernate since they added .Net Core support
	- No code level change required
- Switch to official System.Drawing.Common
	- Please replace "using System.DrawingCore" to "using System.Drawing"
