### 1.8 Release Note

### Breaking changes

- Upgrade ZKWeb.System.Drawing to 3.0.0
	- Please renamed `System.Drawing` to `System.DrawingCore` in your code
	- We can throw `DisableImplicitFrameworkReferences` option away and no longer need to face errors given by vs2017

### Changes

- Add IActionParameterProvider
	- Can be used to customize the method of getting action parameters
- Bug fixes
	- Fix deserialize `string` to `ZKWeb.Localize.T` failed
