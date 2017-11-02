### 2.1 Release Note

### Changes

- Update packages
	- NHibernate 5.0.0
	- Microsoft.CodeAnalysis.CSharp 2.4.0
- Bug fixes
	- Fix state didn't reset during EFCore dbcontext recycle
- Improve ORM
	- Change dapper's mysql provider to MySqlConnector (previous is Pomelo.Data.MySql)
	- Support command logger (see IDatabaseContext.CommandLogger)
		- Dapper: Log insert, update and select
		- EFCore: TBD
		- InMemory: No logging
		- MongoDB: TBD
		- NHibernate: TBD
