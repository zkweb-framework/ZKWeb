@echo off
cd ..
dotnet restore
dotnet build -c Release
cd nuget
del *.nupkg
set /p VERSION=<..\..\VERSION.txt
echo %VERSION%
for %%f in (*.nuspec) do nuget pack -Properties "version=%VERSION%" "%%f"
timeout 5