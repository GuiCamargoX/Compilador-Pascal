# Grammar and Parsing (from Theory to Code) 🧩

If you ever asked "what is grammar in a compiler?", this is your page.

This guide explains the theory, then shows exactly how this repository implements it.

## 1) What is grammar? 📘

In compiler theory, a **grammar** is a formal description of valid program structure.

Think of grammar as the language's construction rules:

- which tokens can appear
- in what order
- with what nesting

Example idea:

- valid: `x := 1;`
- invalid: `:= x 1;`

Both strings contain known tokens, but only the first follows grammar order.

## 2) Two grammars you should not confuse 🔀

### Lexical grammar (scanner level)

Defines how characters form tokens:

- identifiers
- integer literals
- operators
- keywords

In this repo, lexical rules are implemented in:

- `Compiler/Tools/TypePascal.cs`
- `Compiler/FrontEnd/Lexer/Scanner.cs`

### Syntax grammar (parser level)

Defines how tokens form language constructs:

- declarations
- statements
- expressions

In this repo, syntax rules are implemented in:

- `Compiler/FrontEnd/Parser/Parser.cs`
- `Compiler/FrontEnd/Parser/Parser.Declarations.cs`
- `Compiler/FrontEnd/Parser/Parser.Statements.cs`
- `Compiler/FrontEnd/Parser/Parser.Expressions.cs`

## 3) Grammar notation (BNF/EBNF) in 60 seconds ✍️

You will often see rules like this:

```text
Program -> 'program' id '(' id {',' id} ')' ';' Block '.' EOF
```

Meaning:

- `->` means "is defined as"
- quoted words are terminals (actual token values)
- names like `Program`, `Block` are non-terminals (grammar concepts)
- `{ ... }` means repetition (zero or more)
- `[ ... ]` means optional

## 4) How this repo implements grammar (recursive descent) 🧠

This project uses **recursive descent parsing**:

- each non-terminal is implemented as a C# method
- `match("TOKEN")` consumes an expected terminal token
- methods call other methods to express nesting

Core primitive:

- `match(...)` in `Compiler/FrontEnd/Parser/Parser.cs`

### Mapping diagram

```text
Grammar non-terminal        Parser method
-----------------------     -------------------------------
Program                     parse()
Block                       block()
BeginEnd                    comando_Begin()
Statements                  statements()
Expression                  Expressao()
SimpleExpression            ExpressaoSimples()
Term                        Termo()
Factor                      Fator()
```

## 5) Concrete example: program header grammar ✅

### Grammar rule (EBNF)

```text
Program
  -> 'program' id '(' id {',' id} ')' ';' Block '.' EOF
```

### Implemented in code

`Compiler/FrontEnd/Parser/Parser.cs` inside `parse()`:

- `match("TK_PROGRAM")`
- `match("TK_IDENTIFIER")`
- `match("TK_OPEN_PARENTHESIS")`
- `match("TK_IDENTIFIER")`
- loop for `TK_COMMA` + `TK_IDENTIFIER`
- `match("TK_CLOSE_PARENTHESIS")`
- `match("TK_SEMI_COLON")`
- parse `block()`
- `match("TK_DOT")`, `match("TK_EOF")`

### Flow diagram

```text
[Start parse]
   |
   v
match TK_PROGRAM
   |
   v
match TK_IDENTIFIER
   |
   v
match '(' id {, id} ')'
   |
   v
match ';' -> block() -> match '.' -> match EOF
```

## 6) Concrete example: expression grammar and precedence 📐

Expression parsing is split into layered methods to enforce precedence.

### Grammar rule (EBNF)

```text
Expression       -> SimpleExpression [RelOp SimpleExpression]
SimpleExpression -> Term {AddOp Term}
Term             -> Factor {MulOp Factor}
Factor           -> id | intlit | boollit | '(' Expression ')' | 'not' Factor
```

### Why this matters

Because `Term` is inside `SimpleExpression`, multiplication/division bind tighter than addition/subtraction.

### Implemented in code

- `Expressao()` -> relation layer
- `ExpressaoSimples()` -> `+`, `-`, `or`
- `Termo()` -> `*`, `/`, `and`
- `Fator()` -> atoms and parentheses

All in `Compiler/FrontEnd/Parser/Parser.Expressions.cs`.

### Precedence diagram

```text
Highest precedence: Factor
        |
        v
      Term (*, /, and)
        |
        v
SimpleExpression (+, -, or)
        |
        v
 Expression (<, <=, >, >=, =, <>)
Lowest precedence
```

## 7) Concrete example: statement grammar to method dispatch 🧭

`statements()` in `Compiler/FrontEnd/Parser/Parser.Statements.cs` works like a grammar dispatcher.

It checks `currentToken.TokenType` and routes to the right rule:

- `TK_WHILE` -> `whileStat()`
- `TK_IF` -> `ifStat()`
- `TK_FOR` -> `forStat()`
- `TK_VAIDEN` -> `assignmentStat()`

This is a direct parser implementation of a grammar "choice" (alternatives).

## 8) How grammar connects to semantics and code generation 🔗

Parsing only checks structure. Real compilers also check meaning and emit instructions.

In this repo, each rule often does all three:

1. **syntax** via `match(...)`
2. **semantics** via `SymbolTable.Lookup(...)` / `Insert(...)`
3. **codegen** via `GenerateMepa(...)`

Example: `assignmentStat()` in `Parser.Statements.cs`

- parses `id := expression`
- resolves symbol for `id`
- emits `ARMZ` to store computed value

## 8.1) What is already implemented in this repo ✅

Grammar families currently implemented:

- declarations (`label`, `var`, `procedure`)
  - file: `Compiler/FrontEnd/Parser/Parser.Declarations.cs`
- statements (`if`, `while`, `for`, `repeat`, `case`, `goto`, `read`, `write`)
  - file: `Compiler/FrontEnd/Parser/Parser.Statements.cs`
- expressions (relational + arithmetic + boolean)
  - file: `Compiler/FrontEnd/Parser/Parser.Expressions.cs`

Parser entry and startup grammar:

- file: `Compiler/FrontEnd/Parser/Parser.cs`
- entry method: `Parse()` / `parse()`
- startup expects: `program <id> ( <id> [, <id>]* );` then `block` and `.`

## 9) "How do I implement a new grammar rule?" (template) 🏗️

Use this exact workflow:

1. Write grammar in EBNF first.
2. Decide starting token and where dispatch happens.
3. Add/adjust parser method in the right partial file.
4. Add `match(...)` sequence for terminals.
5. Call other non-terminal methods for nesting.
6. Add semantics and MEPA generation if needed.
7. Validate with one tiny fixture.

### Template diagram

```text
EBNF rule
   |
   v
starting token?
   |
   v
dispatch in statements/declarations/expressions
   |
   v
new parser method + match(...) calls
   |
   +--> SymbolTable checks (if needed)
   |
   +--> GenerateMepa(...) (if needed)
   |
   v
fixture + smoke test
```

## 10) Practice exercise 🎓

Try this with existing code (no new feature required):

1. Write EBNF for `while` statement.
2. Locate implementation in `whileStat()`.
3. Identify where condition is parsed.
4. Identify where false-jump is emitted (`DSVF`).
5. Compare with `Compiler/Tests/Mepa.txt`.

If you can do this, you already understand the core parser architecture.
