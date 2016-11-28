### 1.3 Release Note

### Changes

- Upgrade packages
	- Microsoft.AspNetCore.Hosting.Abstractions 1.1.0
	- Microsoft.AspNetCore.Http.Abstractions 1.1.0
	- Microsoft.Extensions.PlatformAbstractions 1.1.0
	- System.Threading.Thread 4.3.0
	- System.Diagnostics.Process 4.3.0
	- System.Security.Cryptography.Algorithms 4.3.0
	- System.Runtime.Loader 4.3.0
	- System.Data.Common 4.3.0
	- System.IO.FileSystem.Watcher 4.3.0
	- NETStandard.Library 1.6.1
	- Microsoft.Data.Sqlite 1.1.0
	- Microsoft.EntityFrameworkCore 1.1.0
	- Microsoft.EntityFrameworkCore.Design 1.1.0
	- Microsoft.EntityFrameworkCore.InMemory 1.1.0
	- Microsoft.EntityFrameworkCore.Sqlite 1.1.0
	- Microsoft.EntityFrameworkCore.SqlServer 1.1.0
	- Microsoft.Extensions.DependencyModel 1.1.0
	- Npgsql 3.1.9
	- Npgsql.EntityFrameworkCore.PostgreSQL 1.1.0
	- Pomelo.EntityFrameworkCore.MySql 1.0.1
- Downgrade packages
	- MySql.Data 6.9.9 (new version breaks asp.net)
- Improve getting arguments from http request
	- IHttpRequest.Get support json content
	- IHttpRequest.Get support posted file
	- IHttpRequest.GetAll support json content
- Support range header for file and stream
	- Only available for stream that can seek
- Improve NHibernate RawUpdate and RawQuery, allow passing null parameters
- Bug fixes
	- Fix confict error releated to System.Drawing
	- Fix HttpContextCache error when http context not exists
