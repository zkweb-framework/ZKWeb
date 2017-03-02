### 1.6 Release Note

### Changes

- Dapper enhancement
	- Change Dapper.Contrib to Dommel
	- Support custom column name
	- Support ignore unmapped property
	- Support insert guid key into mysql database
	- Support serialized type
- Log error instead of throw exception for unsupported ORM mapping action
- Support retrieve underlying database connection from IDatabaseContext
- Add TemplateManager.CreateHash
- Split template widget render logic to ITemplateWidgetRenderer
- Bug fixes
	- LocalFileEntry.OpenWrite should truncate exists file
	- TemplateWidget.Args should be IDictionary<string, object> for deserialize from json
- Update packages
	- NHibernate 4.1.1.4000
	- Npgsql 3.2.1
	- NSubstitute 2.0.1-rc
	- MongoDB.Driver 2.4.2
	- Microsoft.DiaSymReader.PortablePdb 1.2.0
	- Microsoft.DiaSymReader.Native 1.5.0-beta2-24728
	- Microsoft.CodeAnalysis.CSharp 2.0.0-rc4
	- Dommel 1.8.0
	- Dapper.FluentMap 1.5.1
	- Dapper.FluentMap.Dommel 1.4.3
	- ZKWeb.Repack.SQLite 1.0.104
