### 1.0.0 beta 5 Release Note

### 主要更新内容

- 修复使用Roslyn编译的插件无法正常调试的功能
  - 添加ZKWeb.CompilePluginsWithReleaseConfiguration选项
  - 修改此选项后需要手动删除原有编译的bin文件夹才能生效

### 已知问题

- Asp.Net Core的502问题
  - https://github.com/aspnet/IISIntegration/issues/219
