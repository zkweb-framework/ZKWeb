### 2.2 Release Note

### Changes

- Refactory Container
	- Use thread free data struction
	- (Break Change) Use ContainerFactoryData to create instance
	- (Break Change) Remove static ContainerFactoryCache to simplify code
	- (Break Change) Update interface IMultiConstructorResolver
	- (Break Change) Update interface IRegistrator
- Update Tests
	- Add more assertion type to Assert class
	- Move test cases under ZKWeb and ZKWebStandard project to standalone assembly
	- Improve error message for test failure
	- Add Scenario class for BDD
	- Rewrite some tests in BDD style for readability
- Update Utilities
	- Use thread local random generator in RandomUtils class
	- Use concurrent dictionary and remove rwlock form MemoryCache class
	- Add memory barrier in LazyCache class
	- Remove finalizer from SimpleDisposable class to make it really simple
- Update Project Creator
	- Make template projects support inplace upgrade