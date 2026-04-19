# Test Fixtures Guide 🧪

This folder contains small Pascal programs used to validate compiler behavior.

## Why this folder is important

- default run mode uses `while.pas` when no CLI argument is passed
- fixtures here match current parser expectations
- they are ideal for step-by-step learning and debugging

## Recommended fixture order for learners 📚

1. `while.pas` - baseline smoke test
2. `ifElse.pas` - conditionals
3. `for.pas` - iterative flow
4. `repeat.pas` - repeat/until loop
5. `case.pas` - case branch behavior
6. `goto.pas` - labels and jumps
7. `procSemPar.pas` - procedure call/declaration

## How to run

From repo root:

```bash
xbuild Compiler.sln /p:Configuration=Debug
```

Default fixture mode (run inside this folder):

```bash
mono ../bin/Debug/Compiler.exe
```

Explicit fixture mode (run from repo root):

```bash
mono Compiler/bin/Debug/Compiler.exe Compiler/Tests/for.pas
```

## What to verify ✅

1. token stream reaches `TK_EOF`
2. `Mepa.txt` is updated
3. no parser exception is thrown

## Common pitfalls ⚠️

- using `examples/` files for parser verification (many headers do not match startup grammar)
- running default mode outside this folder without CLI input path
