#!/usr/bin/env bash
cd ..
dotnet restore
dotnet build -c Release
cd nuget
Version=$(cat ../../VERSION.txt)
rm -fv *.nupkg
for filename in $(ls *.nuspec); do
	nuget pack -Properties "version=${Version}" "$filename"
done