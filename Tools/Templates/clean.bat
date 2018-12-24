@echo off

call :CleanNET AspNet.Dapper
call :CleanNET AspNet.EFCore
call :CleanNET AspNet.MongoDB
call :CleanNET AspNet.NHibernate
call :CleanNETCore AspNetCore.Dapper
call :CleanNETCore AspNetCore.EFCore
call :CleanNETCore AspNetCore.MongoDB
call :CleanNETCore AspNetCore.NHibernate
call :CleanNET Owin.Dapper
call :CleanNET Owin.EFCore
call :CleanNET Owin.MongoDB
call :CleanNET Owin.NHibernate
timeout 5
exit 0

:CleanNET
echo Clean %~1
rmdir /s /q %~1\.vs
rmdir /s /q %~1\packages
rmdir /s /q %~1\__ProjectName__.AspNet\bin
rmdir /s /q %~1\__ProjectName__.AspNet\obj
rmdir /s /q %~1\src\__ProjectName__.Console\bin
rmdir /s /q %~1\src\__ProjectName__.Console\obj
rmdir /s /q %~1\src\__ProjectName__.Plugins\bin
rmdir /s /q %~1\src\__ProjectName__.Plugins\obj
exit /b 0

:CleanNETCore
echo Clean %~1
rmdir /s /q %~1\.vs
rmdir /s /q %~1\src\__ProjectName__.AspNetCore\bin
rmdir /s /q %~1\src\__ProjectName__.AspNetCore\obj
rmdir /s /q %~1\src\__ProjectName__.Console\bin
rmdir /s /q %~1\src\__ProjectName__.Console\obj
rmdir /s /q %~1\src\__ProjectName__.Plugins\bin
rmdir /s /q %~1\src\__ProjectName__.Plugins\obj
exit /b 0