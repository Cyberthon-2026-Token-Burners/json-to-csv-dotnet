# JsonToCsv — End-User Usage Guide

Welcome to the `JsonToCsv` utility. This document describes how to execute, configure, and troubleshoot the tool after compilation.

`JsonToCsv` is a command-line interface (CLI) program designed to parse hierarchical JSON text documents, recursively flatten their elements, and write them as well-formed, tabular CSV files using custom column delimiters.

---

## Command-Line Arguments

The utility uses a single primary command verb: `run`.

### Parameters
| Short Flag | Long Flag | Required | Default | Description |
|:---|:---|:---:|:---|:---|
| `-i` | `--input` | **Yes** | *None* | Path to the source JSON file on disk. |
| `-o` | `--output` | **Yes** | *None* | Path where the generated CSV file will be written. |
| `-d` | `--delimiter`| No | `,` (comma) | Custom separator character (e.g. `,`, `;`, `\t`). |

---

## Installation & Compilation

Ensure you have the .NET 10 SDK installed. From the repository root directory, compile a release version of the binary:

```sh
dotnet publish src/JsonToCsv/JsonToCsv.csproj -c Release -o ./dist --self-contained false
```

The executable will be available in the `./dist` folder.

---

## Real-World Examples

### Example 1: Standard Object Array to Comma-Separated CSV
Suppose you have a JSON file named `users.json` containing the following data:

```json
[
  {
    "id": 101,
    "profile": {
      "name": "Alice Vance",
      "roles": ["admin", "user"]
    }
  },
  {
    "id": 102,
    "profile": {
      "name": "Bob Smith"
    },
    "active": true
  }
]
```

Execute the conversion command:

```sh
./dist/JsonToCsv run -i users.json -o users.csv -d ","
```

#### Output (`users.csv`):
```csv
id,profile.name,profile.roles,active
101,Alice Vance,"[\"admin\",\"user\"]",
102,Bob Smith,,True
```

*Note how `profile.roles` is compactly serialized and nested properties are transformed using dot-notation (`profile.name`), while missing keys are safely treated as empty cells.*

### Example 2: Tab-Separated Values (TSV)
Using the same dataset, output a TSV table using the `\t` escape sequence:

```sh
./dist/JsonToCsv run -i users.json -o users.tsv -d "\t"
```

---

## Troubleshooting & Common Errors

The tool validates syntax and file structures strictly. If errors occur, they are written to standard error (`stderr`), the program exits with code `1`, and any partially written files are deleted from the disk.

### 1. Source File Missing
* **Trigger**: The path provided in `-i` or `--input` does not exist.
* **Console Error Output**:
  ```
  Error: Input file not found: 'nonexistent.json'
  ```
* **Resolution**: Double-check that your file path is accurate and that the application has permissions to read the path.

### 2. Invalid Root JSON Structures
* **Trigger**: Passing a JSON file with flat strings/integers at the root, e.g. `["apple", "banana"]`.
* **Console Error Output**:
  ```
  Error: JSON array must contain only objects. Found a non-object element.
  ```
* **Resolution**: Ensure your JSON is formatted either as a single root JSON Object (e.g. `{"id": 1}`) or a JSON Array of Objects.

### 3. Syntax Anomalies (Malformed JSON)
* **Trigger**: A trailing comma or missing brackets in the input JSON.
* **Console Error Output**:
  ```
  Error: JSON parsing failed at line 5, column 12. Details: Expected a cell separator but found... 
  ```
* **Resolution**: Use a JSON validation utility to locate and correct syntax mistakes at the reported coordinates.