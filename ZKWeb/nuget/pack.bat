@echo off
del *.nupkg
set /p VERSION=<..\..\VERSION.txt
echo %VERSION%
for %%f in (*.nuspec) do nuget pack -Properties "version=%VERSION%" "%%f"
