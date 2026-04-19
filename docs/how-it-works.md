# How This Compiler Works

This document explains the current implementation as it exists in code.

## What is implemented

- Scanner: reads Pascal source and emits tokens.
- Parser: validates grammar, performs semantic checks, and emits MEPA instructions.
- Symbol table: stores variables, procedures, labels, and scope information.

Core wiring:

- `Compiler/Program.cs`
- `Compiler/FrontEnd/Lexer/Scanner.cs`
- `Compiler/FrontEnd/Parser/Parser.cs`
- `Compiler/FrontEnd/Semantics/SymbolTable.cs`
- `Compiler/Tools/TypePascal.cs`

## Why this design

- Stages are explicit (`Scanner` -> `Parser` -> output), which makes the compile pipeline easy to follow.
- Keywords/operators are centralized in `TypePascal`, so token classification rules live in one place.

## Tradeoffs

- Good for learning the complete flow in a small codebase.
- Parser is split by concern into partial files under `Compiler/FrontEnd/Parser/`.
- Runtime behavior depends on working directory only when no CLI input path is passed.

## How it works (flow)

1. `Main` resolves input path from `args[0]` when present, otherwise defaults to `while.pas`, then creates `Scanner(...)`.
2. `Scanner` reads the file as lowercase chars and classifies each char using `TypePascal.Get`.
3. Tokens are generated and printed to stdout.
4. `Parser` consumes the token list with `SetTokenIterator(...)` + `Parse()`.
5. Parser expects `program <id> ( <id> [, <id>]* );` before declarations.
6. During parse, symbols are inserted/looked up in `SymbolTable`.
7. Parser writes MEPA instructions to `Mepa.txt` in the current working directory.

## Correct usage example

From repo root:

```bash
xbuild Compiler.sln /p:Configuration=Debug
```

From `Compiler/Tests`:

```bash
mono ../bin/Debug/Compiler.exe
```

Why `Compiler/Tests`? default mode expects `while.pas` in the current working directory.

Optional custom input from repo root:

```bash
mono Compiler/bin/Debug/Compiler.exe Compiler/Tests/for.pas
```

## Common mistakes

- Running without CLI input outside `Compiler/Tests` (default `while.pas` not found).
- Using `examples/*.pas` fixtures for parser validation; many headers do not match current parser startup rule.
- Editing `keywords.txt` without preserving embedded resource loading (`Compiler.Resource.keywords.txt`).

## Safe refactor workflow

1. Choose one stage (`Scanner` or `Parser`) and change only that stage.
2. Rebuild.
3. Run smoke test with `Compiler/Tests/while.pas`.
4. Confirm token stream ends at `TK_EOF`.
5. Confirm `Mepa.txt` is still generated.
6. Run one additional fixture (`for.pas` or `ifElse.pas`).
