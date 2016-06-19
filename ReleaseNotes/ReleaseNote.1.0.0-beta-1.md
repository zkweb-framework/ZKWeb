### 1.0.0 beta 1 Release Note

### 主要更新内容

- 同时提供Asp.Net, Asp.Net Core, Owin版本
  - 所有版本兼容相同的插件
- 发布ZKWeb到nuget
  - 开发插件时应该引用nuget上的ZKWeb
- 从这个版本开始将会尽量提供向后兼容性

### 已知问题

- Asp.Net Core版本在返回304的时候有一定几率会变502
  - 目前预计是kestrel和iis之间的问题，单独运行kestrel时不会出现此问题
  - 等待Asp.Net Core RTM后再看是否已修复
