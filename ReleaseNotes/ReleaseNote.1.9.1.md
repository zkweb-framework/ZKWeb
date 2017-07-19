### 1.9.1 Release Note

### Changes

- Improve ORM layer
	- Allow disable database auto migration for NHibernate or EFCore
		- Add `"ZKWeb.DisableEFCoreDatabaseAutoMigration": true` under `Extra` in `App_Data\config.json`
		- Add `"ZKWeb.DisableNHibernateDatabaseAutoMigration": true` under `Extra` in `App_Data\config.json`
- Bug fixes
	- Fix auto increment primary key support for dapper
	- Make project creator write files with utf-8 bom
