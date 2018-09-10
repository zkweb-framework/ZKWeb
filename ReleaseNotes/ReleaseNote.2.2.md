### 2.2 Release Note

### Changes

- Testing
	- Add more assertion type to Assert class
	- Move test cases under ZKWeb and ZKWebStandard project to distinct assembly
	- Improve error message for test failure
	- Add Scenario class for BDD
	- Rewrite some tests in BDD style for readability
- Small Fixes
	- Use thread local random generator in RandomUtils class
	- Use concurrent dictionary and remove rwlock form MemoryCache class
	- Add memory barrier in LazyCache class
	- Remove finalizer from SimpleDisposable class to make it really simple
