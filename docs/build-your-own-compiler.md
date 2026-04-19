# How to Build a Compiler (Concept Roadmap)

This is a language-agnostic roadmap for beginners, connected to this repository.

## 1) Choose your source language subset

Do not start with a full language.

- define minimal grammar first
- include variables, assignment, expressions, conditionals, and loops

In this repo, the subset includes statements like `if`, `while`, `for`, `repeat`, `case`, and simple procedures.

## 2) Define lexical vocabulary

Before writing parser rules, define what token kinds exist.

- keywords
- operators
- identifier and literal rules

In this repo, vocabulary is driven by:

- `Compiler/Resource/keywords.txt`
- `Compiler/Tools/TypePascal.cs`

## 3) Build scanner (lexer)

Input: characters. Output: tokens.

Goals:

- split source text into meaningful token units
- attach source position for debugging

In this repo: `Compiler/FrontEnd/Lexer/Scanner.cs`.

## 4) Build parser

Input: tokens. Output: validated structure/logic.

Goals:

- enforce grammar order
- call semantic checks at key points

In this repo: `Compiler/FrontEnd/Parser/Parser.cs`.

## 5) Add semantic model

Parser alone checks form, not meaning.

You need symbol tracking:

- declarations
- scope levels
- identifier categories and types

In this repo: `Compiler/FrontEnd/Semantics/SymbolTable.cs` and `Compiler/FrontEnd/Semantics/Symbol.cs`.

## 6) Generate target representation

After syntax + semantics, emit instructions.

- choose target format (bytecode, assembly, IR)
- map each statement/expression rule to emitted operations

In this repo, the target is MEPA-like text in `Mepa.txt`.

## 7) Validate with tiny fixtures

Small test programs are essential.

- one fixture per feature
- one smoke test for full pipeline

In this repo, start with `Compiler/Tests/while.pas`.

## Recommended implementation order for a new compiler

1. literals and identifiers
2. arithmetic expressions
3. assignment statements
4. `if/else`
5. `while`
6. declarations + symbol table
7. procedures/scopes

Each step should include one tiny fixture and one expected output check.
