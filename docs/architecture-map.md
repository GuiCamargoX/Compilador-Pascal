# Architecture Map 🗺️

Use this file as your quick mental map before diving into code.

## High-level boundaries

- `Compiler/Program.cs`
  - application entrypoint
  - wires scanner output into parser input

- `Compiler/FrontEnd/Lexer/Scanner.cs`
  - lexical analysis (characters -> tokens)

- `Compiler/FrontEnd/Parser/Parser*.cs`
  - grammar rules
  - semantic checks
  - MEPA code emission

- `Compiler/FrontEnd/Semantics/SymbolTable.cs` and `Compiler/FrontEnd/Semantics/Symbol.cs`
  - identifier metadata
  - scope stack behavior

- `Compiler/Tools/TypePascal.cs`
  - keyword and operator classification tables
  - character categories used by scanner

## Runtime call flow (in order) 🔄

1. `Program.Main`
2. resolve source path (`args[0]` or default `while.pas`) and call `new Scanner(sourcePath).GetTokens()`
3. `Parser.SetTokenIterator(tokens)`
4. `Parser.Parse()`
5. parser emits `Mepa.txt`

## Core data objects 📦

- `Token`
  - `TokenType`, `TokenValue`, `LineCol`, `LineRow`

- `Symbol`
  - name, token kind, semantic type, scope level, address

- `Parser.TYPE`
  - internal semantic categories (`I`, `R`, `B`, `C`, `S`, `P`, `L`, `A`)

## Easy-to-break couplings ⚠️

- default source filename `while.pas` is resolved relative to current working directory when no CLI argument is provided
- parser startup grammar requires parameter list in program header
- `Mepa.txt` output depends on current working directory
- keyword resource name must match embedded manifest path

## Suggested reading strategy 📚

1. read `Program.cs` to see wiring
2. inspect `Token.cs` and `TypePascal.cs` for lexical vocabulary
3. read scanner flow in `Scanner.cs`
4. read symbol model (`Symbol.cs`, `SymbolTable.cs`)
5. read parser top-down starting from `Parser.cs` (`parse()`), then continue in the other parser partial files
