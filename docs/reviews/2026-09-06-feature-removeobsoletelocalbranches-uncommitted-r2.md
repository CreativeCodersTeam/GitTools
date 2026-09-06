# .NET Code Review — feature/removeobsoletelocalbranches (uncommitted) — Round 2

**Date:** 2026-09-06
**Mode:** uncommitted (working tree vs. HEAD `db492c2`) — origin: provided
**Round:** 2 (re-check after rework R-1…R-4)
**Previous report:** `docs/reviews/2026-09-06-feature-removeobsoletelocalbranches-uncommitted.md` (round 1, not modified)
**Plan:** `docs/plans/branch-remove-orphaned-tracking.md` (§11 Change Log now has 4 rows)
**Spec source:** `SPEC.md` (repository root) plus plan §8 Clarification Log and §10 Spec Feedback (IDs assigned by the plan); no document in `docs/specs/`
**Implementation notes:** scratchpad `run-note.md` (decisions D-1…D-7, rework items R-1…R-4 with the main agent's verification)
**Detected SDK:** 10.0.400 (`global.json` pins 10.0.100, `rollForward: latestFeature`) — unchanged from round 1
**Target Framework(s):** net10.0
**Version origin:** repo:source/Directory.Build.props (line 5) — same resolution as round 1 (`detect-dotnet-version.sh` exits 5 because the TFM is centralised)
**Checklist:** review-checklist-net10.md + security, performance, architecture, code-quality checklists; refactoring and dotnet-tester perspectives folded in
**Tools run:** build=Y (provided) · format=Y (verify-only, no changes made) · test=Y (provided) · mutation check=Y (reviewer's own run, file restored — see Part A / R-1)
**Report language:** English (default)
**Exclusions:** .gitignore, *.min.js, wwwroot/lib/**
**Review strategy:** full for the rework delta; the rest of the working tree re-checked only where round 1 findings or plan conformance depend on it
**Diff size (whole working tree):** 8 files, 395 changed LOC (348 insertions / 47 deletions; `docs/plans/…` is bookkeeping — ticks, Status lines, 4 Change Log rows). Rework delta vs. round 1: 3 code files (~27 LOC) + 3 Change Log rows.

Files in the rework delta (focus of Part A):

- `source/GitTool/CreativeCoders.GitTool.Cli.Commands/BranchGroup/RemoveOrphaned/OrphanedLocalBranchesProvider.cs` (R-2)
- `tests/CreativeCoders.GitTool.Tests/Cli/Commands/BranchGroup/RemoveOrphaned/OrphanedLocalBranchesProviderTests.cs` (R-1, R-4)
- `tests/CreativeCoders.GitTool.Tests/Cli/Commands/BranchGroup/RemoveOrphaned/RemoveOrphanedLocalBranchesCommandTests.cs` (R-3)

Other files in the working tree (`README.md`, `IOrphanedLocalBranchesProvider.cs`, `RemoveOrphanedLocalBranchesCommand.cs`, `RemoveOrphanedLocalBranchesOptions.cs`) are unchanged since round 1 per the run-note; spot checks (call site `RemoveOrphanedLocalBranchesCommand.cs:62`, the round-1 markers F-4/F-5 in the interface file) confirm this. Files already uncommitted before the run: none (D-2). `docs/reviews/` is untracked and contains only the round-1 report.

## Executive Summary

| Severity | Count |
|---|---|
| Critical | 0 |
| Major | 0 |
| Minor | 0 |
| Suggestion | 0 |
| Nitpick | 2 |

(New findings only. Round-1 items F-4, F-5, F-8 were deferred by the user (D-7) and are listed once under "Deferred from round 1"; F-6 is blocked by constraint 6 and stays as-is.)

**Top risks:**
1. None at Critical/Major. The round-1 Major (F-1, rule 3 of SPEC §3.1 not isolated) is resolved: the reviewer's own mutation run shows exactly one failing test — the new `GetOrphanedLocalBranches_NotTrackingWithTrackedBranchSet_DoesNotReturnBranch` — when `&& x.IsTracking` is removed; the file was restored byte-identically.
2. The helper split (R-2) is behaviour-neutral: `git diff -w HEAD` shows zero changed lines inside the name-matching block (constraint 6), the public signature IF-1 is unchanged, and no new types were introduced (constraint 4).
3. The `CreateSut` overload (R-3) keeps a single SUT construction path; overload resolution is unambiguous because the `out` parameter is mandatory; the 13 pre-existing command tests are untouched (diff hunks: only the new `[Theory]` and `CreateSut`).

**Overall:** All four rework items are resolved as specified. Build clean (0 errors, exactly 1 unique pre-existing xUnit1012 warning = baseline), 157 tests pass (156 + the R-1 test), `dotnet format --verify-no-changes` clean on both changed folders. Two cosmetic nitpicks are the only new observations; nothing needs another rework round.

## Findings

### [Nitpick][Code-Quality] source/GitTool/CreativeCoders.GitTool.Cli.Commands/BranchGroup/RemoveOrphaned/OrphanedLocalBranchesProvider.cs:27
The two sibling helpers wrap their parameter lists differently: `GetOrphanedByName(IGitBranch[] branches, IGitBranch currentBranch)` is a single 110-character line (`:27`), while `GetOrphanedByTracking` breaks after `branches,` at a comparable length (`:49-50`).

No line-length rule is configured (no `.editorconfig` exists in the repository — see checklist walk), `dotnet format` passes, and a 108-character private signature exists elsewhere in the project (`FeatureGroup/Finish/FinishFeatureCommand.cs:74`), so this is purely cosmetic. Aligning the two makes the "moved verbatim" helper and the new helper read as a pair.

```csharp
private IReadOnlyCollection<IGitBranch> GetOrphanedByName(IGitBranch[] branches,
    IGitBranch currentBranch)
{
```

### [Nitpick][Tests] tests/CreativeCoders.GitTool.Tests/Cli/Commands/BranchGroup/RemoveOrphaned/RemoveOrphanedLocalBranchesCommandTests.cs:352
In the new `CreateSut` overload the fake is created in a local `provider`, used in the `A.CallTo` lambda, and only then copied to the `out` parameter (`:356`). The indirection is correct and necessary — C# forbids capturing an `out` parameter inside a lambda (CS1628) — but it reads like a leftover, and a future "simplification" to assign the `out` parameter directly and use it in the lambda would not compile. A one-line comment removes the trap.

```csharp
// An out parameter cannot be captured by the A.CallTo lambda (CS1628), hence the local
var provider = A.Fake<IOrphanedLocalBranchesProvider>();

A.CallTo(() => provider.GetOrphanedLocalBranches(A<bool>._)).Returns(orphanedBranches);

orphanedLocalBranchesProvider = provider;
```

## Deferred from round 1 (unchanged)

Listed once, not re-reported as new findings (D-7, user's own decision):

- **F-4** [Nitpick][Code-Quality] `IOrphanedLocalBranchesProvider.cs:11` — `<summary>` describes only the default mode. Unchanged (`Gets all local branches whose remote counterpart no longer exists.`).
- **F-5** [Nitpick][Code-Quality] `IOrphanedLocalBranchesProvider.cs:15` — space-form self-closing XML tags (`<see … />`); still 6 occurrences, rest of `source/` uses the no-space form. Unchanged.
- **F-8** [Nitpick][Code-Quality] `RemoveOrphanedLocalBranchesCommandTests.cs:174-175` — two consecutive blank lines before the AC-11 `[Theory]`. Still present (only double blank line in either test file).
- **F-6** [Nitpick][Code-Quality] `OrphanedLocalBranchesProvider.cs:29` / `:52` — `branches.Where(x => x.IsRemote)` evaluated in both helpers. Blocked by constraint 6 (the name-matching helper must stay verbatim); stays as-is by design.

## Part A — Rework delta review

### R-2 — `OrphanedLocalBranchesProvider.cs`

- **IF-1 signature unchanged:** `IReadOnlyCollection<IGitBranch> GetOrphanedLocalBranches(bool includeUntracked)` at `IOrphanedLocalBranchesProvider.cs:34` and `OrphanedLocalBranchesProvider.cs:15`. The public method now reads `Head`, materialises `Branches.ToArray()` once, and returns `includeUntracked ? GetOrphanedByName(branches, currentBranch) : GetOrphanedByTracking(branches, currentBranch)` — same evaluation order and same single enumeration of `Branches` as before.
- **Constraint 6 (verbatim block):** `git diff -w HEAD -- OrphanedLocalBranchesProvider.cs` shows only: the signature change, the 4-line ternary return + closing brace, the new `GetOrphanedByName` header, and the new static `GetOrphanedByTracking` method. Every line of the original name-matching block (`remoteFriendlyNames`, `trackedBranchNames` with `StartsWith(..., OrdinalIgnoreCase)` and the range slice, the final `Where`/`ToArray`) appears unchanged; the only non-whitespace difference is that the block now lives inside `GetOrphanedByName`. `git diff --numstat`: 25 insertions / 1 deletion (identical to the pre-mutation state, see R-1).
- **Constraint 4 (no new types):** two private methods, no new types, no new files, DI registration untouched.
- **Constraint 7:** `remoteCanonicalNames` built with `StringComparer.OrdinalIgnoreCase`; compatibility set unchanged.
- **Constraint 8 (style):** file-scoped namespace, primary constructor with `Ensure.NotNull` initialiser untouched, newline before every opening brace, final `return` on its own line in all three methods, `is not null`, `<inheritdoc />` kept on the public method. Private helpers carry no XML docs — consistent with the project (docs required on public members only).
- **Design (refactoring perspective):** `GetOrphanedByName` is an instance method because it needs `_gitRepository.Remotes`; `GetOrphanedByTracking` is `static` because it only consumes its parameters — the asymmetry is the honest one (CA1822-friendly) and documents which mode consults `Remotes`. Parameter type `IGitBranch[]` matches the `ToArray()` result; no widening to `IEnumerable<>` was needed for two private callers. The Change Log row 3 description ("instance; contains the name-matching block verbatim" / "static") matches the code exactly.

### R-1 — new test `GetOrphanedLocalBranches_NotTrackingWithTrackedBranchSet_DoesNotReturnBranch`

- Location `OrphanedLocalBranchesProviderTests.cs:266-286` (`[Fact]` at 266, method at 267), placed between AC-4 and AC-5 in the default-mode group. Arrange: `isTracking: false`, `trackedBranch` = a remote fake **not** present in `Branches`; call `includeUntracked: false`; assert `BeEmpty()`. Under the rules of SPEC §3.1: rule 1 passes (`IsRemote false`), rule 2 passes (not head), rule 4 passes (`TrackedBranch` non-null), rule 5 passes (ref missing) — **only rule 3 rejects**. The test therefore isolates rule 3, which is what F-1 asked for. Comment on the inconsistent fake is accurate (SPEC §3.1: "Conditions 3 and 4 are checked explicitly and independently").
- **Mutation check — performed by this review (not just taken from the run-note):** backed up the provider file to the scratchpad, deleted the line `&& x.IsTracking` (`OrphanedLocalBranchesProvider.cs:58`) with `sed`, ran `dotnet test --nologo --filter "FullyQualifiedName~OrphanedLocalBranchesProviderTests"` → **1 failed, 18 passed, 19 total**; the single `[FAIL]` is `GetOrphanedLocalBranches_NotTrackingWithTrackedBranchSet_DoesNotReturnBranch`. Restored the file from the backup; `cmp` reports byte-identical; `git diff --numstat HEAD` is `25 1` before and after; `git diff -w HEAD --stat` = 25 insertions / 1 deletion. **The provider file is restored exactly.**
- Convention (constraint 9): `Method_Scenario_Expectation` name, `// Arrange` / `// Act` / `// Assert`, `sut` variable, no conditional logic.

### R-3 — `CreateSut` overload, AC-11 test

- `RemoveOrphanedLocalBranchesCommandTests.cs:344-346`: the original overload `CreateSut(IAnsiConsole, IGitRepository, params IGitBranch[])` now delegates with an expression body to `CreateSut(ansiConsole, gitRepository, out _, orphanedBranches)`.
- `:348-359`: the new overload `CreateSut(IAnsiConsole, IGitRepository, out IOrphanedLocalBranchesProvider, params IGitBranch[])` holds the body: fake created, `GetOrphanedLocalBranches(A<bool>._)` configured with `orphanedBranches`, fake assigned to the `out` parameter, command constructed with `A.Fake<ICml>()`. `params` after an `out` parameter is legal; overload resolution is unambiguous (the `out` argument is required for the new overload, absent for the old). Every existing call site with 2 or 3+ arguments keeps resolving to the delegating overload — unchanged behaviour for the 13 pre-existing tests.
- **13 pre-existing tests untouched:** `git diff HEAD` on the file has exactly two hunks — the new `[Theory]` (`:175-191`) and `CreateSut` (`:344-359`). Test count: 13 `[Fact]` + 1 `[Theory]` = 14 attributes; 15 test cases at run time.
- **AC-11 (`:175-191`)** uses `CreateSut(new TestConsole(), gitRepository, out var orphanedLocalBranchesProvider)` and still asserts `A.CallTo(() => orphanedLocalBranchesProvider.GetOrphanedLocalBranches(includeUntracked)).MustHaveHappenedOnceExactly()` with the exact `bool` — a constant or inverted pass-through fails one of the two `[InlineData]` cases. The inline fake from round 1 is gone; one construction path remains.

### R-4 — comment in the AC-8 test

- `OrphanedLocalBranchesProviderTests.cs:363`: `// Remotes are not consulted in tracking mode; the second remote only documents the scenario` directly above the `CreateRepository(..., remoteNames: ["origin", "upstream"])` call. Accurate: `GetOrphanedByTracking` is `static` and cannot touch `_gitRepository.Remotes`; the only `Remotes` consumer is `GetOrphanedByName` (`:35`).

### Checklist walk (delta; no finding)

- **Security / Performance:** no change in trust boundary or complexity; the split adds two method calls per invocation, `Branches` still enumerated once.
- **Architecture / DI:** no registration change; the provider stays behind `IOrphanedLocalBranchesProvider`; production call site `RemoveOrphanedLocalBranchesCommand.cs:62` unchanged (`options.IncludeUntracked`).
- **.NET 10 idioms:** ternary expression-bodied delegation in tests, collection expressions unchanged, `is not null`.
- **Code style:** `dotnet format --verify-no-changes` exit 0 on both changed folders. Note (not a finding of this diff): `CLAUDE.md` and plan constraint 8 reference an `.editorconfig`, but no `.editorconfig` exists anywhere in the repository (`find` over the tree, excluding `bin/obj`); `dotnet format` therefore runs with SDK defaults only.
- **Tests (dotnet-tester perspective):** the provider suite now has one negative per §3.1 rule row (1: AC-6, 2: AC-5, 3: R-1, 4: AC-4, 5: AC-2/AC-8) and one positive (AC-1); no trivially passing test was introduced.

## Plan Conformance

### B1. Traceability matrix (plan §7)

Legend — "Passes": from `dotnet test --nologo` (157 passed, 0 failed) plus the filtered provider run (19/19 green before mutation). "Asserts the criterion": would the test fail if the SPEC text of the ID were violated? Line numbers are the method declaration lines in the current working tree.

| ID | Task | Planned test name | Actual test name | Change Log row? | Exists (file) | Passes | Asserts the ID's criterion | Verdict |
|---|---|---|---|---|---|---|---|---|
| AC-1 | #1 | `GetOrphanedLocalBranches_TrackingBranchWithMissingRemoteRef_ReturnsBranch` | same | n/a | `OrphanedLocalBranchesProviderTests.cs:189` | yes | yes — upstream ref absent from `Branches`, branch returned by identity | verified |
| AC-2 | #1 | `GetOrphanedLocalBranches_TrackingBranchWithExistingRemoteRef_DoesNotReturnBranch` | same | n/a | `:210` | yes | yes — only rule 5 rejects | verified |
| AC-3 | #1 | `GetOrphanedLocalBranches_LocalBranchWithoutUpstream_DoesNotReturnBranch` | same | n/a | `:230` | yes | yes for the spec's case (`IsTracking false`, `TrackedBranch null`); rule 3 is now isolated separately by the R-1 test (see FR-1) | verified |
| AC-4 | #1 | `GetOrphanedLocalBranches_IsTrackingWithoutTrackedBranch_DoesNotReturnBranch` | same | n/a | `:248` | yes | yes — only rule 4 rejects | verified |
| AC-5 | #1 | `GetOrphanedLocalBranches_CurrentHeadBranchWithMissingRemoteRef_DoesNotReturnBranch` | same | n/a | `:289` | yes | yes — only rule 2 rejects | verified |
| AC-6 | #1 | `GetOrphanedLocalBranches_RemoteBranch_DoesNotReturnBranch` | same | yes (row 1: `isTracking: true` + missing `trackedBranch`) | `:308` | yes | yes — only rule 1 rejects | verified |
| AC-7 | #1 | `GetOrphanedLocalBranches_MultipleTrackingBranchesWithMissingRemoteRefs_ReturnsAllOfThem` | same | n/a | `:330` | yes | yes — two qualifying returned, one tracked-with-existing-ref excluded | verified |
| AC-8 | #1 | `GetOrphanedLocalBranches_TrackingBranchOnSecondRemoteWithExistingRef_DoesNotReturnBranch` | same | n/a (R-4 comment only) | `:354` | yes | yes — `refs/remotes/upstream/…` present in `Branches`; inert `remoteNames` now commented (`:363`) | verified |
| AC-9 | #1 | 9 existing tests, unchanged names, `includeUntracked: true` | all 9 present, names unchanged, every call `includeUntracked: true` | n/a | `:15`, `:33`, `:52`, `:69`, `:87`, `:107`, `:129`, `:148`, `:168` | yes (9/9) | yes — expectations unchanged; they exercise `GetOrphanedByName` (moved verbatim) | verified |
| AC-10 | #1 | `GetOrphanedLocalBranches_IncludeUntrackedWithLocalBranchWithoutUpstream_ReturnsBranch` | same | n/a | `:376` | yes | yes — same fixture rejected in AC-3 with `false`, returned with `true` | verified |
| AC-11 | #2 | `ExecuteAsync_IncludeUntrackedOption_PassesValueToProvider` (`[Theory]` true/false) | same, `[InlineData(true)]`, `[InlineData(false)]`; now via `CreateSut(..., out var …)` | yes (row 4: `CreateSut` overload) | `RemoveOrphanedLocalBranchesCommandTests.cs:179` | yes (2/2) | yes — `MustHaveHappenedOnceExactly()` with the exact value | verified |
| AC-12 | #2 | manual — real repository, §3.3 stop rule | — | n/a | — | — | not verifiable by review; user ran it and reported OK (D-6): pruned-upstream branch reported, never-pushed not reported, `-u` reports both. Code path unchanged by the rework (helper split is behaviour-neutral, verified by `git diff -w` and the full suite). | not verifiable by review — user reported OK (D-6) |
| FR-1 | #1 | AC-1 test (+ AC-2…AC-8 as one negative per rule row) | same **plus** `GetOrphanedLocalBranches_NotTrackingWithTrackedBranchSet_DoesNotReturnBranch` (`:267`) for rule 3 | yes (row 2) | as above | yes | **yes — now one negative per rule row:** 1 → AC-6, 2 → AC-5, 3 → R-1 test (mutation-verified by this review), 4 → AC-4, 5 → AC-2 / AC-8 | verified (round-1 Major resolved) |
| FR-2 | #1 | `GetOrphanedLocalBranches_LocalBranchWithoutRemoteCounterpart_ReturnsBranch` (`true`) + AC-10 | same | n/a | `:15`, `:376` | yes | yes — name matching unchanged (`git diff -w`), untracked branch returned | verified |
| FR-3 | #1 | AC-5, AC-6 (default); existing HEAD / remote-branch tests (compat) | same (`:289`, `:308`, `:52`, `:69`) | AC-6 row | as listed | yes | yes — both invariants pinned in both helpers | verified |
| FR-4 | #2 | `ExecuteAsync_IncludeUntrackedOption_PassesValueToProvider` | same | row 4 (helper only) | `:179` | yes | yes — `RemoveOrphanedLocalBranchesCommand.cs:62` passes `options.IncludeUntracked` | verified |
| IF-1 | #1 | compiler: both test classes build only against `GetOrphanedLocalBranches(bool)` | signature `IOrphanedLocalBranchesProvider.cs:34` unchanged by R-2; build green; 19 provider-test calls, 2 command-test fakes (`A<bool>._`, exact value), 1 production call site | yes (row 3 states "IF-1 signature unchanged") | yes | build green | yes | verified |
| IF-2 | #2 | manual — `gt … --help` | — | n/a | — | — | not verifiable by review (no `gt` run); option attribute unchanged since round 1; user confirmed (D-6) | not verifiable by review — user reported OK (D-6) |
| NFR-1 | #2 | manual — warning count ≤ `main` | — | n/a | — | — | evidence from this review: `dotnet build --no-incremental --nologo` → 0 errors, exactly 1 unique warning (`VersionUtilsTests.cs(55,17): xUnit1012`, pre-existing, outside the diff) = baseline. Comparison against branch HEAD `db492c2` (checking out `main` is a git write) — same caveat as round 1 | met (vs. HEAD) |
| NFR-2 (#1) | #1 | `dotnet test` — all pre-existing tests green | — | n/a | — | 157 passed / 0 failed (145 pre-existing + 12 new) | yes | verified |
| NFR-2 (#2) | #2 | `dotnet test` — all pre-existing tests green | — | n/a | — | same run | yes | verified |

Totals: 22 rows. 19 automated rows verified (FR-1 now without a finding); NFR-1 evidence collected (met vs. HEAD); IF-2 and AC-12 user-reported OK (D-6), not reproducible by review.

### B2. Per task

**Task #1 — Status `done`.** Done-when: all Verifies tests pass — yes (AC-1…AC-10, FR-1…FR-3 incl. the added rule-3 test, IF-1, NFR-2 green); full run green — yes; interface docs describe both modes — yes (unchanged since round 1); command behaves as before via the temporary literal — superseded by Task #2 as planned. Files: the rework touched `OrphanedLocalBranchesProvider.cs` and `OrphanedLocalBranchesProviderTests.cs` — both in the task's Files list and in §5. Provides as implemented still match the plan signatures (IF-1; `CreateBranch(string, bool isRemote = false, bool isTracking = false, IGitBranch? trackedBranch = null)` at `:392`). Status matches.

**Task #2 — Status `done`.** Done-when: AC-11 theory passes both values — yes; build and full run green with no new warnings — yes (1 warning = baseline); README and description describe the new default — yes (unchanged since round 1); manual checks confirmed by the user — yes (D-6). Files: the rework touched `RemoveOrphanedLocalBranchesCommandTests.cs` only — in the task's Files list and in §5. Status matches.

**Plan bookkeeping diff:** `docs/plans/branch-remove-orphaned-tracking.md` differs from HEAD only in Todo tick boxes, the two `Status: ready → done` lines and the four Change Log rows (verified with `git diff -U0`). No Verifies block, signature, or matrix wording was altered — consistent with §11's freeze policy ("recorded here, not applied silently").

### B3. Change Log completeness

Compared plan §6/§7 Verifies names and Provides signatures against the working tree:

| Row | Content | Matches code? |
|---|---|---|
| 1 (#1) | AC-6 fake configured with `isTracking: true` + missing `trackedBranch` | yes (`OrphanedLocalBranchesProviderTests.cs:308-328`) — unchanged since round 1 |
| 2 (#1) | New test `GetOrphanedLocalBranches_NotTrackingWithTrackedBranchSet_DoesNotReturnBranch` as Verifies entry for FR-1 rule 3; mutation reference | yes (`:267`); the row's mutation claim was independently reproduced by this review |
| 3 (#1) | `GetOrphanedLocalBranches(bool)` delegates to private `GetOrphanedByName` (instance, verbatim block) and `GetOrphanedByTracking` (static) instead of `if (includeUntracked)`; IF-1 unchanged, constraint 4/6 held | yes (`OrphanedLocalBranchesProvider.cs:15-62`); staticness, instance-ness and verbatim claim all confirmed |
| 4 (#2) | `CreateSut` overload with `out IOrphanedLocalBranchesProvider`; old overload delegates; AC-11 uses it | yes (`RemoveOrphanedLocalBranchesCommandTests.cs:344-359`, `:183`) |

Other differences checked: all 12 planned + added test names exist verbatim; all 9 pre-existing provider test names and all 13 pre-existing command test names unchanged; R-4 is a comment only (no behaviour, no name, no signature) — correctly without a row.

**Change Log gaps: 0.**

### B4. Scope

- **Behaviour without a spec ID:** none. The rework added no behaviour: R-2 is a pure Extract-Method refactoring (both modes' predicates and sets are line-for-line what round 1 reviewed; `git diff -w` proves the compatibility block), R-1/R-3/R-4 are test-only. Constraint 5 (command untouched), 3 (`source/Git/` untouched), 4 (no new files/types/DI), 11 (default mode unchanged) all still hold.
- **Trivially passing tests:** none. The R-1 test has exactly one rejecting rule and is mutation-verified; AC-11 still discriminates on the exact `bool`.

**Scope findings: 0.**

### B5. Rework re-check

| R-n | Finding | Resolved? | Evidence |
|---|---|---|---|
| R-1 | F-1 (Major): SPEC §3.1 rule 3 not isolated by any test | **yes** | `OrphanedLocalBranchesProviderTests.cs:266-286`; reviewer's own mutation (delete `&& x.IsTracking`, filtered run) → 1 failed / 18 passed / 19 total, the failing test is the new one; file restored byte-identically (`cmp`), `git diff --numstat` 25/1 before and after; Change Log row 2 present |
| R-2 | F-2 (Suggestion, user-selected): split the two algorithms behind the unchanged IF-1 signature | **yes** | `OrphanedLocalBranchesProvider.cs:15-62`; public signature unchanged (`IOrphanedLocalBranchesProvider.cs:34`); `git diff -w HEAD` shows no changed line inside the name-matching block; no new types; `<inheritdoc />` kept; newline-before-brace and final-`return` style held; Change Log row 3 present and accurate |
| R-3 | F-3 (Suggestion): AC-11 re-implemented `CreateSut` inline | **yes** | `RemoveOrphanedLocalBranchesCommandTests.cs:344-359` (delegating overload + body overload with `out`), AC-11 at `:183` uses `out var`; only two diff hunks in the file, the 13 pre-existing tests untouched; exact-`bool` assertion kept; Change Log row 4 present |
| R-4 | F-7 (Nitpick): inert `remoteNames` in the AC-8 test unexplained | **yes** | comment at `OrphanedLocalBranchesProviderTests.cs:363`; statement is accurate (`GetOrphanedByTracking` is static, cannot consult `Remotes`) |

## Tool Output Appendix

### dotnet build (`--no-incremental --nologo`)
- Exit 0, 0 errors, 1 unique warning: `tests/CreativeCoders.GitTool.Tests/Base/Versioning/VersionUtilsTests.cs(55,17): xUnit1012` — pre-existing, outside the diff, equals the NFR-1 baseline. Not folded into findings (out of scope).

### dotnet test (`--nologo`)
- Exit 0, **157 passed, 0 failed, 0 skipped**: GitTool.Tests 99, Git.UnitTests 45, GitLab 6, GitHub 4, CredentialManagerCore 3.

### Mutation check (reviewer's own run)
- `dotnet test --nologo --filter "FullyQualifiedName~OrphanedLocalBranchesProviderTests"` with `&& x.IsTracking` deleted from `OrphanedLocalBranchesProvider.cs:58`: exit 1, **1 failed / 18 passed / 19 total**; the only `[FAIL]` is `GetOrphanedLocalBranches_NotTrackingWithTrackedBranchSet_DoesNotReturnBranch`.
- Restore: file copied back from the scratchpad backup; `cmp` byte-identical; `git diff --numstat HEAD` = `25 1` (unchanged); `git diff -w HEAD --stat` = 25 insertions / 1 deletion. **The working tree is exactly as before the mutation.**

### dotnet format (`--verify-no-changes`, `--include` both `RemoveOrphaned/` folders)
- Exit 0, no output (no formatting diffs). Note: no `.editorconfig` exists in the repository, so only SDK default rules apply.

### git (read-only)
- `git status --short`: 8 modified files + untracked `docs/reviews/`; no other changes. `git diff --stat HEAD`: 8 files, 348 insertions / 47 deletions. No git write was performed by this review.
