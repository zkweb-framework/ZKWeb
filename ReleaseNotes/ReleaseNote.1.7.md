### 1.7 Release Note

### Changes

- Update project format to new csproj, you will need vs2017 to open newly created Asp.Net Core project
- Update project templates
	- Add [SingletonReuse] to project template's Plugin class
- Add utility classes
	- Add NetworkUtils for getting ip address
- IoC container improvement
	- Add Container.UnregisterImplementation
	- Add InjectAttribute for manually choose constructor to inject
- Wesite stopper improvement
	- Wait for requests finished before stop website up to 3 seconds
