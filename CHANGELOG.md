# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added
- Implemented `JsonFlattener` static class to map recursive nested JSON DOM documents to flat dot-notated string key-value maps.
- Integrated explicit recursion depth validation capped at 100 levels to prevent `StackOverflowException`.
- Added support for compact serialization of primitive arrays inside JSON objects using standard serializers.
- Enforced strict structural limits to reject non-primitive arrays containing objects or nested array structures.
- Added mapping of empty/null JSON structures directly to empty string table cells (`""`).

### Changed
- None.

### Fixed
- None.

### Removed
- None.

## [0.1.0] - 2026-02-17

### Added
- Scaffolded the main solution configuration `JsonToCsv.sln` tracking production and test projects.
- Created production CLI project `src/JsonToCsv/JsonToCsv.csproj` targeting .NET 10.
- Introduced `CliOptions` argument binding support parsing `-i/--input`, `-o/--output`, and `-d/--delimiter` flags.
- Implemented delimiter validator supporting escape characters (`\t`, `\n`, `\r`) and rejecting multi-character structures.
- Implemented `Program` static entrypoint returning zero exit code on success, or non-zero and descriptive errors on validation failure.
- Configured xUnit testing library infrastructure with `tests/JsonToCsv.Tests/JsonToCsv.Tests.csproj` and registered it inside solution files.
- Reconciled system `.gitignore` preventing build, test results, and visual studio caching directories from reaching source control.