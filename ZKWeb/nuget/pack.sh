#!/usr/bin/env bash
Version=$(cat ../../VERSION.txt)
rm -fv *.nupkg
for filename in $(ls *.nuspec); do
	nuget pack -Properties "version=${Version}" "$filename"
done
