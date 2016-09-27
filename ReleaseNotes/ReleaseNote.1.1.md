### 1.1 Release Note

### Changes

- Improvements
	- Add EFCore PostgreSQL support
	- Add HtmlString.Encode, HtmlString.Decode
	- Add IKeyValueCache, the abstrace interface for key-value cache
	- Add ICacheFacotry, the abstract interface for creating cache
	- Add IFileStorage, the abstract interface for file storage
	- Add FileEntryResult, to replace FileResult

- Deprecates
	- Mark IsolatedMemoryCache obsoleted, please use ICacheFactory
	- Mark PathConfig obsoleted, please use IFileStorage
		- May break some testing code that mock this class
	- Mark PathManager obsoleted, please use IFileStorage
		- May break some testing code that mock this class
	- Mark FileResult obsoleted, please use FileEntryResult
