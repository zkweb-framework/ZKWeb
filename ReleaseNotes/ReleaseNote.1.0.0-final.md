### 1.0.0 final Release Note

This version bring many small fixes.

### Changes

- Framework changes
	- Improve website publisher error message
	- Make Asp.Net Core website reboot after initialization failed
	- Make website start handler execute after controllers initialized
	- Support mapping action parameter from all request parameter
	- Support mapping action parameter from json contents
	- Improve error message for type have no public constrcutor
- ORM changes
	- Support retrieve ORM name and database type from IDatabaseContext
	- Support retrieve ORM name from IEntityMappingBuilder
	- Allow pass before delete action to IDatabaseContext.BatchDelete
- NHibernate changes
	- Fix Guid entity key support
	- Support specific navigation column name for HasMany, HasManyToMany
- Entity Framework Core changes
	- Fix Guid entity key support
	- Fix incorrect nullable settings
	- Fix old MSSQL support
	- Support cascade option in many-to-one part
	- Support specific navigation property for Reference, HasMany, HasManyToMany
	- Use unique index instead of alternate key
