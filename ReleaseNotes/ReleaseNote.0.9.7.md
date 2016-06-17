### 0.9.7 Release Note

### 主要更新内容

- 迁移核心到Asp.Net Core
- 提供一套不依赖于Asp.Net Core和Asp.Net的抽象层
- 支持创建不使用事务的DatabaseContext

### 更新说明

这次更新把核心项目的运行环境迁移到了Asp.Net Core。<br/>
因为之前使用的运行环境是Asp.Net (System.Web)，这次更新导致之前的插件都需要较多的修改才能运行。<br/>

考虑到以后可能需要移出Asp.Net Core，这次更新ZKWeb提供了独立的抽象层来防止对Asp.Net Core的依赖。<br/>
原有的ZKWeb.Utils项目更名到ZKWebStandard（ZKWeb标准类库）。<br/>
插件应该使用ZKWeb标准类库中提供的接口和函数，不应该依赖于System.Web或Asp.Net Core。<br/>

这次更新中还发现原有的System.Data.Sqlite在更新后不能使用，详见以下的地址：<br/>
http://stackoverflow.com/questions/36284533/project-json-referencing-sqllite<br/>
http://system.data.sqlite.org/index.html/tktview?name=942ab10de2<br/>
目前使用了独自打包的`SQLiteForZKWeb`，但将会在上游更新后恢复到上游提供的包。<br/>

### 性能数据

虚拟机比迁移前慢1倍，实机比迁移前慢15%左右。

2016-06-17 (i7 Q720 1.6Ghz x 4 core 8 threads, ab -n 4000 -c 8, vmware player 12, asp.net core)

- 首页 3.34ms, 3.26ms, 3.23mss
- 商品列表页 3.078ms, 3.00ms, 2.965ms
- 商品详情页 3.727ms, 3.37ms, 3.42ms

2016-06-17 (i7 Q720 1.6Ghz x 4 core 8 threads, ab -n 4000 -c 16, real machine, asp.net core)

- 首页 0.934ms, 0.945ms, 0.949ms
- 商品列表页 0.867ms, 0.824ms, 0.852ms
- 商品详情页 1.539ms, 1.512ms, 1.488ms
