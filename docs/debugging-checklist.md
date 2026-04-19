# Debugging Checklist 🧯

Use this when your compiler changes break build, parsing, or generated output.

## 1) Fast sanity checks (do these first) ⚡

1. Build works:
   - `xbuild Compiler.sln /p:Configuration=Debug`
2. Run mode is correct:
   - no args -> run from `Compiler/Tests`
   - with args -> verify path exists
3. Scanner output reaches `TK_EOF`.
4. `Mepa.txt` gets regenerated.

## 2) If scanner fails 🔎

Symptoms:

- `Unhandled element scanned`

Check:

1. unsupported characters/comments in source
2. operator/token definitions in `Compiler/Tools/TypePascal.cs`
3. keyword list in `Compiler/Resource/keywords.txt`

## 3) If parser fails early 🧩

Symptoms:

- exception from `match(...)`

Check:

1. startup grammar: `program <id> ( <id> [, <id>]* );`
2. token stream near failure point
3. parser branch coverage in `Compiler/FrontEnd/Parser/Parser*.cs`
4. fixture choice (prefer `Compiler/Tests/*.pas`)

## 4) If semantic behavior is wrong 🗂️

Check:

1. declarations inserted into `SymbolTable`
2. lookups resolved in expected scope
3. `OpenScope()` / `CloseScope()` usage in parser rules

## 5) If MEPA output is wrong ⚙️

Check:

1. compare `Mepa.txt` before/after change
2. trace relevant parser rule and its `GenerateMepa(...)` calls
3. confirm expression order and jump labels (`DSVF`, `DSVS`) are consistent

## 6) Beginner-safe debugging routine 🪜

1. reproduce with `Compiler/Tests/while.pas`
2. isolate one parser or scanner function
3. add temporary print/log in one place only
4. re-run and capture token + failure point
5. remove temporary print
6. validate with second fixture (`for.pas` or `ifElse.pas`)
