# zkweb creator

## build

    cd ./ZKWeb.Toolkits/ZKWeb.Toolkits.ProjectCreator.Crossplatform.Gui

    npm install

    npm run prebuild

    npm run build:prod

## package

    npm run packagewin

    npm run packagelinux

    npm run packagemac

应用程序会生成在build目录下,需要移动到Tools目录里面。

编译打包后生成的可执行文件，当前存在的两个问题：

1. 与之前的gui工具相同，这个工具也需要放在Tools目录下面。

2. 在ubuntu系统中，双击执行拿不到shell的PATH，因而需要shell启动程序中  ./zkweb
