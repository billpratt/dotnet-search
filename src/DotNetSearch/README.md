# Usage

```
Search for Nuget packages

Usage: dotnet nuget-search [options] <query>

Arguments:
  query                    The search terms to used to filter packages

Options:
  --version                Show version information.
  -?|-h|--help             Show help information.
  -i|--include-prerelease  Include prerelease packages
  -s|--skip                The number of results to skip, for pagination
                           Default value is: 0.
  -t|--take                The number of results to return, for pagination
                           Default value is: 10.
  -p|--package-type        The package type to use to filter packages
                           Allowed values are: dependency, dotnettool, dotnetclitool, template, all.
                           Default value is: all.
  -l|--list                Print results as full detail list
  -v|--verbose             Prints all messages to standard output
  -r|--repository          The repository to use for searching
```