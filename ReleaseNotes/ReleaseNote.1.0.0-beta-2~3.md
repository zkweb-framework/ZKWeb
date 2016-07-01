### 1.0.0 beta 2~3 Release Note

### 主要更新内容

- 嵌入Hosting代码到主项目
  - 发布ZKWeb.Hosting.AspNet到Nuget
  - 发布ZKWeb.Hosting.AspNetCore到Nuget
  - 发布ZKWeb.Hosting.Owin到Nuget
- ZKWebStandard兼容.Net Core
- 添加IList.AddRange
- 添加MemoryCache.GetOrCreate
- 项目格式重新转换成xproj
  - 等待vs2016出来后还需要转换回csproj (roslyn-project-system)
- 升级.net框架版本到4.6.1
  - 微软已经删除System.Diagnostics.Process等包中对4.5的支持，这次升级是万不得已

### 已知问题

- 请参考beta 1的已知问题
  - Asp.Net Core的502问题因为升级到RTM仍然出现，将会尽快调查原因

### 兼容.Net Core需要的条件

目前完全兼容.Net Core仍然比较困难，主要由于引用的类库仍未兼容。

- System.Drawing
  - 官方未有计划
- CSScript
  - 官方未有计划
- FluentNHibernate, NHibernate
  - https://nhibernate.jira.com/browse/NH-3807
- NSubstitute
  - https://github.com/nsubstitute/NSubstitute/issues/192
