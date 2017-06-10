### 1.9 Release Note

### Changes

- Update toolkit
	- improve linux support
- Add utility functions
	- Exception.ToDetailedString
	- Exception.ToSummaryString
	- Expression.ReplaceNode
- Improve template engine
	- Keep widget before and after html when render failed
- Improve IoC container
	- Add ExportAttributeBase for support customize registration by attribute
- Improve ORM layer
	- Support specify table name in mapping builder, eg: builder.TableName("MyTable")
	- Mark internal classes as public
- Improve visual studio support
	- Mark Response.End as DebuggerNonUserCode to make visual studio ignore the special exception
- Update project template
	- Allow Asp.Net Core hosting take configuration from command line and json
- Bug fixes
	- Fix IFileEntry.WriteAllBytes
