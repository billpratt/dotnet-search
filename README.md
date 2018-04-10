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
  --version             Show version information
  -?|-h|--help          Show help information
  --include-prerelease  Include prerelease packages
  -s|--skip             Number of results to skip. (Default: 0)
  -t|--take             Number of results to display. (Default: 10)
```

### Search

```
$ dotnet search json.net

Name                              Description                 Authors                     Version          Downloads   Verified
_______________________________________________________________________________________________________________________________
Newtonsoft.Json                   Json.NET is a               James Newton-King           11.0.2           113.78M        *
                                  popular
                                  high-performance
                                  JSON framework for
                                  .NET
-------------------------------------------------------------------------------------------------------------------------------
Json.NET.Web                      Json.NET web client         Caelan                      1.0.49           24.32K
-------------------------------------------------------------------------------------------------------------------------------
Fluent-Json.NET                   See the project page        Miguel Angelo (masbicudo)   0.2.0            1.14K
                                  for more.
-------------------------------------------------------------------------------------------------------------------------------
TagCache.Redis.Json.Net           JSON.NET                    Jon Menzies-Smith           1.0.0.2          1.10K
                                  serialization for           and Fabian Nicollier
                                  TagCache.Redis
-------------------------------------------------------------------------------------------------------------------------------
Fluent-Json.NET.Lib_v10           Fluent configuration        Miguel Angelo (masbicudo)   0.2.1            51
                                  for Json.NET v10
                                  library. Tried to
                                  follow Fluent
                                  NHibernate mapping
                                  style. Implemented
                                  as converter and
                                  contract resolver.
-------------------------------------------------------------------------------------------------------------------------------
Fluent-Json.NET.Lib_v9            Fluent configuration        Miguel Angelo (masbicudo)   0.2.1            60
                                  for Json.NET v9
                                  library. Tried to
                                  follow Fluent
                                  NHibernate mapping
                                  style. Implemented
                                  as converter and
                                  contract resolver.
-------------------------------------------------------------------------------------------------------------------------------
NanoMessageBus.Json.NET           Additional                  Jonathan Oliver             2.0.51           13.01K
                                  serialization
                                  provider for
                                  NanoMessageBus based
                                  on the Newtonsoft
                                  Json.NET library.
-------------------------------------------------------------------------------------------------------------------------------
SOLIDplate.Json.Net               A set of boilerplate        Afzal Hassen                1.0.0.10         1.54K
                                  code libraries that
                                  facilitate
                                  implementation of
                                  S.O.L.I.D principles
                                  in .Net solutions
-------------------------------------------------------------------------------------------------------------------------------
Json.Net.Unity3D                  Forked                      Esun Kim                    9.0.1            627
                                  Newtonsoft.Json to
                                  support Unity3D
-------------------------------------------------------------------------------------------------------------------------------
Invisual.Serialization.Json.Net   Json Serializer             Invisual                    3.0.5886.24324   456
                                  built with Json.Net

1 - 10 of 259 results
```

### Search including prerelease

```
$ dotnet search microsoft.aspnetcore.all --include-prerelease

Name                       Description                Authors     Version                Downloads   Verified
_____________________________________________________________________________________________________________
Microsoft.AspNetCore.All   Microsoft.AspNetCore.All   Microsoft   2.1.0-preview2-final   2.23M          *

1 - 1 of 1 results
```

### Pagination
```
$ dotnet search [query] --skip 10             # skip 10, take default
$ dotnet search [query] --take 50             # skip 0, take 50
$ dotnet search [query] --skip 10 --take 50   # skip 10, take 50
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
