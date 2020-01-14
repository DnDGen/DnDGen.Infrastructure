 
echo "Deploying DnDGen.Infrastructure to NuGet"

ApiKey=$1
Source=$2

echo "Nuget Source is $Source"
echo "Nuget API Key is $ApiKey (should be secure)"

echo "Pushing DnDGen.Infrastructure"
dotnet nuget push ./DnDGen.Infrastructure/bin/Release/DnDGen.Infrastructure.*.nupkg --api-key $ApiKey --source $Source --skip-duplicate
