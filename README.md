# Pascal Compiler in C# (Learning Repository)

This repository is a teaching-oriented compiler project. It shows how a compiler can translate a Pascal-like source program into an intermediate instruction format (MEPA).

## What is a compiler?

A compiler is a program that translates source code from one language into another representation.

- Source language here: Pascal-like syntax (`.pas` files)
- Target here: MEPA-like instructions written to `Mepa.txt`

Most compilers are organized as phases. This repository follows that classic structure.

## Compiler types (quick intuition)

- AOT compiler: translates before execution (for example, C/C++ style workflows)
- Interpreter: executes source directly without generating a separate executable
- Hybrid compiler: compiles to intermediate representation, then executes/optimizes later

This project is closest to a front-end + intermediate-code compiler used for learning compiler construction.

## What is implemented in this repository

- lexical analysis (`Scanner`) to generate tokens
- syntax + semantic checks (`Parser` + `SymbolTable`)
- code generation to MEPA instructions (`Parser.GenerateMepa`)

Current flow:

1. read Pascal source file
2. tokenize text into `Token` objects
3. parse grammar and validate declarations/uses
4. emit MEPA output (`Mepa.txt`)

## Type systems used here

There are three different notions of "type" in this codebase:

1. Pascal data types (language-level): `integer`, `real`, `boolean`, `char`, `string`, `array`
2. Token types (lexer output): `TK_PROGRAM`, `TK_IDENTIFIER`, `TK_INTLIT`, `TK_ASSIGNMENT`, ...
3. Internal semantic types (`Parser.TYPE`): `I`, `R`, `B`, `C`, `S`, `P`, `L`, `A`

See `docs/types-in-this-compiler.md` for a beginner-friendly mapping.

## Project structure

- solution: `Compiler.sln`
- project: `Compiler/Compiler.csproj` (.NET Framework 4.6.1)
- entrypoint: `Compiler/Program.cs`
- front-end compiler code: `Compiler/FrontEnd/`
- keyword/operator configuration: `Compiler/Tools/TypePascal.cs`
- embedded keyword list: `Compiler/Resource/keywords.txt`

## Build and run (exact commands)

Build from repository root:

```bash
xbuild Compiler.sln /p:Configuration=Debug
```

Run from `Compiler/Tests` (important):

Windows:

```bat
..\bin\Debug\Compiler.exe
```

Linux/Mono:

```bash
mono ../bin/Debug/Compiler.exe
```

Why this directory matters: `Program.cs` currently compiles a hardcoded relative file (`while.pas`).

## What to verify after running

- token stream is printed to stdout
- final token includes `TK_EOF`
- `Mepa.txt` is generated/updated in the current working directory

## Supported subset (practical view)

- declarations: `label`, `var`, `procedure`
- commands: assignment, `if/else`, `while`, `repeat/until`, `for`, `case`, `goto`, `read`, `write`
- expressions: arithmetic, relational, and boolean operators

Use `Compiler/Tests/*.pas` as primary fixtures for parser validation.

## Common mistakes beginners hit

- Running the executable outside `Compiler/Tests` (input file not found)
- Testing with `examples/*.pas` and expecting full parser compatibility (many headers do not match parser startup rule)
- Expecting comment support (comment handling is currently incomplete)

## Safe refactor workflow

1. modify one compiler phase at a time
2. rebuild
3. run `while.pas` smoke test
4. inspect stdout token stream and `Mepa.txt`
5. validate with one extra fixture (`for.pas`, `ifElse.pas`)

## Learning path

1. read `docs/compiler-fundamentals.md`
2. read `docs/architecture-map.md`
3. read `docs/how-it-works.md`
4. read `docs/types-in-this-compiler.md`
5. read `docs/learning-path.md`
6. follow `docs/implement-new-feature.md`
7. read `docs/build-your-own-compiler.md`
8. use `docs/debugging-checklist.md` while changing code
