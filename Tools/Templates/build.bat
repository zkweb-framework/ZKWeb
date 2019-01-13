@echo off

set DevenvPath="D:\Program Files (x86)\Microsoft Visual Studio\2017\Community\Common7\IDE\devenv.exe"

call :BuildNET AspNet.Dapper
call :BuildNET AspNet.EFCore
call :BuildNET AspNet.MongoDB
call :BuildNET AspNet.NHibernate
call :BuildNETCore AspNetCore.Dapper
call :BuildNETCore AspNetCore.EFCore
call :BuildNETCore AspNetCore.MongoDB
call :BuildNETCore AspNetCore.NHibernate
call :BuildNET Owin.Dapper
call :BuildNET Owin.EFCore
call :BuildNET Owin.MongoDB
call :BuildNET Owin.NHibernate
timeout 5
exit 0

:BuildNETCore
echo Build %~1
cd %~1
dotnet restore
dotnet build -c Release
cd ..
exit /b 0

:BuildNET
echo Build %~1
cd %~1
%DevenvPath% /Build Release __ProjectName__.sln
cd ..
exit /b 0