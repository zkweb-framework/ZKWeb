2.0
	- Support action url like "action/{id}"
		- Solution A (faster):
			- Split them into two part, base url part and parameter url part
			- Register base url part to action mapping
		- Solution B (powerful):
			- Convert to regexp
			- Match them during request handling
	- Update .Net standard requirement to 2.0
	- Update Nhibernate to 5.0
	- Support NHibernate sql logging
2.1~
	- More execution performance improvement
	- More database performance improvement
	- Support publish to other platform
		- Provide linux project creator and website publisher

Undefined
- Run all tests on macos
