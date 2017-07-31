 
echo "Deploying DnDGen.Core to NuGet"

ApiKey=$1
Source=$2

echo "Nuget Source is $Source"
echo "Nuget API Key is $ApiKey (should be secure)"

echo "Listing bin directory"
for entry in "./DnDGen.Core/bin"/*
do
  echo "$entry"
done

echo "Packing DnDGen.Core"
nuget pack ./DnDGen.Core/DnDGen.Core.nuspec -Verbosity detailed

echo "Pushing DnDGen.Core"
nuget push ./DnDGen.Core.*.nupkg -Verbosity detailed -ApiKey $ApiKey -Source $Source
