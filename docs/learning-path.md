# Guided Learning Path

Follow this order to learn compiler construction with this repository.

## Stage 1: Understand the pipeline

Read:

1. `README.md`
2. `docs/compiler-fundamentals.md`
3. `docs/architecture-map.md`

Goal: understand the end-to-end flow before reading details.

## Stage 2: Learn tokens and scanner behavior

Read:

1. `Compilador/Tools/TypePascal.cs`
2. `Compilador/FrontEnd/Token.cs`
3. `Compilador/FrontEnd/Word.cs`
4. `Compilador/FrontEnd/Scanner.cs`

Exercise:

- run with `Compilador/Testes/while.pas`
- observe token stream in stdout

## Stage 3: Learn parser and semantics

Read:

1. `Compilador/FrontEnd/Symbol.cs`
2. `Compilador/FrontEnd/SymbolTable.cs`
3. `Compilador/FrontEnd/Parser.cs`

Exercise:

- trace one statement path (`ifStat`, `whileStat`, or `forStat`)
- map consumed tokens to generated MEPA operations

## Stage 4: Learn type layers

Read:

- `docs/types-in-this-compiler.md`

Exercise:

- pick one declaration and trace language type -> token type -> internal semantic type

## Stage 5: Practice extension safely

Read:

1. `docs/implement-new-feature.md`
2. `docs/build-your-own-compiler.md`
3. `docs/debugging-checklist.md`

Exercise:

- propose one tiny feature
- write one fixture
- implement in small reversible commits
