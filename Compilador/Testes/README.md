# Test Fixtures Guide

This folder contains parser-focused Pascal fixtures used as smoke tests.

## Why these fixtures matter

`Program.cs` currently compiles a hardcoded file (`while.pas`), so this folder is the safest place to run baseline checks.

## Suggested fixture order

1. `while.pas` - baseline smoke test
2. `ifElse.pas` - conditional flow
3. `for.pas` - iterative flow and assignment updates
4. `repeat.pas` - post-test loop form
5. `case.pas` - branch selection
6. `goto.pas` - label and jump behavior
7. `procSemPar.pas` - procedure declaration and call

## Quick smoke workflow

1. build from repo root
2. run executable from `Compilador/Testes`
3. verify token stream reaches `TK_EOF`
4. verify `Mepa.txt` is created/updated

## Common fixture pitfalls

- Many files under `examples/` use program headers that do not match parser startup rule.
- Keep using this folder for parser validation unless you also adapt parser startup grammar.
