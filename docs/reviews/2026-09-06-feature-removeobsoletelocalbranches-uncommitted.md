# .NET Code Review — feature/removeobsoletelocalbranches (uncommitted)

**Date:** 2026-09-06
**Mode:** uncommitted (working tree vs. HEAD `db492c2`) — origin: provided
**Round:** 1
**Plan:** `docs/plans/branch-remove-orphaned-tracking.md`
**Spec source:** `SPEC.md` (repository root) plus plan §8 Clarification Log and §10 Spec Feedback (IDs assigned by the plan); no document in `docs/specs/`
**Implementation notes:** scratchpad `run-note.md` (decisions D-1…D-6, one Change Log row). D-6 arrived while this review was in progress: the user ran the manual checks IF-2 and AC-12 and reported both OK; Task #2 is now `done` and both manual Todos are ticked in the plan. The review was re-checked against that state.
**Detected SDK:** 10.0.400 (`global.json` pins 10.0.100, `rollForward: latestFeature`)
**Target Framework(s):** net10.0
**Version origin:** repo:source/Directory.Build.props (line 5; `tests/Directory.Build.props:5` identical). `detect-dotnet-version.sh` exited 5 ("no TargetFramework in CreativeCoders.Git.Abstractions.csproj") because the TFM is centralised in `Directory.Build.props`; non-interactive run, so the TFM was resolved from that file instead of asking.
**Checklist:** review-checklist-net10.md + security, performance, architecture, code-quality checklists; refactoring and dotnet-tester perspectives folded in
**Tools run:** build=Y (provided) · format=Y (verify-only, reviewer's choice, no changes made) · test=Y (provided)
**Report language:** English (default)
**Exclusions:** .gitignore, *.min.js, wwwroot/lib/**
**Review strategy:** full (8 files, 368 LOC — below the large-diff gate)
**Diff size:** 8 files, 368 changed LOC (of which `docs/plans/…` is plan bookkeeping — ticked Todos, Status lines, one Change Log row — not code under review)

Files under review:

- `README.md`
- `source/GitTool/CreativeCoders.GitTool.Cli.Commands/BranchGroup/RemoveOrphaned/IOrphanedLocalBranchesProvider.cs`
- `source/GitTool/CreativeCoders.GitTool.Cli.Commands/BranchGroup/RemoveOrphaned/OrphanedLocalBranchesProvider.cs`
- `source/GitTool/CreativeCoders.GitTool.Cli.Commands/BranchGroup/RemoveOrphaned/RemoveOrphanedLocalBranchesCommand.cs`
- `source/GitTool/CreativeCoders.GitTool.Cli.Commands/BranchGroup/RemoveOrphaned/RemoveOrphanedLocalBranchesOptions.cs`
- `tests/CreativeCoders.GitTool.Tests/Cli/Commands/BranchGroup/RemoveOrphaned/OrphanedLocalBranchesProviderTests.cs`
- `tests/CreativeCoders.GitTool.Tests/Cli/Commands/BranchGroup/RemoveOrphaned/RemoveOrphanedLocalBranchesCommandTests.cs`

All seven files are listed in the plan's File Structure (§5). No file outside §5 was touched.

## Executive Summary

| Severity | Count |
|---|---|
| Critical | 0 |
| Major | 1 |
| Minor | 0 |
| Suggestion | 2 |
| Nitpick | 5 |

**Top risks:**
1. FR-1 rule 3 (`IsTracking == true`) has no test that fails when the check is removed — the plan's claim "one negative per rule row" does not hold for row 3 (`OrphanedLocalBranchesProviderTests.cs`).
2. SPEC §3.3 (does `TrackedBranch` survive a prune?) cannot be verified by review; per D-6 the user ran AC-12 on a real repository and reported the pruned-upstream branch detected, so the §3.3 stop rule was not triggered. The code path is consistent with that outcome (`GitBranch.From` tolerates a `Branch` with `Tip == null`). Not a finding.
3. None. The production change is small, matches SPEC §3.1/§3.2 rule for rule, and the compatibility block is moved verbatim (`git diff -w` shows only the added `if` wrapper and indentation).

**Overall:** Build clean (1 pre-existing warning, equal to baseline), 156 tests pass (up from 145 at run start), `dotnet format --verify-no-changes` clean on the changed folders. All 11 planned test names exist verbatim and pass; the single Change Log row covers the only deviation; the user-owned manual checks are reported OK (D-6). Implementation conforms to the plan; the one Major item is a coverage gap on a spec rule (a test that should exist), not a behaviour defect.

## Findings

### [Major][Tests] tests/CreativeCoders.GitTool.Tests/Cli/Commands/BranchGroup/RemoveOrphaned/OrphanedLocalBranchesProviderTests.cs:228
SPEC §3.1 rule 3 (`branch.IsTracking == true`) is not independently asserted; removing `&& x.IsTracking` from the provider leaves the whole suite green.

SPEC §3.1 states "Conditions 3 and 4 are checked explicitly and independently" and the plan's FR-1 row promises "AC-2…AC-8 tests (one negative per rule row)". The negatives in place are: AC-3 (`isTracking: false, trackedBranch: null`) and AC-4 (`isTracking: true, trackedBranch: null`) — both are rejected by rule 4 (`TrackedBranch is not null`), so neither proves rule 3. AC-4 does prove rule 4 independently; the mirror case for rule 3 — `isTracking: false` with a non-null `TrackedBranch` whose ref is missing — is absent. The spec's own case list (§6 cases 1–8) does not include it either, so this is a gap in the spec's case list that the plan carried over; it is a finding here because the plan's FR-1 row claims a per-row negative that does not exist. In production `LibGit2Sharp.Branch.IsTracking` is derived from `TrackedBranch != null`, so the two are consistent there; the spec explicitly wants the rule robust against inconsistent implementations and fakes, which is exactly what this missing test would pin down.

Proposed fix (test-only, new `[Fact]` in the default-mode group; a Change Log row is needed because it is a new Verifies entry for FR-1):

```csharp
[Fact]
public void GetOrphanedLocalBranches_NotTrackingWithTrackedBranchSet_DoesNotReturnBranch()
{
    // Arrange
    var head = CreateBranch("refs/heads/main");
    var missingRemoteBranch = CreateBranch("refs/remotes/origin/feature/stale", true);

    // IsTracking and TrackedBranch are independent members; rule 3 must reject this on its own
    var inconsistentBranch = CreateBranch("refs/heads/feature/stale", isTracking: false,
        trackedBranch: missingRemoteBranch);

    var repository = CreateRepository(head, [head, inconsistentBranch]);

    var sut = new OrphanedLocalBranchesProvider(repository);

    // Act
    var orphanedBranches = sut.GetOrphanedLocalBranches(includeUntracked: false);

    // Assert
    orphanedBranches.Should().BeEmpty();
}
```

### [Suggestion][Architecture] source/GitTool/CreativeCoders.GitTool.Cli.Commands/BranchGroup/RemoveOrphaned/IOrphanedLocalBranchesProvider.cs:35
`GetOrphanedLocalBranches(bool includeUntracked)` is a boolean flag selecting one of two unrelated algorithms (code-quality checklist: "boolean parameters often hide two methods").

The two branches of the method share nothing but `Head` and `Branches.ToArray()`; each has its own name set and its own predicate. Two methods (or one method plus a private per-mode helper) would read better and make the compatibility block's "moved verbatim" status obvious. **Not actionable in this run:** the signature is fixed by IF-1 (SPEC §4, plan §10) and the plan's Change Log freezes interfaces. Recorded for a future spec revision only; do not apply now.

```csharp
// Only if IF-1 is revised — keeps the public contract, splits the algorithms:
public IReadOnlyCollection<IGitBranch> GetOrphanedLocalBranches(bool includeUntracked)
{
    var currentBranch = _gitRepository.Head;
    var branches = _gitRepository.Branches.ToArray();

    return includeUntracked
        ? GetOrphanedByName(branches, currentBranch)
        : GetOrphanedByTracking(branches, currentBranch);
}
```

### [Suggestion][Tests] tests/CreativeCoders.GitTool.Tests/Cli/Commands/BranchGroup/RemoveOrphaned/RemoveOrphanedLocalBranchesCommandTests.cs:180
`ExecuteAsync_IncludeUntrackedOption_PassesValueToProvider` re-implements `CreateSut` inline (fake provider + `A.CallTo(...).Returns` + constructor call) because `CreateSut` does not expose the provider fake.

The plan allowed "extend `CreateSut` or create the fake inline", so this is not a deviation. From the refactoring perspective it is the only test in the class that constructs the command by hand; if `CreateSut` ever changes (e.g. a new constructor dependency), this test drifts. An `out` parameter or a small overload keeps a single construction path.

```csharp
private static RemoveOrphanedLocalBranchesCommand CreateSut(IAnsiConsole ansiConsole,
    IGitRepository gitRepository, out IOrphanedLocalBranchesProvider orphanedLocalBranchesProvider,
    params IGitBranch[] orphanedBranches)
{
    orphanedLocalBranchesProvider = A.Fake<IOrphanedLocalBranchesProvider>();

    A.CallTo(() => orphanedLocalBranchesProvider.GetOrphanedLocalBranches(A<bool>._)).Returns(orphanedBranches);

    return new RemoveOrphanedLocalBranchesCommand(ansiConsole, A.Fake<ICml>(), gitRepository,
        orphanedLocalBranchesProvider);
}

// existing overload delegates:
private static RemoveOrphanedLocalBranchesCommand CreateSut(IAnsiConsole ansiConsole,
    IGitRepository gitRepository, params IGitBranch[] orphanedBranches)
    => CreateSut(ansiConsole, gitRepository, out _, orphanedBranches);
```

### [Nitpick][Code-Quality] source/GitTool/CreativeCoders.GitTool.Cli.Commands/BranchGroup/RemoveOrphaned/IOrphanedLocalBranchesProvider.cs:11
`<summary>` "Gets all local branches whose remote counterpart no longer exists" describes only the default mode; with `includeUntracked: true` never-pushed branches (no counterpart ever existed) are returned too. The `<remarks>` paragraphs are accurate; the summary slightly over-promises.

```csharp
/// <summary>
/// Gets all local branches which are orphaned: by default those whose configured upstream no longer
/// exists, optionally also those without a remote branch of the same name.
/// </summary>
```

### [Nitpick][Code-Quality] source/GitTool/CreativeCoders.GitTool.Cli.Commands/BranchGroup/RemoveOrphaned/IOrphanedLocalBranchesProvider.cs:15
Self-closing XML tags use the space form `<see langword="true" />` / `<paramref name="…" />` (6 occurrences, all new in this diff); the rest of `source/` uses the no-space form `<see langword="true"/>` (127 occurrences, including the sibling `RemoveOrphanedLocalBranchesOptions.cs` and the new `IncludeUntracked` docs). Constraint 8 / CLAUDE.md: match surrounding style.

```csharp
/// If <paramref name="includeUntracked"/> is <see langword="false"/>, only local branches with a configured
/// upstream are examined: a branch is orphaned when <see cref="IGitBranch.IsTracking"/> is
/// <see langword="true"/>, <see cref="IGitBranch.TrackedBranch"/> is not <see langword="null"/> and no
```

### [Nitpick][Code-Quality] source/GitTool/CreativeCoders.GitTool.Cli.Commands/BranchGroup/RemoveOrphaned/OrphanedLocalBranchesProvider.cs:23
`branches.Where(x => x.IsRemote)` is evaluated in both modes with different projections (`Name.Friendly` vs `Name.Canonical`). A hoisted `remoteBranches` array would remove the duplicate filter. **Blocked by constraint 6** (compatibility block moved verbatim) — do not apply; recorded for completeness.

```csharp
// Not now — would touch the verbatim block (constraint 6):
var remoteBranches = branches.Where(x => x.IsRemote).ToArray();
```

### [Nitpick][Tests] tests/CreativeCoders.GitTool.Tests/Cli/Commands/BranchGroup/RemoveOrphaned/OrphanedLocalBranchesProviderTests.cs:338
`GetOrphanedLocalBranches_TrackingBranchOnSecondRemoteWithExistingRef_DoesNotReturnBranch` passes `remoteNames: ["origin", "upstream"]`, but the tracking path never consults `IGitRepository.Remotes` — the argument has no effect on the outcome. The test still asserts AC-8 correctly (the `refs/remotes/upstream/…` ref is present in `Branches`, so rule 5 rejects the branch); the arrangement merely suggests a dependency that does not exist. This follows the plan Todo literally, so it is not a deviation. Optional: drop the argument, or keep it with a one-line comment that it documents the scenario only.

```csharp
// Remotes are not consulted in tracking mode; the second remote only documents the scenario
var repository = CreateRepository(head, [head, trackedBranch, remoteBranch],
    remoteNames: ["origin", "upstream"]);
```

### [Nitpick][Code-Quality] tests/CreativeCoders.GitTool.Tests/Cli/Commands/BranchGroup/RemoveOrphaned/RemoveOrphanedLocalBranchesCommandTests.cs:174
Two consecutive blank lines between `ExecuteAsync_SkipFetchPruneSet_DoesNotFetchFromRemote` and the new `[Theory]` (lines 174–175); every other member in the file is separated by exactly one. `dotnet format` has no rule for this, so it is not caught by tooling. The run-note mentions a duplicate blank line was removed in the Options file; this one in the test file was missed.

```csharp
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task ExecuteAsync_IncludeUntrackedOption_PassesValueToProvider(bool includeUntracked)
```

## Checklist walk (no finding)

- **Security:** no input crosses a trust boundary; `IncludeUntracked` is a bool bound by the CLI parser. Nothing to flag.
- **Performance:** `Branches.ToArray()` once, two `HashSet` builds at most one per call; no hot path. `x.TrackedBranch` is read twice per branch in the predicate (`is not null` then `.Name.Canonical`) — the property is an auto-property on `GitBranch`, so it is a field read, not a re-materialisation.
- **Architecture / DI:** no registration change; the provider stays behind `IOrphanedLocalBranchesProvider`; `ExecuteAsync` touched only at the one call site (constraint 5 held — prompt, delete loop, summary and exit codes unchanged).
- **.NET 10 idioms:** primary constructor with `Ensure.NotNull` field initialiser unchanged; collection expressions used in tests (`Returns([])`, `[head, …]`); `is not null` used.
- **Code style (constraint 8):** file-scoped namespace, newline before braces, final `return` on its own line, `ConfigureAwait(false)` untouched in production, none in tests. Verified by `dotnet format --verify-no-changes` on both folders (exit 0).
- **Test convention (constraint 9):** all 11 new tests follow `Method_Scenario_Expectation`, `// Arrange` / `// Act` / `// Assert`, `sut` variable, `[Theory]` + `[InlineData]` for AC-11. No conditional logic in test bodies.
- **XML docs:** every changed public member has updated docs (`IOrphanedLocalBranchesProvider.GetOrphanedLocalBranches`, `RemoveOrphanedLocalBranchesOptions.IncludeUntracked`); `OrphanedLocalBranchesProvider` uses `<inheritdoc />`. `<see cref="All"/>` / `<see cref="SkipFetchPrune"/>` resolve in the same class.
- **Compatibility block (constraint 6):** `git diff -w` on `OrphanedLocalBranchesProvider.cs` shows the original lines 22–37 untouched apart from the wrapping `if (includeUntracked) { … }`; the three LINQ statements, the `StartsWith(..., OrdinalIgnoreCase)`, the range slice and the predicate are byte-identical modulo indentation.
- **Case-insensitivity (constraint 7):** `remoteCanonicalNames` is built with `StringComparer.OrdinalIgnoreCase`; the compatibility set is unchanged.
- **README / description:** G-1 and G-2 texts applied verbatim.

## Plan Conformance

### B1. Traceability matrix (plan §7)

Legend — "Passes": from `dotnet test --nologo` (156 passed, 0 failed) and the filtered run listing every `RemoveOrphaned*` test by name. "Asserts the criterion": would the test fail if the SPEC text of the ID were violated?

| ID | Task | Planned test name | Actual test name | Change Log row? | Exists (file) | Passes | Asserts the ID's criterion | Verdict |
|---|---|---|---|---|---|---|---|---|
| AC-1 | #1 | `GetOrphanedLocalBranches_TrackingBranchWithMissingRemoteRef_ReturnsBranch` | same | n/a (no deviation) | `OrphanedLocalBranchesProviderTests.cs:187` | yes | yes — upstream ref absent from `Branches`, branch returned by identity (`BeSameAs`) | verified |
| AC-2 | #1 | `GetOrphanedLocalBranches_TrackingBranchWithExistingRemoteRef_DoesNotReturnBranch` | same | n/a | `:208` | yes | yes — same remote-tracking fake is both `TrackedBranch` and present in `Branches`; rule 5 is the only rejecting rule | verified |
| AC-3 | #1 | `GetOrphanedLocalBranches_LocalBranchWithoutUpstream_DoesNotReturnBranch` | same | n/a | `:228` | yes | yes for the spec's case (`IsTracking false`, `TrackedBranch null`); note it is rejected by rule 4 alone — rule 3 not isolated (see Major finding) | verified (with finding) |
| AC-4 | #1 | `GetOrphanedLocalBranches_IsTrackingWithoutTrackedBranch_DoesNotReturnBranch` | same | n/a | `:246` | yes | yes — `IsTracking true`, `TrackedBranch null`; only rule 4 rejects | verified |
| AC-5 | #1 | `GetOrphanedLocalBranches_CurrentHeadBranchWithMissingRemoteRef_DoesNotReturnBranch` | same | n/a | `:265` | yes | yes — head qualifies under rules 1,3,4,5; only rule 2 rejects | verified |
| AC-6 | #1 | `GetOrphanedLocalBranches_RemoteBranch_DoesNotReturnBranch` | same | yes (2026-09-06, #1: fake configured `isTracking: true` + missing `trackedBranch`) | `:284` | yes | yes — with the Change Log setup rules 3–5 pass, only rule 1 rejects | verified |
| AC-7 | #1 | `GetOrphanedLocalBranches_MultipleTrackingBranchesWithMissingRemoteRefs_ReturnsAllOfThem` | same | n/a | `:306` | yes | yes — two qualifying branches returned, a tracked branch with existing ref (`develop`) excluded, `BeEquivalentTo` | verified |
| AC-8 | #1 | `GetOrphanedLocalBranches_TrackingBranchOnSecondRemoteWithExistingRef_DoesNotReturnBranch` | same | n/a | `:330` | yes | yes — `refs/remotes/upstream/…` present in `Branches`; `remoteNames` argument inert (Nitpick) | verified |
| AC-9 | #1 | 9 existing tests, unchanged names, `includeUntracked: true` | all 9 present with unchanged names (`:13`–`:185`), every call is `includeUntracked: true` | n/a | `:13`, `:31`, `:50`, `:67`, `:85`, `:105`, `:127`, `:146`, `:166` | yes (9/9) | yes — expectations unchanged; they exercise the moved compatibility block | verified |
| AC-10 | #1 | `GetOrphanedLocalBranches_IncludeUntrackedWithLocalBranchWithoutUpstream_ReturnsBranch` | same | n/a | `:351` | yes | yes — `IsTracking false`, `TrackedBranch null`, returned with `includeUntracked: true`; the same fixture is rejected in AC-3 with `false` — the mode difference is pinned | verified |
| AC-11 | #2 | `ExecuteAsync_IncludeUntrackedOption_PassesValueToProvider` (`[Theory]` true/false) | same, `[InlineData(true)]`, `[InlineData(false)]` | n/a (inline fake is a plan-offered option) | `RemoveOrphanedLocalBranchesCommandTests.cs:176` | yes (2/2) | yes — `MustHaveHappenedOnceExactly()` with the exact value; a constant or inverted pass-through fails one case | verified |
| AC-12 | #2 | manual — real repository, §3.3 stop rule | — | n/a | — | — | not verifiable by review. Code shows: `GitBranch.TrackedBranch = From(branch.TrackedBranch)`; `From` returns `null` only for a `null` LibGit2Sharp `Branch`; `Tip`/`Commits` factories tolerate `null` (`GitCommit.From`, `GitCommitLog.From`), so a `Branch` over a void reference materialises without throwing and its `CanonicalName` feeds rule 5. Per run-note D-6 the user ran the check (pruned-upstream branch reported; never-pushed branch not reported; `-u` reports both) and the §3.3 stop rule was not triggered; the Todo is ticked — consistent with the code. | not verifiable by review — user reported OK (D-6) |
| FR-1 | #1 | AC-1 test (+ AC-2…AC-8 as one negative per rule row) | same | n/a | as above | yes | partial — negatives exist for rules 1 (AC-6), 2 (AC-5), 4 (AC-4), 5 (AC-2, AC-8); **no negative isolates rule 3** | verified with Major finding |
| FR-2 | #1 | `GetOrphanedLocalBranches_LocalBranchWithoutRemoteCounterpart_ReturnsBranch` (`true`) + AC-10 | same | n/a | `:13`, `:351` | yes | yes — name matching unchanged, untracked branch returned | verified |
| FR-3 | #1 | AC-5 test, AC-6 test (default); existing HEAD / remote-branch tests (compat) | same (`:265`, `:284`, `:50`, `:67`) | AC-6 row | as listed | yes | yes — both invariants pinned in both modes | verified |
| FR-4 | #2 | `ExecuteAsync_IncludeUntrackedOption_PassesValueToProvider` | same | n/a | `:176` | yes | yes — the `false` case is the default; `RemoveOrphanedLocalBranchesCommand.cs:61` passes `options.IncludeUntracked` | verified |
| IF-1 | #1 | compiler: both test classes build only against `GetOrphanedLocalBranches(bool)` | signature at `IOrphanedLocalBranchesProvider.cs:35`; `find_referencing_symbols` lists 18 provider-test calls, 2 command-test fakes, 1 production call site — no parameterless call remains | n/a | yes | build green | yes | verified |
| IF-2 | #2 | manual — `gt … --help` | — | n/a | — | — | not verifiable by review (no `gt` run). Code shows `RemoveOrphanedLocalBranchesOptions.cs:50`: `[OptionParameter('u', "include-untracked", HelpText = "Also treats local branches without a configured upstream as orphaned")]` on `public bool IncludeUntracked { get; set; }` — short name, long name and help text match SPEC §2 verbatim. Per run-note D-6 the user confirmed `--help` lists `-u, --include-untracked` with that text; the Todo is ticked — consistent with the code. | not verifiable by review — user reported OK (D-6) |
| NFR-1 | #2 | manual — warning count ≤ `main` | — | n/a | — | — | evidence collected by review: `dotnet build --no-incremental --nologo` → 0 errors, exactly 1 unique warning (`VersionUtilsTests.cs(55,17): xUnit1012`, pre-existing, file outside the diff) = baseline. Comparison is against branch HEAD `db492c2`, not `main` (checking out `main` is a git write, forbidden) — same caveat as the run-note. | met (vs. HEAD) |
| NFR-2 (#1) | #1 | `dotnet test` — all pre-existing tests green | — | n/a | — | 156 passed / 0 failed (145 pre-existing + 11 new) | yes | verified |
| NFR-2 (#2) | #2 | `dotnet test` — all pre-existing tests green | — | n/a | — | same run | yes | verified |

Totals: 22 rows. 19 automated rows verified (one, FR-1, with a Major coverage finding); NFR-1 evidence collected (met vs. HEAD); IF-2 and AC-12 not verifiable by review — user-owned manual checks, reported OK by the user (D-6), code consistent with the reported outcome.

### B2. Per task

**Task #1 — Status `done`.** Done-when: "all Verifies tests pass" — yes (AC-1…AC-10, FR-1…FR-3, IF-1, NFR-2 all green); "full test run green" — yes; "interface documentation describes both modes" — yes (`IOrphanedLocalBranchesProvider.cs:11-33`, three `<para>` blocks: default mode, compatibility mode, invariants + case-insensitivity); "command behaves exactly as before via the temporary literal" — superseded by Task #2 (the literal was replaced by `options.IncludeUntracked`, as planned; the run-note records the intermediate state). Files: all five within §5 and within the task's own Files list; `RemoveOrphanedLocalBranchesCommandTests.cs` touched only at `CreateSut` for this task, as prescribed. Provides as implemented match the plan signatures exactly (IF-1; `CreateSut` with `A<bool>._`; `CreateBranch(string, bool isRemote = false, bool isTracking = false, IGitBranch? trackedBranch = null)`). Status matches.

**Task #2 — Status `done`** (was `partial` when this review started; updated by the main agent after D-6). Done-when: AC-11 theory passes both values — yes; build and full test run green with no new warnings — yes (1 warning = baseline); README and command description describe the new default — yes (`README.md:69` = G-2 verbatim; `RemoveOrphanedLocalBranchesCommand.cs:17` = G-1 verbatim); manual checks IF-2 and AC-12 confirmed by the user — yes per run-note D-6 (origin: user's own; not reproducible by review). Files: all four within §5. Documentation Todos (option XML docs, description, README, "XML docs re-read") produced documentation. All Todos ticked; `done` matches the run-note. Status consistent — not a finding.

**Plan bookkeeping diff:** `docs/plans/branch-remove-orphaned-tracking.md` changed only in Todo tick boxes (including the two manual ones after D-6), the two Status lines and one Change Log row — as the review brief expected. No wording of Verifies blocks, signatures or the matrix was altered.

### B3. Change Log completeness

Compared plan §6/§7 Verifies names and Provides signatures with the working tree:

- All 11 planned test names exist verbatim (no renames). All 9 pre-existing provider test names unchanged.
- IF-1 signature, call site, `CreateSut` fake, `CreateBranch` extension — identical to the plan's Provides.
- Deviation 1: AC-6 test setup extended (`isTracking: true`, missing `trackedBranch`) — **row present** (2026-09-06, #1, implement-dev-plan). The reason given is correct: without it rule 3 rejects before rule 1 is reached and the test would not prove the invariant.
- AC-11 built the provider fake inline instead of extending `CreateSut` — the plan Todo offers both options; not a deviation, no row needed.
- Duplicate blank line removed in `RemoveOrphanedLocalBranchesOptions.cs` by the main agent — formatting, no row needed.
- The Major finding above (missing rule-3 negative) is a gap in the plan's coverage claim, not a silent code deviation; if the proposed test is added it needs a Change Log row (new Verifies entry for FR-1).

**Change Log gaps: 0.**

### B4. Scope

- **Behaviour without a spec ID:** none. The production diff consists of: the `if (includeUntracked)` wrapper around the unchanged compatibility block (FR-2), the tracking-based filter (FR-1, FR-3, constraint 7), the option property with attribute and docs (IF-2), the pass-through (FR-4), the description text (G-1) and the README line (G-2). No message, prompt, delete-loop, exit-code, `--all` or `--skip-fetch-prune` code changed (constraint 5). No file in `source/Git/` changed (constraint 3). No new files, types or DI registrations (constraint 4). Default remains the tracking mode; no third mode (constraint 11).
- **Spec IDs with no real code path / trivially passing tests:** none. Each acceptance test was read against its rule and has a single rejecting (or accepting) condition, except the rule-3 isolation gap already reported. AC-8's inert `remoteNames` argument does not make the test trivial — the asserted condition (existing `refs/remotes/upstream/…` ref) is the one the rule evaluates.

**Scope findings: 0.**

## Tool Output Appendix

### dotnet build (`--no-incremental --nologo`)
- 0 errors, 1 warning: `tests/CreativeCoders.GitTool.Tests/Base/Versioning/VersionUtilsTests.cs(55,17): xUnit1012` — pre-existing, file not in the diff, equals the NFR-1 baseline. Not folded into findings (out of scope).

### dotnet test (`--nologo`)
- 156 passed, 0 failed, 0 skipped: GitTool.Tests 98, Git.UnitTests 45, GitLab 6, GitHub 4, CredentialManagerCore 3. Filtered run (`FullyQualifiedName~RemoveOrphaned`): 33 test cases passed (18 provider, 15 command incl. the two `[InlineData]` cases).

### dotnet format (`--verify-no-changes`, changed folders only)
- Exit 0, no formatting diffs reported. (The double blank line at `RemoveOrphanedLocalBranchesCommandTests.cs:174` is not covered by any configured rule.)

### detect-dotnet-version.sh
- Exit 5 (no `<TargetFramework>` in `CreativeCoders.Git.Abstractions.csproj`); TFM resolved manually from `source/Directory.Build.props:5` = `net10.0`.
