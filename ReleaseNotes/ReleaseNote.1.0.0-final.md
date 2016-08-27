### 1.0.0 final Release Note

This version bring many small bug fixes.

### Changes

- Improve website publisher error message
- Make website start handler execute after controllers initialized
	- Allow override default registered actions
- Fix Guid entity key support for NHibernate and PostgreSQL
- Fix Guid entity key support for Entity Framework Core
- Allow Asp.Net Core retry after initialization failed
- Support cascade option in many-to-one part
- Support mapping action parameter from all request parameter
- Support mapping action parameter from json contents
- Improve error message for type have no public constrcutor
- Fix incorrect nullable settings for Entity Framework Core
- Fix old MSSQL support for Entity Framework Core
