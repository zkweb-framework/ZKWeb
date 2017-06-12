### 1.9 Release Note

### Changes

- Update toolkit
	- Improve linux support
- Add utility functions
	- Exception.ToDetailedString
	- Exception.ToSummaryString
	- Expression.ReplaceNode
- Improve application initialization
	- Log emergency log when initialize failed
	- Change protect level of `Initialize` methods, from "internal static" to "internal protected virtual"
- Improve template engine
	- Keep widget before and after html when render failed
- Improve IoC container
	- Add ExportAttributeBase
- Improve ORM layer
	- Support specify table name in mapping builder, eg: builder.TableName("MyTable")
	- Change protect level of classes, from "internal" to "public"
- Improve visual studio support
	- Mark Response.End as DebuggerNonUserCode to make visual studio ignore the exception
- Update project template
	- Allow Asp.Net Core hosting take configuration from command line and json
- Bug fixes
	- Fix IFileEntry.WriteAllBytes
