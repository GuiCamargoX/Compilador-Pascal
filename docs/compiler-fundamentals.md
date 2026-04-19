# Compiler Fundamentals for This Repository

This guide explains core compiler ideas using the code in this project.

## What is a compiler?

A compiler transforms source code into another representation.

- Input in this repo: Pascal-like `.pas` source
- Output in this repo: MEPA-like instructions in `Mepa.txt`

## Front-end vs back-end

- Front-end: understands source language (tokens, grammar, meaning)
- Back-end: generates target representation

In this repository:

- front-end is implemented by `Scanner`, `Parser`, and `SymbolTable`
- back-end is simple MEPA emission inside parser methods (`GenerateMepa`)

## Core phases in practice

1. Lexical analysis (scanner)
   - file: `Compiler/FrontEnd/Scanner.cs`
   - role: convert characters into token stream

2. Syntax analysis (parser)
   - file: `Compiler/FrontEnd/Parser.cs`
   - role: check token order follows grammar rules

3. Semantic checks
   - files: `Compiler/FrontEnd/Parser.cs`, `Compiler/FrontEnd/SymbolTable.cs`
   - role: verify declarations, uses, scope, and typing context

4. Intermediate code generation
   - file: `Compiler/FrontEnd/Parser.cs`
   - role: write MEPA-like instructions to `Mepa.txt`

## Why tokens matter

Tokens are the parser's vocabulary. Instead of parsing raw characters, parser rules operate on token categories like:

- `TK_PROGRAM`, `TK_BEGIN`, `TK_END`
- `TK_IDENTIFIER`, `TK_INTLIT`
- `TK_ASSIGNMENT`, `TK_PLUS`, `TK_LESS_THAN`

This separation makes parser logic cleaner and easier to reason about.

## Why symbol tables matter

The symbol table stores information about identifiers:

- name
- kind (variable, procedure, label)
- data type
- scope level
- address used during code generation

Without this table, the parser could read syntax but could not validate meaning.

## Design tradeoffs in this repo

- Strength: clear end-to-end pipeline for learning
- Strength: small codebase makes tracing easier
- Tradeoff: parser file is large, so scaling grammar is harder
- Tradeoff: runtime depends on current working directory and hardcoded input file

## Read-order recommendation for beginners

1. `Compiler/Program.cs`
2. `Compiler/FrontEnd/Token.cs`
3. `Compiler/Tools/TypePascal.cs`
4. `Compiler/FrontEnd/Scanner.cs`
5. `Compiler/FrontEnd/Symbol.cs`
6. `Compiler/FrontEnd/SymbolTable.cs`
7. `Compiler/FrontEnd/Parser.cs`
