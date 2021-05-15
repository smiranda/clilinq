# CliLINQ

![clilinq](clilinq.png)

![nuget](https://img.shields.io/nuget/v/clilinq.svg)

This is a command line interface to run LINQ packaged as cli dotnet tool nuget at <https://www.nuget.org/packages/clilinq>.

## Installation

```bash
dotnet tool install --global clilinq
```

Examples:

```bash
cat .\test.txt | clilinq --format "lines" --script "Data.Sum(e => Convert.ToInt32(e))"
```

```bash
cat .\test.json | clilinq --format "json" --script "Data.Sum(e => (long)e)"
```

```bash
cat .\test.jsonl | clilinq --format "jsonl" --script "Data.Sum(e => (long)e[\""v\""].Value<long>())"
```
