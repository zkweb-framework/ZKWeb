@echo off

call :CleanNETCore AspNetCore.Dapper
call :CleanNETCore AspNetCore.EFCore
call :CleanNETCore AspNetCore.MongoDB
call :CleanNETCore AspNetCore.NHibernate
timeout 5
exit 0

:CleanNETCore
echo Clean %~1
rmdir /s /q %~1\src\__ProjectName__.AspNetCore\bin
rmdir /s /q %~1\src\__ProjectName__.AspNetCore\obj
rmdir /s /q %~1\src\__ProjectName__.Console\bin
rmdir /s /q %~1\src\__ProjectName__.Console\obj
rmdir /s /q %~1\src\__ProjectName__.Plugins\bin
rmdir /s /q %~1\src\__ProjectName__.Plugins\obj
exit /b 0