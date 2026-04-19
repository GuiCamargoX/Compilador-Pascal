# Guided Learning Path 🧭

Welcome! This is your step-by-step study plan if you are new to compilers.

You do not need to know compiler theory in advance. Follow the stages in order and run the checkpoints.

If you want a level-based route first, start at `docs/tutorial-index.md`.

## Stage 0: Setup and first run ⚙️

Goal: prove the project builds and runs on your machine.

Theory focus:

- observability-first development (run before modifying)
- compiler outputs are artifacts you can inspect (`TK_EOF`, `Mepa.txt`)

1. Build from repo root:

```bash
xbuild Compiler.sln /p:Configuration=Debug
```

2. Run default fixture from `Compiler/Tests`:

```bash
cd Compiler/Tests
mono ../bin/Debug/Compiler.exe
```

Checkpoint:

- token output reaches `TK_EOF`
- `Mepa.txt` appears/updates in current directory

## Stage 1: Understand the big picture 🧠

Read in this order:

1. `README.md`
2. `docs/compiler-fundamentals.md`
3. `docs/architecture-map.md`
4. `docs/glossary.md`

Theory focus:

- compiler pipeline as a composition of stages
- front-end responsibilities vs code generation responsibilities

Checkpoint:

- you can explain the pipeline: source -> tokens -> parse/semantics -> MEPA

## Stage 2: Learn the lexer (Scanner) 🔎

Read:

1. `Compiler/Tools/TypePascal.cs`
2. `Compiler/FrontEnd/Lexer/Token.cs`
3. `Compiler/FrontEnd/Lexer/Word.cs`
4. `Compiler/FrontEnd/Lexer/Scanner.cs`

Theory focus:

- lexical grammar (character patterns -> tokens)
- finite-state behavior via `Word` flags
- why parser should consume tokens, not raw characters

Hands-on exercise:

1. Run with `Compiler/Tests/while.pas`
2. Watch token lines in stdout
3. Pick one line from `while.pas` and map it to emitted tokens

Checkpoint:

- you understand where tokens are emitted (`generateToken(...)`)

## Stage 2.5: Learn grammar and parsing theory 📐

Read:

1. `docs/grammar-and-parsing.md`

Theory focus:

- what grammar is (BNF/EBNF)
- non-terminals and terminals
- recursive descent parsing
- precedence by grammar layering

Hands-on exercise:

1. Write EBNF for `while` statement.
2. Map your EBNF rule to `whileStat()` in `Parser.Statements.cs`.
3. Identify every `match(...)` corresponding to your terminals.

Checkpoint:

- you can map one EBNF rule directly to parser method calls

## Stage 3: Learn parser + semantic checks 🧩

Read:

1. `Compiler/FrontEnd/Semantics/Symbol.cs`
2. `Compiler/FrontEnd/Semantics/SymbolTable.cs`
3. `Compiler/FrontEnd/Parser/Parser.cs`
4. `Compiler/FrontEnd/Parser/Parser.Declarations.cs`
5. `Compiler/FrontEnd/Parser/Parser.Statements.cs`
6. `Compiler/FrontEnd/Parser/Parser.Expressions.cs`

Theory focus:

- syntax validation vs semantic validation
- statement dispatch using first-token decisions
- symbol scope and name resolution

Hands-on exercise:

1. Trace `whileStat()` in `Parser.Statements.cs`
2. Find the MEPA operations emitted for condition and jump
3. Compare with generated `Mepa.txt`

Checkpoint:

- you understand how parser rules call `GenerateMepa(...)`

## Stage 4: Learn type layers and symbol meaning 🏷️

Read:

1. `docs/types-in-this-compiler.md`

Theory focus:

- language type vs token type vs semantic type
- why compilers keep separate representations at each layer

Hands-on exercise:

1. Trace `var x: integer;`
2. Follow: language type -> token type -> internal `Parser.TYPE`
3. Confirm symbol insertion into `SymbolTable`

Checkpoint:

- you can explain why token type and semantic type are different

## Stage 5: Build confidence by running multiple fixtures 🧪

Run:

- default: `Compiler/Tests/while.pas`
- explicit CLI input: `Compiler/Tests/for.pas`
- optional extras: `ifElse.pas`, `repeat.pas`

Theory focus:

- differential validation (same compiler, multiple input programs)
- regression safety before/after refactors

Checkpoint:

- you can run both default mode and CLI-input mode

## Stage 6: Implement your first tiny feature ✨

Read:

1. `docs/implement-new-feature.md`
2. `docs/build-your-own-compiler.md`
3. `docs/debugging-checklist.md`

Theory focus:

- grammar-first implementation workflow
- syntax -> semantics -> codegen chain
- commit-sized scientific method: hypothesis, change, validate

Suggested first features:

- new reserved word
- new operator
- small parser rule extension

Checkpoint:

- feature added in small commits, with smoke validation after each commit
