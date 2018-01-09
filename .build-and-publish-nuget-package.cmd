dotnet build --force --no-incremental --configuration Release ./GroboContainer.sln
dotnet pack --no-build --configuration Release ./GroboContainer.sln
cd ./GroboContainer/bin/Release
dotnet nuget push *.nupkg -s https://nuget.org
pause