# Implement a New Feature (Hands-on Workshop) ✨

This guide teaches a safe way to extend the compiler without getting lost.

Use it when adding a new keyword, operator, statement, or literal.

## The golden rule 🛡️

Change one concept at a time.

Good:

- one feature
- one fixture
- small commits

Bad:

- many grammar changes in one commit
- lexer + parser + semantics + refactor all at once

## Step-by-step workflow 🪜

### Step 0: Write the grammar rule first (theory-first)

Before touching code, write a compact EBNF rule for your feature.

Example pattern:

```text
MyStatement -> 'mykeyword' Expression 'then' Statement
```

Why this matters:

- avoids ad-hoc parser edits
- makes terminals/non-terminals explicit
- gives you a direct checklist for `match(...)` calls

Grammar-to-code mapping diagram:

```text
EBNF rule -> parser method -> match(...) terminals -> nested non-terminal calls
```

### Step 1: Write the feature example first

Create a tiny Pascal snippet showing exactly what you want.

Example format:

```pascal
begin
    <new construct here>
end.
```

This snippet becomes your acceptance test.

### Step 2: Decide where the change belongs

Ask:

- is this only lexical (new symbol/keyword)?
- does parser need a new rule branch?
- does semantics need symbol/type checks?
- does code generation need new MEPA output?

### Step 3: Update lexer layer (if needed)

Files:

- `Compiler/Resource/keywords.txt` (new reserved words)
- `Compiler/Tools/TypePascal.cs` (new operators/token mappings)

Goal: scanner emits the token you expect.

### Step 4: Update parser rule dispatch

Files:

- `Compiler/FrontEnd/Parser/Parser.Statements.cs`
- `Compiler/FrontEnd/Parser/Parser.Expressions.cs`
- `Compiler/FrontEnd/Parser/Parser.Declarations.cs`

Goal: parser recognizes the new token sequence and uses `match(...)` correctly.

Theory tip:

- dispatch method (`statements()`/`declarations`/`expressions`) chooses which production to apply using lookahead token type

### Step 5: Add semantic logic (if needed)

Files:

- `Compiler/FrontEnd/Semantics/SymbolTable.cs`
- parser rule where symbol insertion/lookup is required

Goal: declarations/usages are checked with scope awareness.

### Step 6: Add MEPA generation (if behavior changes)

Files:

- parser rule methods + `GenerateMepa(...)`

Goal: compiled output reflects the new construct.

Theory tip:

- code generation should preserve semantic intent of grammar rule, not just token sequence

### Step 7: Verify with fixtures

Minimum checks:

1. build: `xbuild Compiler.sln /p:Configuration=Debug`
2. run default fixture from `Compiler/Tests`
3. run your explicit feature fixture using CLI arg mode
4. confirm token stream reaches `TK_EOF`
5. inspect `Mepa.txt`

## Mini example: adding a new keyword 🧪

Suppose keyword is `foo` (illustrative):

1. add `foo` to `Compiler/Resource/keywords.txt`
2. scanner now can emit `TK_FOO`
3. add parser branch for `TK_FOO` in appropriate parser partial file
4. emit MEPA if needed
5. add fixture exercising the new construct

## Common mistakes (and fixes) 🚨

- parser branch added, but keyword missing in lexer tables -> add token definition first
- keyword added, but parser dispatch not updated -> add case in statement/expression/declaration rule
- changed many features at once -> split into focused commits
- broke startup grammar accidentally -> re-check `program <id> ( <id> [, <id>]* );`

## Recommended commit sequence ✅

1. scanner/token support
2. parser recognition
3. semantic/codegen updates
4. docs + fixture + verification notes

This sequence keeps changes reviewable and easy to roll back.
