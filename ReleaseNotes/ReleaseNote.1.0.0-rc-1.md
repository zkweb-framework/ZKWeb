### 1.0.0 rc 1 Release Note

### Changes

- Translate all comments to english
- Fix Asp.Net Core 502 error issue

### Breaking Changes

- Provide full support for .net core
	- Support multiple ORM
		- TBD
	- Replace System.Drawing with CoreCompat.System.Drawing

### Upgrade from previous version

- Replace "DatabaseContext" with "IDatabaseContext"
- Replace "DatabaseManager.GetContext" with "DatabaseManager.CreateContext"
- Install "ZKWeb.ORM.NHibernate" and "ZKWeb.ORM.InMemory" from nuget
