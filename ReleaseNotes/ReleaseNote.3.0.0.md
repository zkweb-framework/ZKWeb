### 3.0.0 Release Note

### Changes

- Update framework
    - update netcoreapp2.2 to netcoreapp3.0
    - supported framework: net461, netcoreapp2.0, netcoreapp3.0
    - notice: efcore 3.0 only works on netcoreapp3.0, net461 will use efcore 2.2 instead
      it's recommended to use nhibernate because it's much stable and have backward compatibility
    - notice: if you upgrade from an exists Asp.NET Core project and set the framework to netcoreapp3.0,
      please replace `.UseKestrel()` to `.UseKestrel(options => options.AllowSynchronousIO = true)`
      in Program.cs under {ProjectName}.AspNetCore project.
- Add plugin hot reloading support for .NET Core 3.0
    - notice: it will disable lazy loading support for efcore (see #18272 in efcore repo)
      as a workaround, you can disable plugin reloading by setting
      "Extra"."ZKWeb.DisableAutomaticPluginReloading" to true in App_Data/config.json
    - there no hot reloading support for .NET Framework because microsoft will drop it in the future
- Update packages
    - Microsoft.Extensions.DependencyInjection.Abstractions 3.0.0
    - Newtonsoft.Json 12.0.2
    - System.Drawing.Common 4.6.0
    - Microsoft.CodeAnalysis.CSharp 3.3.1
    - Microsoft.CSharp 4.6.0
    - Microsoft.Extensions.DependencyModel 3.0.0
    - Microsoft.Owin 4.0.1
    - Dapper 2.0.30
    - Dommel 1.11.0
    - Dapper.FluentMap 1.8.0
    - Dapper.FluentMap.Dommel 1.7.0
    - Microsoft.Data.Sqlite 3.0.0
    - Npgsql 4.1.0
    - MySqlConnector 0.59.1
    - Microsoft.EntityFrameworkCore 3.0.0
    - Microsoft.EntityFrameworkCore.Design 3.0.0
    - Microsoft.EntityFrameworkCore.InMemory 3.0.0
    - Microsoft.EntityFrameworkCore.Sqlite 3.0.0
    - Microsoft.EntityFrameworkCore.SqlServer 3.0.0
    - Microsoft.EntityFrameworkCore.Proxies 3.0.0
    - Npgsql.EntityFrameworkCore.PostgreSQL 3.0.0
    - Pomelo.EntityFrameworkCore.MySql 2.2.0
    - MongoDB.Driver 2.9.2
    - NHibernate 5.2.6
    - MySql.Data 6.10.9
    - System.Data.SQLite 1.0.111
