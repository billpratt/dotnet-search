dotnet-nuget-search
============
Search for Nuget packages using the .NET CLI.

## Installation

### .NET 6 or higher
```
dotnet tool install --global dotnet-nuget-search
```

## Build

```
git clone https://github.com/lucaslra/dotnet-nuget-search
cd dotnet-search/src/DotNetSearch
dotnet pack -c release -o nupkg
```

Output is located in ```src/DotNetSearch/nupkg```

### Uninstall

```
dotnet tool uninstall -g dotnet-nuget-search
```
