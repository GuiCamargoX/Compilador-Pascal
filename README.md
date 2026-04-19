# Compilador Pascal (C#)

Small educational Pascal compiler written in C#.  
This repository is focused on learning how a compiler is wired end-to-end.

## What is implemented

- lexical analysis (scanner) for Pascal-like tokens
- syntactic + semantic passes over tokens
- symbol table with scope handling
- MEPA-like instruction generation into `Mepa.txt`

Main pipeline:

1. read Pascal source
2. tokenize source (`Scanner`)
3. parse + semantic checks (`Parser` + `SymbolTable`)
4. emit MEPA output (`Mepa.txt`)

## Why this design

- The code is separated by compiler stages (`Scanner`, `Parser`, symbol structures) so beginners can study one stage at a time.
- `Parser` is a single-file grammar implementation, which is not the most modular design, but makes control flow easy to trace while learning.

Tradeoffs:

- Easy to follow for study.
- Harder to maintain as grammar grows because many parser rules live in one file.

## Project shape

- Solution: `Compilador.sln`
- Main project: `Compilador/Compilador.csproj` (.NET Framework 4.6.1)
- Entrypoint: `Compilador/Program.cs`
- Core compiler code: `Compilador/FrontEnd/`
- Token metadata: `Compilador/Tools/TypePascal.cs` + `Compilador/Resource/keywords.txt`

## Build and run

### 1) Build

From repository root:

```bash
msbuild Compilador.sln /p:Configuration=Debug
```

### 2) Run (current behavior)

Important: `Program.cs` currently compiles a hardcoded file, `while.pas`.

Run from `Compilador/Testes` so that `while.pas` is found:

Windows:

```bat
..\bin\Debug\Compilador.exe
```

Linux (Mono):

```bash
mono ../bin/Debug/Compilador.exe
```

### 3) Check output

- token stream is printed to stdout during scanning
- generated MEPA is written to `Mepa.txt` in the current working directory

## Correct usage examples

- Good input fixture: `Compilador/Testes/while.pas`
- Other focused fixtures: `Compilador/Testes/for.pas`, `Compilador/Testes/ifElse.pas`, `Compilador/Testes/procSemPar.pas`

## Common mistakes

- Running from the wrong directory (compiler cannot find `while.pas`).
- Editing files under `examples/` and expecting parser parity. Many `examples/*.pas` headers do not match current parser expectations.
- Expecting comments to work. Comment handling is currently incomplete.

## Safe refactor workflow (for beginners)

1. Change one compiler stage at a time (scanner or parser, not both).
2. Rebuild.
3. Run with `Compilador/Testes/while.pas` smoke test.
4. Compare token stream and `Mepa.txt` before and after.
5. Only then move to a second fixture like `for.pas` or `ifElse.pas`.

## Debugging checklist

- Build succeeds for `Compilador.sln`.
- The executable is launched from `Compilador/Testes`.
- Scanner reaches `TK_EOF`.
- Parser does not throw in `match(...)`.
- `Mepa.txt` is created and updated on each run.
