@echo off
for %%f in (*.nupkg) do nuget push -Source nuget.org "%%f"
