# How to Implement a New Language Feature

This guide shows a safe, beginner-friendly workflow for extending this compiler.

## Goal

Add one language feature at a time with minimal risk.

Examples of features:

- new keyword
- new statement form
- new operator
- new literal kind

## Safe implementation steps

1. Pick one small feature and one fixture.
2. Update lexical layer if needed.
3. Update parser rule(s).
4. Update semantic/symbol logic if required.
5. Update MEPA generation if behavior should emit code.
6. Run smoke verification and compare output.

## Detailed checklist

### Step 1: define syntax and scope

- write one example Pascal snippet using the new feature
- decide if feature is only syntax, or syntax + semantics + codegen

### Step 2: scanner/token updates (if needed)

- add keyword to `Compiler/Resource/keywords.txt` when introducing new reserved word
- add operators to `Compiler/Tools/TypePascal.cs` when introducing new symbol/operator

### Step 3: parser updates

- add parser branch in `Compiler/FrontEnd/Parser/Parser.cs` where statement/expression should be recognized
- consume tokens with `match(...)`
- keep style consistent with existing parser methods

### Step 4: semantic updates

- insert or look up symbols via `SymbolTable` when feature introduces identifiers
- validate scope/type assumptions explicitly

### Step 5: code generation updates

- emit MEPA through `GenerateMepa(...)` in the parser branch that implements feature behavior

### Step 6: verification

Minimum smoke checks:

1. build solution
2. run from `Compiler/Tests`
3. confirm token stream reaches `TK_EOF`
4. confirm `Mepa.txt` updated
5. compare output before/after for unchanged fixtures

## Correct usage example (new keyword)

If you add keyword `foo`:

1. add `foo` to `keywords.txt`
2. parser can now see `TK_FOO`
3. add parser branch for `TK_FOO`
4. add fixture that exercises the new branch

## Common mistakes

- Adding parser logic without scanner/token support
- Adding keyword but forgetting to handle it in parser dispatch
- Changing multiple features in one commit (hard to debug and review)
- Breaking startup grammar (`program <id> ( <id> [, <id>]* );`) accidentally

## Recommended commit style

- Commit 1: scanner/token updates
- Commit 2: parser + semantic updates
- Commit 3: docs + fixtures

This keeps refactors reversible and easy to review.
