### 1.4 Release Note

### Changes

- Add fast batch action to database context
	- Add IDatabaseContext.FastBatchSave
	- Add IDatabaseContext.FastBatchDelete
- Add IHttpRequestPostHandler
	- Usually used to cleanup things done in pre handler
- Add IActionFilter
	- Can register as global action filter
- Add ActionFilterAttribute
	- Can mark with per action
