# Build Your Own Compiler (Beginner Roadmap) 🏗️

This is the "from zero to working compiler" plan.

You can use it for any language, and this repository gives concrete examples for each step.

## Before you start ✅

Keep scope small. A tiny working compiler teaches more than a giant unfinished one.

Start with:

- variables
- assignment
- arithmetic expressions
- one loop or condition

## Milestone 1: Define your language subset ✍️

Goal: decide exactly what syntax you support.

Theory lens:

- language design always starts with constraints
- smaller language subset reduces grammar ambiguity and implementation risk

Do:

1. write 3-5 sample programs
2. list which statements and expressions are allowed
3. freeze this list for your first version

In this repo, that subset includes `if`, `while`, `for`, `repeat`, `case`, and simple procedures.

## Milestone 2: Define tokens (vocabulary) 🔤

Goal: decide what token categories exist.

Theory lens:

- lexical analysis is typically modeled with regular languages and finite-state automata
- scanner converts a character stream into a symbol stream for parser consumption

Do:

1. list keywords
2. list operators/symbols
3. define literal and identifier rules

In this repo:

- keywords source: `Compiler/Resource/keywords.txt`
- operator and char classification: `Compiler/Tools/TypePascal.cs`

Done when:

- every language symbol maps to a token type

## Milestone 3: Build scanner / lexer 🔎

Goal: convert character stream to token stream.

Do:

1. read source text
2. classify characters
3. build lexemes
4. emit tokens with line/column

In this repo:

- scanner: `Compiler/FrontEnd/Lexer/Scanner.cs`
- token object: `Compiler/FrontEnd/Lexer/Token.cs`
- temporary lexeme state: `Compiler/FrontEnd/Lexer/Word.cs`

Done when:

- token stream ends with `TK_EOF`

Conceptual diagram:

```text
Characters -> Lexeme state -> Token object -> Token stream
```

## Milestone 4: Build parser 🧩

Goal: validate grammar structure from token stream.

Theory lens:

- parser works on context-free grammar rules
- recursive descent maps non-terminals to functions
- `match(...)` corresponds to consuming terminal symbols

Do:

1. define parser entry point
2. implement grammar rules for declarations/statements/expressions
3. fail fast when token order is invalid

In this repo:

- entry: `Parse()` in `Compiler/FrontEnd/Parser/Parser.cs`
- declarations: `Parser.Declarations.cs`
- statements: `Parser.Statements.cs`
- expressions: `Parser.Expressions.cs`

Done when:

- valid programs parse completely
- invalid programs fail at clear token mismatch points

Conceptual diagram:

```text
Grammar rule (EBNF)
        |
        v
Parser method
        |
        v
match(...) + nested rule calls
```

## Milestone 5: Add semantic checks 🧠

Goal: check meaning (not only syntax).

Theory lens:

- static semantics checks validity without running the program
- symbol table is the central structure for declarations, types, and scope

Do:

1. create symbol table
2. insert declarations
3. resolve uses with scope rules

In this repo:

- symbol model: `Compiler/FrontEnd/Semantics/Symbol.cs`
- symbol table: `Compiler/FrontEnd/Semantics/SymbolTable.cs`

Done when:

- undeclared identifiers are not silently accepted

## Milestone 6: Generate target instructions ⚙️

Goal: emit executable/intermediate representation.

Theory lens:

- code generation lowers high-level constructs into lower-level operations
- this project targets a stack-style intermediate language (MEPA-like)

In this repo:

- generation helper: `GenerateMepa(...)`
- output file: `Mepa.txt`

Done when:

- a valid fixture produces stable instruction output

## Milestone 7: Validate with tiny fixtures 🧪

Use one small file per feature and one smoke test for full flow.

Start with:

- `Compiler/Tests/while.pas`

Smoke checklist:

1. build (`xbuild Compiler.sln /p:Configuration=Debug`)
2. run default or CLI input mode
3. confirm `TK_EOF`
4. inspect `Mepa.txt`

Theory lens:

- verification should include both structural correctness (tokens, parse) and behavioral artifacts (generated instructions)

## Suggested implementation order for a brand-new compiler 🪜

1. identifiers + integer literals
2. arithmetic expressions
3. assignment
4. `if/else`
5. `while`
6. declarations + symbol table
7. procedures/scopes

At each step: one fixture, one expected output, one focused commit.
