dotnet-search
============

[![NuGet][main-nuget-badge]][main-nuget] [![NuGet][nuget-dl-badge]][main-nuget]

[main-nuget]: https://www.nuget.org/packages/dotnet-search/
[main-nuget-badge]: https://img.shields.io/nuget/v/dotnet-search.svg?style=flat-square&label=nuget
[nuget-dl-badge]: https://img.shields.io/nuget/dt/dotnet-search.svg?style=flat-square


Search for Nuget packages using the .NET Core CLI.

## Installation

### .NET Core 2.1 & higher
```
dotnet install tool --global dotnet-search
```
## Usage

### Help

```
$ dotnet search --help

dotnet-search

Usage: dotnet search [arguments] [options]

Arguments:
  query  The search terms used to find packages

Options:
  -?|-h|--help          Show help information
  --include-prerelease  Include prerelease packages
```

### Search

```
$ dotnet search json.net

Name                                    Description              Authors                  Version          Downloads     Verified
_________________________________________________________________________________________________________________________________
Newtonsoft.Json                         Json.NET is a popul...   James Newton-King        11.0.2           112,705,018       *
Json.NET.Web                            Json.NET web client      Caelan                   1.0.49           24,115
Fluent-Json.NET                         See the project pag...   Miguel Angelo (masb...   0.2.0            1,130
TagCache.Redis.Json.Net                 JSON.NET serializat...   Jon Menzies-Smith a...   1.0.0.2          1,091
Fluent-Json.NET.Lib_v9                  Fluent configuratio...   Miguel Angelo (masb...   0.2.1            60
Fluent-Json.NET.Lib_v10                 Fluent configuratio...   Miguel Angelo (masb...   0.2.1            51
NanoMessageBus.Json.NET                 Additional serializ...   Jonathan Oliver          2.0.51           12,876
SOLIDplate.Json.Net                     A set of boilerplat...   Afzal Hassen             1.0.0.10         1,516
Json.Net.Unity3D                        Forked Newtonsoft.J...   Esun Kim                 9.0.1            582
Invisual.Serialization.Json.Net         Json Serializer bui...   Invisual                 3.0.5886.24324   451
CUL.JSON.Net                            Craig's Utility Lib...   James Craig              4.0.304          5,023
Json.NET.ContractResolverExtentions     Contract resolver e...   Raphael Haddad           1.0.1            2,761
Json.Net.Proprety.ToPascalCase          This is a diagnosti...   tanaka_733               1.0.0            791
JWT.DNX.Json.Net                        THIS PACKAGE IS NOW...   John Sheehan, Micha...   1.0.0.1          1,032
Json.NET.Unsigned                       Newtonsoft.Json - U...   OmniBean                 1.0.0            1,113
jwt.dotnetstandard.json-net             Json.NET Serializer...   John Sheehan, Micha...   0.0.1            7,422
WcfJsonNetFormatter                     WcfJsonNetFormatter...   The Hunter               1.4.0.1          7,639
Thot.Json.Net                           Elm inspire encoder...   Maxime Mangel            1.0.1            148
FM.JsonNet                              A library for .NET ...   Frozen Mountain Sof...   2.9.32           9,687
JsonNet.PrivatePropertySetterResolver   Have JSON.Net deser...   Petar Vujic              1.0.0            192
```

### Search including prerelease

```
$ dotnet search microsoft.aspnetcore.all --include-prerelease

Name                       Description              Authors     Version                Downloads   Verified
___________________________________________________________________________________________________________
Microsoft.AspNetCore.All   Microsoft.AspNetCor...   Microsoft   2.1.0-preview1-final   2,137,755       *
```

## Build

```
git clone https://github.com/billpratt/dotnet-search
```
```
cd dotnet-search/src/DotNetSearch
```
```
dotnet pack -c release -o nupkg
```

Output is located in ```src/DotNetSearch/nupkg```

### Uninstall

.NET Core 2.1 preview 1 does not have a way to uninstall yet. For now, you'll have to remove the files from the following paths:

```
(Windows)
%USERPROFILE%\.dotnet\tools\dotnet-search.exe
%USERPROFILE%\.dotnet\tools\dotnet-search.config
%USERPROFILE%\.dotnet\toolspkgs\dotnet-search\

(macOS/Linux)
$HOME/.dotnet/tools/dotnet-search
$HOME/.dotnet/toolspkgs/dotnet-search/
```

## Useful Links

* [.NET Core 2.1 Global Tools Annoucement](https://blogs.msdn.microsoft.com/dotnet/2018/02/27/announcing-net-core-2-1-preview-1/#global-tools)
* [.NET Core Global Tools Sample](https://github.com/dotnet/core/blob/master/samples/dotnetsay/README.md)
* [.NET Core Global Tools and Gotchas](https://www.natemcmaster.com/blog/2018/02/02/dotnet-global-tool/)
