# AGENTS.md

## Maintainer intent
- Keep this as a learning-first compiler repo: prioritize clarity of the Pascal -> token -> parse -> MEPA flow.
- Preserve runtime behavior unless a behavior change is explicitly approved.

## Project shape
- Single solution and project: `Compilador.sln` -> `Compilador/Compilador.csproj` (.NET Framework 4.6.1, non-SDK style).
- Entrypoint: `Compilador/Program.cs`.
- Compiler stages live in `Compilador/FrontEnd/` (`Scanner`, `Parser`, `SymbolTable`, tokens/symbols).
- Pascal keyword table is embedded from `Compilador/Resource/keywords.txt` and loaded by `Compilador/Tools/TypePascal.cs`.

## Exact run commands
- Build from repo root:
  - `msbuild Compilador.sln /p:Configuration=Debug`
- Run from `Compilador/Testes` (required for current hardcoded input file lookup):
  - Windows: `..\bin\Debug\Compilador.exe`
  - Linux/Mono: `mono ../bin/Debug/Compilador.exe`

## Critical coupling (easy to break)
- `Program.cs` currently hardcodes `new Scanner("while.pas")`; it does not consume CLI args.
- `Parser` writes `Mepa.txt` to the current working directory (`File.Open("Mepa.txt", FileMode.Create)`).
- Parser startup strictly expects `program <id> ( <id> [, <id>]* );` before declarations and `begin` block.
- Resource name coupling matters: embedded `keywords.txt` must remain resolvable as `Compilador.Resource.keywords.txt`.

## Verification (smoke)
1. Build solution in Debug.
2. Run executable from `Compilador/Testes`.
3. Confirm token stream prints to stdout and ends with `TK_EOF`.
4. Confirm `Mepa.txt` is generated/updated in the run directory.

## Repo gotchas
- Prefer `Compilador/Testes/*.pas` as parser fixtures; many `examples/*.pas` files use headers that do not match current parser expectation.
- Comment support is incomplete; unknown characters can throw scanner exception (`Unhandled element scanned`).
- `Compilador/FrontEnd/ParserAux.cs` is legacy/commented-out content and not part of active compiler flow.
