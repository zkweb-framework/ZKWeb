@echo off

call :BuildNETCore AspNetCore.Dapper
call :BuildNETCore AspNetCore.EFCore
call :BuildNETCore AspNetCore.MongoDB
call :BuildNETCore AspNetCore.NHibernate
timeout 5
exit 0

:BuildNETCore
echo Build %~1
cd %~1
dotnet restore
dotnet build -c Release
cd ..
exit /b 0