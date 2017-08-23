# Tools used to create or publish project

### **Create project on windows by gui:**

First Open 'ProjectCreator.Gui.Windows\ZKWeb.Toolkits.ProjectCreator.Gui.exe',

then choose project type, project name, orm and database and click 'Create Project'.

If you want to use default plugin collection(like [zkweb.org](http://zkweb.org)), then you need to download [ZKWeb.Plugins](https://github.com/zkweb-framework/ZKWeb.Plugins) and choose 'src/ZKWeb.Plugins/plugin.collection.json'.

Notice for now only NHibernate support all these plugins, if you choose other ORM you had to write your own plugin set.

Also notice if you choose Dapper, datatable won't created automatically, you should create the datatable first by sql or by other ORM.

And you may want to check [ZKWeb.MVVMDemo](https://github.com/zkweb-framework/ZKWeb.MVVMDemo) for how to use EFCore with MVVM.

### **Create project on windows by command line:**

Execute 'ProjectCreator.Cmd.Windows\ZKWeb.Toolkits.ProjectCreator.Cmd.exe' and follow the help.

For example you can use:

``` text
ProjectCreator.Cmd.Windows\ZKWeb.Toolkits.ProjectCreator.Cmd.exe --type=AspNetCore --name=Hello.World --orm=NHibernate --database=SQLite --connectionString="Data Source={{App_Data}}/test.db;" --output=e:\
```

### **Create project on linux by command line:**

Execute 'dotnet ProjectCreator.Cmd.NetCore/ZKWeb.Toolkits.ProjectCreator.Cmd.dll' and follow the help.

### **Publish project on windows by gui:**

First if your project type is 'Asp.Net' or 'Owin', compile the  project with release configuration,

and if your project type is 'Asp.Net Core', you'll also need publish the web project by `dotnet publish` command.

Then open 'WebsitePublisher.Gui.Windows\ZKWeb.Toolkits.WebsitePublisher.Gui.exe',

choose webroot, output name, output directory and click 'Publish Website',

publish tool will copy all plugins to output directory and remove the source files.

Notice you should choose the folder contains 'website.config' as the webroot.

### **Publish project on windows by command line:**

Execute 'WebsitePublisher.Cmd.Windows\ZKWeb.Toolkits.WebsitePublisher.Cmd.exe' and follow the help.

For example you can use:

``` text
cd e:\Hello.World\src\Hello.World.AspNetCore
dotnet publish -c Release -f net461 -r win10-x64
cd e:\ZKWeb\Tools
WebsitePublisher.Cmd.Windows\ZKWeb.Toolkits.WebsitePublisher.Cmd.exe --root=e:\Hello.World\src\Hello.World.AspNetCore --name=HelloWorld --output=e:\Publish --configuration=release --framework=net461
```

### **Publish object on linux by command line:**

Execute 'dotnet WebsitePublisher.Cmd.NetCore/ZKWeb.Toolkits.WebsitePublisher.Cmd.dll' and follow the help.

# 中文说明

### **在Windows上使用图形界面创建项目:**

首先打开'ProjectCreator.Gui.Windows\ZKWeb.Toolkits.ProjectCreator.Gui.exe',

然后选择项目类型, 项目名称, ORM和数据库并点击'创建项目'.

如果您想使用默认插件集(像[zkweb.org](http://zkweb.org)), 则需要下载[ZKWeb.Plugins](https://github.com/zkweb-framework/ZKWeb.Plugins)然后选择'src/ZKWeb.Plugins/plugin.collection.json'.

注意目前只有NHibernate支持所有的默认插件, 如果您选择了其他的ORM则需要编写自己的插件集.

注意如果您选择了Dapper, 数据表不会自动创建, 你可以通过sql或者其他ORM先创建好数据表再使用Dapper.

您可能想看看[ZKWeb.MVVMDemo](https://github.com/zkweb-framework/ZKWeb.MVVMDemo), 这个项目使用了EFCore和MVVM.

### **在Windows上使用命令行创建项目:**

执行'ProjectCreator.Cmd.Windows\ZKWeb.Toolkits.ProjectCreator.Cmd.exe'并跟随里面的帮助.

例如您可以执行:

``` text
ProjectCreator.Cmd.Windows\ZKWeb.Toolkits.ProjectCreator.Cmd.exe --type=AspNetCore --name=Hello.World --orm=NHibernate --database=SQLite --connectionString="Data Source={{App_Data}}/test.db;" --output=e:\
```

### **在Linux上使用命令行创建项目:**

执行'dotnet ProjectCreator.Cmd.NetCore/ZKWeb.Toolkits.ProjectCreator.Cmd.dll'并跟随里面的帮助.

### **在Windows上使用图形界面发布项目:**

首先如果项目类型是'Asp.Net'或者'Owin', 则需要先使用Release配置编译主项目,

如果项目类型是'Asp.Net Core', 还需要执行`dotnet publish`发布网站.

然后打开'WebsitePublisher.Gui.Windows\ZKWeb.Toolkits.WebsitePublisher.Gui.exe',

选择网站根目录, 输出名称, 输出文件夹并点击'发布网站',

发布工具会复制所有插件到输出文件夹并删除源代码文件.

注意您应该选择包含'website.config'的文件夹作为网站根目录.

### **在Windows上使用命令行发布项目:**

执行'WebsitePublisher.Cmd.Windows\ZKWeb.Toolkits.WebsitePublisher.Cmd.exe'并跟随里面的帮助.

例如您可以执行:

``` text
cd e:\Hello.World\src\Hello.World.AspNetCore
dotnet publish -c Release -f net461 -r win10-x64
cd e:\ZKWeb\Tools
WebsitePublisher.Cmd.Windows\ZKWeb.Toolkits.WebsitePublisher.Cmd.exe --root=e:\Hello.World\src\Hello.World.AspNetCore --name=HelloWorld --output=e:\Publish --configuration=release --framework=net461
```

### **在Linux上使用命令行发布项目:**

执行'dotnet WebsitePublisher.Cmd.NetCore/ZKWeb.Toolkits.WebsitePublisher.Cmd.dll' 并跟随里面的帮助.

如果您需要更多帮助, 可以加入QQ群: 522083886
