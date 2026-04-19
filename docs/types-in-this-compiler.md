# Types in This Compiler

The word "type" appears in multiple layers. This document separates them clearly.

## 1) Language-level Pascal data types

These are source-language concepts used in declarations:

- `integer`
- `real`
- `boolean`
- `char`
- `string`
- `array`

Where this mapping is defined:

- `Compiler/FrontEnd/Parser.cs` in `STRING_TYPE_HASH_MAP`

## 2) Token types from lexical analysis

These are categories produced by scanner.

Examples:

- keywords: `TK_PROGRAM`, `TK_VAR`, `TK_IF`, `TK_WHILE`
- identifiers/literals: `TK_IDENTIFIER`, `TK_INTLIT`, `TK_BOOLLIT`
- operators/symbols: `TK_ASSIGNMENT`, `TK_PLUS`, `TK_SEMI_COLON`

Where tokens are configured:

- keywords from `Compiler/Resource/keywords.txt`
- operators in `Compiler/Tools/TypePascal.cs`

## 3) Internal semantic types (`Parser.TYPE`)

Parser uses compact internal enum values:

- `I` -> integer
- `R` -> real
- `B` -> boolean
- `C` -> char
- `S` -> string
- `P` -> procedure
- `L` -> label
- `A` -> array

Why this exists:

- semantic checks and symbol table operations can use compact typed values

## Example: how one declaration flows through layers

Source:

```pascal
var x: integer;
```

Flow:

1. scanner emits tokens like `TK_VAR`, `TK_IDENTIFIER`, `TK_COLON`, `TK_INTEGER`, `TK_SEMI_COLON`
2. parser rule `VarDeclarations` consumes these tokens
3. parser maps `TK_INTEGER` to internal `Parser.TYPE.I`
4. symbol table stores `x` as variable with type `I` and an address

## Common mistakes

- Confusing token type with language type (for example, `TK_INTEGER` is a token label, not the runtime value)
- Adding a keyword in `keywords.txt` but forgetting parser rules for the new token
- Changing internal enum semantics without updating symbol handling and parser branches
