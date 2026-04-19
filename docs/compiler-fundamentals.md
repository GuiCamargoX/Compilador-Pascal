# Compiler Fundamentals (with Real Code) 🧠

This guide explains core compiler ideas using this exact repository.

If theory feels abstract, do not worry. We connect each concept to concrete files.

## 1) What is a compiler? 👋

A compiler transforms source code into another representation.

- input here: Pascal-like `.pas` files
- output here: MEPA-like instructions in `Mepa.txt`

In short: text in, structured instructions out.

## 2) Front-end vs back-end ⚙️

- **Front-end** understands the source language
  - lexer (tokens)
  - parser (grammar)
  - semantic checks (meaning)
- **Back-end** emits target representation

In this repository:

- front-end: `Scanner`, `Parser`, `SymbolTable`
- back-end-like step: MEPA emission through parser (`GenerateMepa(...)`)

## 3) The four core stages in this project 🔄

### Stage A: Lexical analysis (scanner)

- file: `Compiler/FrontEnd/Lexer/Scanner.cs`
- key method: `CheckCharacter(...)`
- job: turn characters into token objects

### Stage B: Syntax analysis (parser)

- files: `Compiler/FrontEnd/Parser/Parser*.cs`
- key methods: `Parse()`, `match(...)`
- job: verify token order follows grammar

### Stage C: Semantic analysis

- files: `Compiler/FrontEnd/Semantics/SymbolTable.cs` + parser rules
- key methods: `Insert(...)`, `Lookup(...)`, `OpenScope()`, `CloseScope()`
- job: validate declarations, uses, and scope

### Stage D: Code generation

- files: parser rules in `Compiler/FrontEnd/Parser/Parser*.cs`
- key method: `GenerateMepa(...)`
- job: write instructions to `Mepa.txt`

## 4) Why tokens matter 🧩

The parser does not parse raw text. It parses token categories like:

- `TK_PROGRAM`, `TK_BEGIN`, `TK_END`
- `TK_IDENTIFIER`, `TK_INTLIT`
- `TK_ASSIGNMENT`, `TK_PLUS`, `TK_LESS_THAN`

This separation keeps parsing logic simpler and more reliable.

## 5) Why a symbol table matters 🗂️

The symbol table stores identifier meaning:

- name
- category (variable, procedure, label)
- semantic type
- scope level
- generated address

Without this structure, parser rules can check syntax, but not correctness of usage.

## 6) Tiny end-to-end example ✨

Source (from `Compiler/Tests/while.pas`):

```pascal
x := 0;
while x < 10 do
begin
    writeln(x);
    x := x + 1;
end;
```

What happens conceptually:

1. lexer emits tokens (`TK_IDENTIFIER`, `TK_ASSIGNMENT`, `TK_INTLIT`, ...)
2. parser validates statement forms (`assignmentStat`, `whileStat`)
3. symbol table resolves `x`
4. parser emits MEPA lines (`CRCT`, `ARMZ`, `DSVF`, `SOMA`, ...)

## 7) Tradeoffs of this teaching design ⚖️

- strength: very traceable flow for beginners
- strength: compact codebase, easy to navigate
- tradeoff: legacy naming still exists in a few methods
- tradeoff: default run mode depends on current working directory

## 8) Recommended read order for first-time learners 📚

1. `Compiler/Program.cs`
2. `Compiler/FrontEnd/Lexer/Token.cs`
3. `Compiler/Tools/TypePascal.cs`
4. `Compiler/FrontEnd/Lexer/Scanner.cs`
5. `Compiler/FrontEnd/Semantics/Symbol.cs`
6. `Compiler/FrontEnd/Semantics/SymbolTable.cs`
7. `Compiler/FrontEnd/Parser/Parser.cs`
8. `Compiler/FrontEnd/Parser/Parser.Declarations.cs`
9. `Compiler/FrontEnd/Parser/Parser.Statements.cs`
10. `Compiler/FrontEnd/Parser/Parser.Expressions.cs`

Next: open `docs/how-it-works.md` and follow one complete execution run.
