### 1.0.1 Release Note

### Changes

- Update dependencies
	- Update Microsoft.EntityFrameworkCore to 1.0.1
	- Update MongoDB.Driver to 2.3.0-rc1
	- Update MySql.Data to 7.0.5-IR21
- Bug fixes
	- Use FileShare.Read option in IHttpResponseExtensions.WriteFile
	- Log errors from NHibernate schema update
	- Make SQLite schema update with NHibernate work again (Switch back to ZKWeb.Repack.SQLite)
- Improvements
	- Display message from most inner exception on request error
