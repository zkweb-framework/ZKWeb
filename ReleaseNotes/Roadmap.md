2.1
	- Update Nhibernate to 5.0
	- Add EFCoreDatabaseContextPool
		- https://github.com/aspnet/EntityFrameworkCore/blob/a661feb049d92533021d7377ad0d289b64a722f0/src/EFCore/Internal/DbContextPool.cs
	- Change Pomelo.Data.MySql to MySqlConnector
	- Provide sql logging support
		- NHibernate logging
		- Pomelo.EntityFrameworkCore.Extensions.ToSql
			- Support IQueryable<T>.ToSql and IQueryable<T>.ToUnevaluated
2.2~
	- More execution performance improvement
	- More database performance improvement
	- Support publish to other platform
		- Provide linux project creator and website publisher

Undefined
- Run all tests on macos

Misc
	* Rewrite document for 2.0
	* Provide better instruction for beginner
	* Provide disinct home site
	* Provide disinct demo site
	* Remove NSubstitute in project templates
	- Enable SSL for demo site (money$$$)
	- Update document for visual page editor
