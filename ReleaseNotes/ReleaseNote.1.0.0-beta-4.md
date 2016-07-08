### 1.0.0 beta 4 Release Note

### 主要更新内容

- 更新项目生成器
  - 修复因为程序集同名导致NHibernate配置失败的问题
- 更新网站发布器
  - 支持发布Asp.Net Core网站到IIS
- 修复Http错误处理器的调用顺序
- 更换Csscript到Roslyn
  - 插件已支持c# 6.0的语法
- 初步支持.Net Core
  - 简单的插件可以在.Net Core下编译
  - 数据库等功能仍未支持，请参考"未兼容.Net Core的部分"

### 已知问题

- Asp.Net Core的502问题
  - https://github.com/aspnet/IISIntegration/issues/219

### 未兼容.Net Core的部分

这个版本提供了初步的.Net Core支持，但以下部分仍未兼容

- System.Drawing
  - 官方未有计划，可能需要换成其他类库
- FluentNHibernate, NHibernate
  - https://nhibernate.jira.com/browse/NH-3807
- NSubstitute
  - https://github.com/nsubstitute/NSubstitute/issues/192
  - https://github.com/nsubstitute/NSubstitute/pull/197
