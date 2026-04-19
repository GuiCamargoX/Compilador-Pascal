# Types in This Compiler (No Confusion Edition) 🏷️

The word "type" appears at different layers.

This guide helps you separate those layers clearly.

## Layer 1: Language data types (Pascal source) 📝

These are types a programmer writes in source code:

- `integer`
- `real`
- `boolean`
- `char`
- `string`
- `array`

Where mapped in code:

- `Compiler/FrontEnd/Parser/Parser.cs` in `STRING_TYPE_HASH_MAP`

## Layer 2: Token types (lexer output) 🔤

These are categories produced by scanner.

Examples:

- keywords: `TK_PROGRAM`, `TK_VAR`, `TK_IF`, `TK_WHILE`
- identifiers/literals: `TK_IDENTIFIER`, `TK_INTLIT`, `TK_BOOLLIT`
- operators/symbols: `TK_ASSIGNMENT`, `TK_PLUS`, `TK_SEMI_COLON`

Where configured:

- keywords: `Compiler/Resource/keywords.txt`
- operators + char categories: `Compiler/Tools/TypePascal.cs`

## Layer 3: Internal semantic types (`Parser.TYPE`) 🧠

Parser uses compact internal values:

- `I` -> integer
- `R` -> real
- `B` -> boolean
- `C` -> char
- `S` -> string
- `P` -> procedure
- `L` -> label
- `A` -> array

Why this layer exists:

- parser and symbol table can reason with compact semantic categories

## End-to-end example ✨

Source:

```pascal
var x: integer;
```

Flow through layers:

1. scanner emits `TK_VAR TK_IDENTIFIER TK_COLON TK_INTEGER TK_SEMI_COLON`
2. parser (`VarDeclarations` in `Parser.Declarations.cs`) consumes tokens
3. parser maps `TK_INTEGER` -> `Parser.TYPE.I`
4. symbol table stores `x` with semantic type `I`

## Quick mental model 🧭

- language type = what user writes
- token type = what lexer emits
- semantic type = what compiler reasons with internally

## Common mistakes 🚨

- mixing token names with language types (`TK_INTEGER` is a token label, not a value type)
- adding keyword token support but skipping parser rule updates
- changing `Parser.TYPE` meaning without updating symbol-handling logic
