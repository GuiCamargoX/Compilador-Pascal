# How This Compiler Works

This document explains the current implementation as it exists in code.

## What is implemented

- Scanner: reads Pascal source and emits tokens.
- Parser: validates grammar, performs semantic checks, and emits MEPA instructions.
- Symbol table: stores variables, procedures, labels, and scope information.

Core wiring:

- `Compilador/Program.cs`
- `Compilador/FrontEnd/Scanner.cs`
- `Compilador/FrontEnd/Parser.cs`
- `Compilador/FrontEnd/SymbolTable.cs`
- `Compilador/Tools/TypePascal.cs`

## Why this design

- Stages are explicit (`Scanner` -> `Parser` -> output), which makes the compile pipeline easy to follow.
- Keywords/operators are centralized in `TypePascal`, so token classification rules live in one place.

## Tradeoffs

- Good for learning the complete flow in a small codebase.
- Parser is a large single file, so adding grammar features can become hard to maintain.
- Runtime behavior depends on working directory and hardcoded input filename.

## How it works (flow)

1. `Main` creates `Scanner("while.pas")`.
2. `Scanner` reads the file as lowercase chars and classifies each char using `TypePascal.Get`.
3. Tokens are generated and printed to stdout.
4. `Parser` consumes the token list with `SetTokenArrayListIterator` + `parse()`.
5. Parser expects `program <id> ( <id> [, <id>]* );` before declarations.
6. During parse, symbols are inserted/looked up in `SymbolTable`.
7. Parser writes MEPA instructions to `Mepa.txt` in the current working directory.

## Correct usage example

From repo root:

```bash
msbuild Compilador.sln /p:Configuration=Debug
```

From `Compilador/Testes`:

```bash
mono ../bin/Debug/Compilador.exe
```

Why `Compilador/Testes`? `Program.cs` uses `while.pas` by relative path.

## Common mistakes

- Running executable outside `Compilador/Testes` (input file not found).
- Using `examples/*.pas` fixtures for parser validation; many headers do not match current parser startup rule.
- Editing `keywords.txt` without preserving embedded resource loading (`Compilador.Resource.keywords.txt`).

## Safe refactor workflow

1. Choose one stage (`Scanner` or `Parser`) and change only that stage.
2. Rebuild.
3. Run smoke test with `Compilador/Testes/while.pas`.
4. Confirm token stream ends at `TK_EOF`.
5. Confirm `Mepa.txt` is still generated.
6. Run one additional fixture (`for.pas` or `ifElse.pas`).
