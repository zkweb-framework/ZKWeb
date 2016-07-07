### 1.0.0 beta 4 Release Note

### 主要更新内容

- 更新项目生成器
  - 修复因为程序集同名导致NHibernate配置失败的问题
- 更新网站发布器
  - 支持发布Asp.Net Core网站到IIS
- 修复Http错误处理器的调用顺序

### 已知问题

- Asp.Net Core的502问题
  - https://github.com/aspnet/IISIntegration/issues/219
