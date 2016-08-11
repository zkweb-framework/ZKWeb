### 1.0.0 rc 1 Release Note

### Changes

- Translate all comments to english
- Fix Asp.Net Core 502 error when returnning 304

### Breaking Changes

- Provide full support for .net core
	- Support multiple ORM
		- Support InMemory
		- Support NHibernate
		- ~~Support EFCore~~
	- Replace System.Drawing with CoreCompat.System.Drawing

### Upgrade from previous version

- Replace "DatabaseContext" with "IDatabaseContext"
- Replace "DatabaseManager.GetContext" with "DatabaseManager.CreateContext"
- Use "IEntityOperationHandler" Instead of "IDataSaveCallback" and "IDataDeleteCallback"
- Install "ZKWeb.ORM.NHibernate" and "ZKWeb.ORM.InMemory" from nuget
