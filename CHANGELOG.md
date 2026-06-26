# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added
- Scaffolded the main solution configuration `JsonToCsv.sln` tracking production and test projects.
- Created production CLI project `src/JsonToCsv/JsonToCsv.csproj` targeting .NET 10.
- Introduced `CliOptions` argument binding support parsing `-i/--input`, `-o/--output`, and `-d/--delimiter` flags.
- Implemented delimiter validator supporting escape characters (`\t`, `\n`, `\r`) and rejecting multi-character structures.
- Implemented `Program` static entrypoint returning zero exit code on success, or non-zero and descriptive errors on validation failure.
- Configured xUnit testing library infrastructure with `tests/JsonToCsv.Tests/JsonToCsv.Tests.csproj` and registered it inside solution files.
- Reconciled system `.gitignore` preventing build, test results, and visual studio caching directories from reaching source control.