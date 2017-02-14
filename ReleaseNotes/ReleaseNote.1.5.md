### 1.5 Release Note

### Changes

- Significantly improve ioc container performance in some cases
- Add IHttpResquestHandlerWrapper for easier override http context
- Add Arguments member into TemplateWidgetInfo
- MemberInfo.GetAttributes support passing inherit option
- IoC container now choose constructor that have most parameters instead of choose first constructor
- Bug fixes
	- Fix override ioc dispose error
- Upgrade packages
	- ZKWeb.Fork.DotLiquid 2.1.1
	- ZKWeb.Fork.FastReflection 2.1.1
	- ZKWeb.System.Drawing 2.0.1
