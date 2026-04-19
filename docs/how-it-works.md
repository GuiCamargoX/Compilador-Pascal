# How This Compiler Works (Step by Step) 🧩

This guide walks through the real execution flow in code, from input file to MEPA output.

If you open files while reading, you will understand the compiler much faster. ✅

## 1) Pick the source file 📄

Start in `Compiler/Program.cs`:

- `Main(...)` calls `ResolveSourceFile(args)`
- if `args[0]` exists, compiler uses it
- otherwise it falls back to `while.pas`

Then `Main(...)` creates the scanner and parser pipeline:

1. `new Scanner(sourceFile).GetTokens()`
2. `Parser.SetTokenIterator(tokens)`
3. `Parser.Parse()`

## 2) Turn characters into tokens 🔤

Open `Compiler/FrontEnd/Lexer/Scanner.cs`.

What happens:

1. file is read in constructor (`Scanner(string namePath)`)
2. text is converted to lowercase chars
3. each char goes through `CheckCharacter(...)`
4. token objects are emitted with `generateToken(...)`

Important helpers:

- `TypePascal.Get(...)` in `Compiler/Tools/TypePascal.cs` classifies each char
- `Word` in `Compiler/FrontEnd/Lexer/Word.cs` tracks the current lexeme state
- `Token` in `Compiler/FrontEnd/Lexer/Token.cs` stores token type/value/position

## 3) Parse the token stream 🧠

Open `Compiler/FrontEnd/Parser/Parser.cs` and parser partial files.

Parser entry:

- `Parse()` delegates to `parse()`
- parser starts by matching program header:
  - `program <id> ( <id> [, <id>]* );`

Parser organization:

- `Parser.Declarations.cs`: labels, variables, procedures
- `Parser.Statements.cs`: `if`, `while`, `for`, assignment, read/write, etc.
- `Parser.Expressions.cs`: expression grammar (`Expressao`, `Termo`, `Fator`)

Core parser mechanics:

- `match(...)` validates expected token order
- `getToken()` advances token iterator
- when parser sees identifiers, it consults symbol table via `SymbolTable.Lookup(...)`

Grammar perspective:

- parser methods are production implementations
- `match(...)` corresponds to terminal consumption
- method calls between parser methods correspond to non-terminal expansion

Mini diagram:

```text
Production (grammar)
      |
      v
Parser method
      |
      +--> match(TOKEN_A)
      +--> call OtherRule()
      +--> match(TOKEN_B)
```

## 4) Semantic checks and symbol tracking 🗂️

Open `Compiler/FrontEnd/Semantics/SymbolTable.cs`.

What this stage does:

- stores identifier metadata (name, kind, scope, address)
- resolves references through current and outer scopes
- manages scope stack (`OpenScope`, `CloseScope`)

Where parser uses it:

- declarations call `Insert(...)`
- statements/expressions call `Lookup(...)`

## 5) Emit MEPA instructions ⚙️

Code generation is done inside parser rules.

Key method:

- `GenerateMepa(label, code, param)` in `Compiler/FrontEnd/Parser/Parser.cs`

Output file:

- opened in `openFileOutput()` as `Mepa.txt`
- written in current working directory

## 6) Concrete example with real files 🧪

Input fixture:

- `Compiler/Tests/while.pas`

Run (default mode):

```bash
cd Compiler/Tests
mono ../bin/Debug/Compiler.exe
```

Token output (excerpt):

```text
tokenType: TK_PROGRAM  Lexema: program
tokenType: TK_IDENTIFIER  Lexema: whileprogram
...
tokenType: TK_WHILE  Lexema: while
...
tokenType: TK_EOF  Lexema: EOF
```

MEPA output (excerpt from `Compiler/Tests/Mepa.txt`):

```text
INPP
AMEM 1
CRCT 0
ARMZ 0,0
...
DSVF L2
...
DMEM 1
```

Custom input mode:

```bash
mono Compiler/bin/Debug/Compiler.exe Compiler/Tests/for.pas
```

## 6.1) Grammar-to-code example: `while` 🔁

Grammar idea (EBNF):

```text
WhileStatement -> 'while' Expression 'do' Statement
```

Implementation path:

- method: `whileStat()` in `Compiler/FrontEnd/Parser/Parser.Statements.cs`
- key sequence:
  - `match("TK_WHILE")`
  - `Expressao()`
  - `match("TK_DO")`
  - `statements()`

Code-generation side effects:

- creates loop labels with `Next_Label()`
- emits conditional jump (`DSVF`) and back jump (`DSVS`)

Control-flow diagram:

```text
L_start: evaluate condition
         if false -> L_end
         body
         jump L_start
L_end:   continue
```

## 7) Why this design works for learning 🎓

- pipeline stages are explicit and easy to trace
- parser rules are visible in one folder and split by concern
- fixtures in `Compiler/Tests/` let you test one concept at a time

## 8) Common pitfalls (and how to avoid them) 🚨

- no CLI argument + wrong working directory -> default `while.pas` not found
- changing lexer and parser in same commit -> debugging becomes hard
- using `examples/*.pas` for parser validation -> header mismatch in many cases

## 9) Safe workflow for experiments 🔒

1. change one stage only
2. run `xbuild Compiler.sln /p:Configuration=Debug`
3. run default `while.pas` fixture
4. confirm token output reaches `TK_EOF`
5. inspect `Mepa.txt`
6. run one more fixture (for example `for.pas`)
