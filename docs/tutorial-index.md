# Tutorial Index 🧭

Use this page to choose your next step based on your current level.

## Level 0 - "I just cloned the repo"

Read next:

1. `README.md`
2. `docs/architecture-map.md`

Theory goals:

- understand the compile pipeline at a bird's-eye level
- understand why output artifacts are part of debugging

Do this exercise:

1. Build from repo root:

```bash
xbuild Compiler.sln /p:Configuration=Debug
```

2. Run default fixture:

```bash
cd Compiler/Tests
mono ../bin/Debug/Compiler.exe
```

3. Run explicit input mode:

```bash
mono ../bin/Debug/Compiler.exe for.pas
```

You are ready for Level 1 when:

- output reaches `TK_EOF`
- `Mepa.txt` is generated

## Level 1 - "I do not know compilers yet"

Read next:

1. `docs/glossary.md`
2. `docs/compiler-fundamentals.md`

Theory goals:

- what a compiler is
- front-end vs semantic checks vs codegen
- why tokens and symbol tables exist

Do this exercise:

1. Explain in your own words:
   - what scanner does
   - what parser does
   - what symbol table does
2. Map each stage to one file path in this repo.

You are ready for Level 2 when:

- you can describe the pipeline: source -> tokens -> parse/semantics -> MEPA

## Level 2 - "I want to understand the lexer"

Read next:

1. `Compiler/Tools/TypePascal.cs`
2. `Compiler/FrontEnd/Lexer/Token.cs`
3. `Compiler/FrontEnd/Lexer/Word.cs`
4. `Compiler/FrontEnd/Lexer/Scanner.cs`

Theory goals:

- lexical grammar
- tokenization as a state-machine-like process
- why token boundaries matter for parser correctness

Do this exercise:

1. Open `Compiler/Tests/while.pas`.
2. Pick line `x := 0;`.
3. Predict tokens before running.
4. Run compiler and compare with printed token output.
5. Locate token creation in `generateToken(...)`.

You are ready for Level 3 when:

- you can explain how `CheckCharacter(...)` and `endOfWord()` produce tokens

## Level 2.5 - "I want to understand grammar"

Read next:

1. `docs/grammar-and-parsing.md`

Theory goals:

- terminals vs non-terminals
- BNF/EBNF notation
- recursive descent parsing
- precedence and associativity by grammar shape

Do this exercise:

1. Write EBNF for the program header rule.
2. Map each terminal to a `match(...)` call in `Parser.parse()`.
3. Write EBNF for expressions and map it to `Expressao()` -> `Termo()` -> `Fator()`.

You are ready for Level 3 when:

- you can explain how grammar becomes parser code

## Level 3 - "I want to understand parser + semantics"

Read next:

1. `Compiler/FrontEnd/Semantics/Symbol.cs`
2. `Compiler/FrontEnd/Semantics/SymbolTable.cs`
3. `Compiler/FrontEnd/Parser/Parser.cs`
4. `Compiler/FrontEnd/Parser/Parser.Declarations.cs`
5. `Compiler/FrontEnd/Parser/Parser.Statements.cs`
6. `Compiler/FrontEnd/Parser/Parser.Expressions.cs`
7. `docs/how-it-works.md`

Theory goals:

- syntax validation vs semantic validation
- scope resolution through symbol table lookup chain
- control-flow code generation via labels and jumps

Do this exercise:

1. Trace `whileStat()` in `Parser.Statements.cs`.
2. List emitted operations (`CMME`, `DSVF`, `DSVS`, etc.).
3. Compare with generated `Compiler/Tests/Mepa.txt`.
4. Trace one `SymbolTable.Lookup(...)` call in that flow.

You are ready for Level 4 when:

- you can explain where jumps and variable reads/writes are emitted

## Level 4 - "I want to add my first feature"

Read next:

1. `docs/implement-new-feature.md`
2. `docs/debugging-checklist.md`

Theory goals:

- grammar-first development
- preserving invariants while extending language
- regression discipline with smoke fixtures

Do this exercise:

1. Propose one tiny feature.
2. Write one fixture for it.
3. Plan 3 commits:
   - token/scanner updates
   - parser/semantic updates
   - docs + fixture notes
4. Verify default `while.pas` and explicit `for.pas` still pass.

You are ready for Level 5 when:

- your feature works and baseline fixtures still reach `TK_EOF`

## Level 5 - "I want to build my own compiler from zero"

Read next:

1. `docs/build-your-own-compiler.md`
2. `docs/types-in-this-compiler.md`

Theory goals:

- language design before implementation
- stage boundaries and information flow
- incremental construction of a compiler architecture

Do this exercise:

1. Create your own plan document (for example `docs/my-compiler-plan.md`).
2. Define:
   - language subset
   - token list
   - first grammar rules
   - symbol table fields
   - first 3 fixtures

You are ready to implement when:

- you can complete milestones 1 and 2 without guessing file boundaries

## If you are not sure where to start 🤔

Start at Level 1, then continue level-by-level.
