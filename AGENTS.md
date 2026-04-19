# AGENTS.md

## Maintainer intent
- Keep this as a learning-first compiler repo: prioritize clarity of the Pascal -> token -> parse -> MEPA flow.
- Preserve runtime behavior unless a behavior change is explicitly approved.

## Project shape
- Single solution and project: `Compiler.sln` -> `Compiler/Compiler.csproj` (.NET Framework 4.6.1, non-SDK style).
- Entrypoint: `Compiler/Program.cs`.
- Compiler stages live in `Compiler/FrontEnd/` with subfolders: `Lexer/`, `Parser/`, and `Semantics/`.
- Pascal keyword table is embedded from `Compiler/Resource/keywords.txt` and loaded by `Compiler/Tools/TypePascal.cs`.

## Exact run commands
- Build from repo root:
  - `xbuild Compiler.sln /p:Configuration=Debug`
- Run default fixture from `Compiler/Tests`:
  - Windows: `..\bin\Debug\Compiler.exe`
  - Linux/Mono: `mono ../bin/Debug/Compiler.exe`
- Optional custom input path:
  - Linux/Mono: `mono Compiler/bin/Debug/Compiler.exe Compiler/Tests/for.pas`

## Critical coupling (easy to break)
- `Program.cs` uses CLI arg `args[0]` when provided; otherwise it falls back to `while.pas` in the current working directory.
- `Parser` writes `Mepa.txt` to the current working directory (`File.Open("Mepa.txt", FileMode.Create)`).
- Parser startup strictly expects `program <id> ( <id> [, <id>]* );` before declarations and `begin` block.
- Resource name coupling matters: embedded `keywords.txt` must remain resolvable as `Compiler.Resource.keywords.txt`.

## Verification (smoke)
1. Build solution in Debug.
2. Run executable from `Compiler/Tests` (default mode) or pass explicit file path.
3. Confirm token stream prints to stdout and ends with `TK_EOF`.
4. Confirm `Mepa.txt` is generated/updated in the run directory.

## Repo gotchas
- Prefer `Compiler/Tests/*.pas` as parser fixtures; many `examples/*.pas` files use headers that do not match current parser expectation.
- Comment support is incomplete; unknown characters can throw scanner exception (`Unhandled element scanned`).
- `Compiler/FrontEnd/Parser/ParserAux.cs` is legacy/commented-out content and not part of active compiler flow.
