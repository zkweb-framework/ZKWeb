rm -fv *.nupkg
for filename in `ls *.nuspec`; do
	nuget pack "$filename"
done
