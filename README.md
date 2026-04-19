# Build a Tiny Pascal Compiler in C# 🚀

Welcome! 👋 This repository is a beginner-friendly compiler tutorial.

If you have never built a compiler before, you are in the right place. The goal here is to learn the full pipeline from source code to intermediate instructions, step by step.

## What you will learn 🎯

- what a compiler is and how it is different from an interpreter
- how lexical analysis turns text into tokens
- how parsing checks grammar and structure
- how semantic checks use a symbol table
- how code generation emits MEPA-like instructions

## Quick start (5 minutes) ⚡

From repository root:

```bash
xbuild Compiler.sln /p:Configuration=Debug
```

Run the default fixture from `Compiler/Tests`:

```bash
mono ../bin/Debug/Compiler.exe
```

What should happen:

- tokens are printed to stdout
- output reaches `TK_EOF`
- `Mepa.txt` is created/updated in your current directory

Run a custom input file (from repo root):

```bash
mono Compiler/bin/Debug/Compiler.exe Compiler/Tests/for.pas
```

## What is a compiler? 🧠

A compiler translates source code from one representation to another.

- source in this repo: Pascal-like `.pas` programs
- target in this repo: MEPA-like instructions in `Mepa.txt`

Many compilers follow the same high-level stages. This repository intentionally keeps those stages explicit so you can study them.

## The pipeline in this repository 🔄

1. **Select input file** in `Compiler/Program.cs` (`ResolveSourceFile`)
2. **Scan characters into tokens** in `Compiler/FrontEnd/Lexer/Scanner.cs`
3. **Parse + check meaning** in `Compiler/FrontEnd/Parser/Parser*.cs`
4. **Track identifiers and scope** in `Compiler/FrontEnd/Semantics/SymbolTable.cs`
5. **Emit MEPA instructions** in parser methods via `GenerateMepa(...)`

## What is implemented here ✅

- lexical analysis (`Scanner`) with token stream output
- syntax + semantic checks (`Parser` + `SymbolTable`)
- MEPA-like code generation into `Mepa.txt`

Supported language subset (practical view):

- declarations: `label`, `var`, `procedure`
- commands: assignment, `if/else`, `while`, `repeat/until`, `for`, `case`, `goto`, `read`, `write`
- expressions: arithmetic, relational, and boolean operators

## Follow the code, step by step 🛠️

Use this as your first code-reading route:

1. `Compiler/Program.cs`
2. `Compiler/FrontEnd/Lexer/Token.cs`
3. `Compiler/Tools/TypePascal.cs`
4. `Compiler/FrontEnd/Lexer/Scanner.cs`
5. `Compiler/FrontEnd/Semantics/Symbol.cs`
6. `Compiler/FrontEnd/Semantics/SymbolTable.cs`
7. `Compiler/FrontEnd/Parser/Parser.cs`
8. `Compiler/FrontEnd/Parser/Parser.Declarations.cs`
9. `Compiler/FrontEnd/Parser/Parser.Statements.cs`
10. `Compiler/FrontEnd/Parser/Parser.Expressions.cs`

## Beginner-friendly examples 🧪

- first fixture: `Compiler/Tests/while.pas`
- second fixture: `Compiler/Tests/for.pas`
- generated output example: `Compiler/Tests/Mepa.txt`

Tip: run `while.pas`, inspect tokens, then inspect `Mepa.txt` immediately after.

## Common beginner mistakes ❗

- running without a CLI argument outside `Compiler/Tests` (default `while.pas` cannot be found)
- editing multiple compiler stages at once (hard to debug)
- testing with `examples/*.pas` and expecting parser parity with `Compiler/Tests/*.pas`
- expecting full comment support (this is still incomplete)

## Safe learning workflow 🔒

1. change only one stage at a time (lexer or parser)
2. rebuild
3. run default `while.pas` smoke test
4. confirm `TK_EOF`
5. compare `Mepa.txt` before/after
6. validate with one extra fixture (`for.pas`, `ifElse.pas`)

## Documentation map 📚

- start here: `docs/learning-path.md`
- concepts: `docs/compiler-fundamentals.md`
- end-to-end trace with code references: `docs/how-it-works.md`
- architecture overview: `docs/architecture-map.md`
- type layers explained: `docs/types-in-this-compiler.md`
- build one from scratch roadmap: `docs/build-your-own-compiler.md`
- add features safely: `docs/implement-new-feature.md`
- debugging flow: `docs/debugging-checklist.md`
