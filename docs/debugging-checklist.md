# Debugging Checklist

Use this when a change breaks compilation or output.

## Quick checks

1. Confirm you built `Compilador.sln` in Debug.
2. Confirm you run from `Compilador/Testes`.
3. Confirm `while.pas` exists in the current directory.
4. Confirm scanner output reaches `TK_EOF`.
5. Confirm `Mepa.txt` was regenerated.

## If scanner fails

- Symptom: exception `Unhandled element scanned`.
- Check for unsupported characters/comments in source.
- Check operator/token maps in `Compilador/Tools/TypePascal.cs`.

## If parser fails early

- Symptom: parser throws during `match(...)`.
- Check the program header format: `program <id> ( <id> [, <id>]* );`.
- Prefer fixtures from `Compilador/Testes/`.

## If MEPA output looks wrong

- Compare generated `Mepa.txt` before/after your change.
- Trace parser rule entry points in `Compilador/FrontEnd/Parser.cs` for the construct under test.
- Check symbol insertion and lookup paths in `Compilador/FrontEnd/SymbolTable.cs`.

## Beginner-safe investigation routine

1. Reproduce with `while.pas`.
2. Add one temporary print in one function only.
3. Re-run and capture token order and failing token type.
4. Remove temporary print after understanding the issue.
5. Re-test with a second fixture.
