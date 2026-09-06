# `gt branch remove-orphaned` — tracking-based orphan detection with `--include-untracked`

Spec source: `SPEC.md` (repository root, untracked file, read 2026-09-06; repository HEAD `6bea4fa`).
Plan set: 1 of 1. Approved 2026-09-06 (draft: `docs/draft/plan_branch-remove-orphaned-tracking.md`).

## 1. Summary

`gt branch remove-orphaned` today treats every local branch without a same-named remote branch as
orphaned, which also offers never-pushed scratch branches for deletion. This plan changes the
default to a tracking-based detection: a local branch is orphaned only when it has an upstream
configured whose remote-tracking ref no longer exists. A new switch `-u` / `--include-untracked`
restores the previous name-matching behaviour. The plan was made directly from `SPEC.md`, which
has no requirement IDs; all IDs below were assigned by the plan and are listed under Spec Feedback.
Two tasks: Task #1 implements both detection modes in `OrphanedLocalBranchesProvider` behind the new
signature, Task #2 exposes the CLI option, passes it through, updates the documentation, and runs
the manual real-repository check the spec demands.

## 2. Global Constraints

| # | Constraint | Source |
|---|---|---|
| 1 | `<TargetFramework>net10.0</TargetFramework>`; SDK `10.0.100`, `rollForward: latestFeature` | `source/Directory.Build.props:5`, `tests/Directory.Build.props:5`, `global.json` |
| 2 | Test stack: `xunit` 2.9.3, `FakeItEasy` 9.0.1, `AwesomeAssertions` 9.6.0; git backend `LibGit2Sharp` 0.32.0 | `Directory.Packages.props:29,14,6,17` |
| 3 | "Do not change types in `CreativeCoders.Git.Abstractions` or `CreativeCoders.Git`." (both live in this repository under `source/Git/`) | SPEC §1 Non-goals, §7 Never |
| 4 | "No new files, no new types, no DI registration changes." | SPEC §4 |
| 5 | "Keep `--all`, `--skip-fetch-prune`, prompt, deletion loop, summary and exit codes untouched." | SPEC §7 Always |
| 6 | "Keep the compatibility mode behaviourally identical to today's implementation." — "the existing name-matching code is moved into its own branch of the detection, not rewritten or 'improved'." | SPEC §7 Always, §5 |
| 7 | Name comparison `StringComparer.OrdinalIgnoreCase` in both modes | SPEC §3.1 rule 5; `OrphanedLocalBranchesProvider.cs:30` |
| 8 | Code style: file-scoped namespaces, primary constructors, `Ensure.NotNull(...)` in field initialisers, `is null` / `is not null`, newline before every opening brace, final `return` on its own line, English XML docs on all public members, `.ConfigureAwait(false)` in production code only | `CLAUDE.md` (C# Development), SPEC §5 |
| 9 | Test convention: `Method_Scenario_Expectation`, `[Fact]` (or `[Theory]` + `[InlineData]`), `// Arrange` / `// Act` / `// Assert`, `sut` variable, no `.ConfigureAwait(false)` | `OrphanedLocalBranchesProviderTests.cs:14`, `ReferenceNameTests.cs:11` |
| 10 | "Do not commit; the user commits manually." | SPEC §7 Never, `CLAUDE.md` (Git Commit Instructions) |
| 11 | "Do not make the new mode the non-default, and do not add a third mode." | SPEC §7 Never |

## 3. Codebase Findings

**Build and test.** `dotnet restore`, `dotnet build`, `dotnet test` from the repository root
(`CLAUDE.md`, Development). The Cake pipeline (`./build.sh -t test`) is CI-only. No
`TreatWarningsAsErrors` in any `Directory.Build.props`, so "no new warnings" (NFR-1) needs a
warning-count comparison, not just a green build.

**Entry points and layering.** CLI commands live in
`source/GitTool/CreativeCoders.GitTool.Cli.Commands/<Group>/<Command>/` as `<Name>Command.cs` +
`<Name>Options.cs` plus helpers; a command is a `[CliCommand]`-attributed `ICliCommand<TOptions>`
(`RemoveOrphanedLocalBranchesCommand.cs:14-22`). Options are properties with `[OptionParameter]`;
single-character short names are in use (`'a'` in `RemoveOrphanedLocalBranchesOptions.cs:19`,
`'v'`, `'c'`, `'b'` in `PushBranchOptions.cs:11-17`), so `'u'` is supported (SPEC §7 "Ask first"
item resolved). Git access goes through `IGitRepository` (`CreativeCoders.Git.Abstractions`).

**Closest existing feature — the command itself.**
- `OrphanedLocalBranchesProvider.GetOrphanedLocalBranches()` (`OrphanedLocalBranchesProvider.cs:15-37`):
  collects `Name.Friendly` of all remote branches, strips `<remote>/` per `_gitRepository.Remotes`,
  builds a `HashSet` with `StringComparer.OrdinalIgnoreCase`, returns local non-HEAD branches whose
  friendly name is not in the set. This block becomes the `includeUntracked == true` path unchanged.
- `IOrphanedLocalBranchesProvider` (`IOrphanedLocalBranchesProvider.cs:8-17`): one method, XML
  `<summary>`, `<remarks>` (HEAD never returned), `<returns>`.
- `RemoveOrphanedLocalBranchesCommand.ExecuteAsync` calls the provider once
  (`RemoveOrphanedLocalBranchesCommand.cs:62`); everything else (fetch prune, prompt, delete loop) is
  out of scope (constraint 5).
- `RemoveOrphanedLocalBranchesOptions.SkipFetchPrune` (`RemoveOrphanedLocalBranchesOptions.cs:22-35`)
  is the shape to copy for the new option: `<summary>`, `<value>`, `<remarks>`, then
  `[OptionParameter(...)]`.

**`IGitBranch` members used by the new rule** (`source/Git/CreativeCoders.Git.Abstractions/Branches/IGitBranch.cs`):
`Name` (`ReferenceName` with `.Canonical` / `.Friendly`), `IsRemote`, `IsTracking`,
`TrackedBranch` (`IGitBranch?`). In the implementation `GitBranch.IsTracking => _branch.IsTracking`
and `TrackedBranch = From(branch.TrackedBranch)` is materialised in the constructor
(`source/Git/CreativeCoders.Git/Branches/GitBranch.cs:18-26,42-45`) — two independent members, as
SPEC §3.1 states. `gt branch list` already prints `TrackedBranch?.Name.Canonical` for every branch
(`ListBranchesCommand.cs:37`), so materialising a pruned upstream is exercised in production today;
this is also a code-free way to observe the §3.3 question on a real repository.

**Tests.** Layout mirrors the source tree:
`tests/CreativeCoders.GitTool.Tests/Cli/Commands/BranchGroup/RemoveOrphaned/<Class>Tests.cs`.
- `OrphanedLocalBranchesProviderTests` (9 `[Fact]` tests) with factories
  `CreateBranch(string canonicalName, bool isRemote = false)` (`:185-196`, fakes `Name`, `IsRemote`,
  and `Equals` by canonical name) and
  `CreateRepository(IGitBranch head, IGitBranch[] branches, string[]? remoteBranchNames = null, string[]? remoteNames = null)`
  (`:198-218`). Example name: `GetOrphanedLocalBranches_LocalBranchWithoutRemoteCounterpart_ReturnsBranch`.
- `RemoveOrphanedLocalBranchesCommandTests` (13 tests) with
  `CreateSut(IAnsiConsole, IGitRepository, params IGitBranch[] orphanedBranches)` (`:319-327`), which
  fakes `IOrphanedLocalBranchesProvider` via `A.CallTo(() => provider.GetOrphanedLocalBranches())`,
  and Spectre `TestConsole` for the console.
- `[Theory]` + `[InlineData]` is an established pattern (`tests/CreativeCoders.Git.UnitTests/Common/ReferenceNameTests.cs:11`).

**Mock boundaries.** `IGitRepository`, `IGitBranchCollection`, `IGitBranch`, `IGitRemoteCollection`,
`IGitRemote` are FakeItEasy fakes; `IOrphanedLocalBranchesProvider` is faked in command tests;
`IAnsiConsole` is Spectre's `TestConsole`. **FakeItEasy caveat:** an unconfigured
`IGitBranch.TrackedBranch` returns a *dummy fake*, not `null` (fakeable return type), and an
unconfigured `IsTracking` returns `false`. The extended factory must therefore configure
`TrackedBranch` explicitly (including `Returns(null)` for "no upstream") — otherwise AC-3/AC-4 do
not test what they claim.

**Acceptance level.** There is no harness that drives `gt` against a real git repository
(searched `tests/` for `LibGit2Sharp`/`Repository.Init`: only credential tests). The system boundary
for this module is the provider and command with a faked `IGitRepository`; acceptance tests in this
plan are xUnit tests at that boundary, and the real-repository check is manual (AC-12).

**Not found.** No tests for `[OptionParameter]` attributes anywhere in `tests/` (IF-2 is checked
manually via `--help`); no `docs/specs/` folder; no ADRs; `docs/draft/`, `docs/plans/`,
`docs/reviews/` exist and are empty.

## 4. Scope

**Covered:** FR-1…FR-4, IF-1…IF-2, NFR-1…NFR-2, AC-1…AC-12.

**Plan Set:** n/a — one plan (C-scope).

**Not in this plan:** n/a — the whole inventory is covered.

## 5. File Structure

| Path | Status | Responsibility | Tasks |
|---|---|---|---|
| `source/GitTool/CreativeCoders.GitTool.Cli.Commands/BranchGroup/RemoveOrphaned/IOrphanedLocalBranchesProvider.cs` | changed | contract `GetOrphanedLocalBranches(bool includeUntracked)` with docs for both modes (IF-1) | #1 |
| `source/GitTool/CreativeCoders.GitTool.Cli.Commands/BranchGroup/RemoveOrphaned/OrphanedLocalBranchesProvider.cs` | changed | tracking-based default detection and name-matching compatibility detection (FR-1, FR-2, FR-3) | #1 |
| `source/GitTool/CreativeCoders.GitTool.Cli.Commands/BranchGroup/RemoveOrphaned/RemoveOrphanedLocalBranchesCommand.cs` | changed | call site of the provider; passes `options.IncludeUntracked` (FR-4); command description | #1 (call site compiles), #2 (option pass-through, description) |
| `source/GitTool/CreativeCoders.GitTool.Cli.Commands/BranchGroup/RemoveOrphaned/RemoveOrphanedLocalBranchesOptions.cs` | changed | `IncludeUntracked` option (IF-2) | #2 |
| `README.md` | changed | usage line for `gt branch remove-orphaned` (line 69) | #2 |
| `tests/CreativeCoders.GitTool.Tests/Cli/Commands/BranchGroup/RemoveOrphaned/OrphanedLocalBranchesProviderTests.cs` | changed | provider tests for both modes; extended `CreateBranch` factory | #1 |
| `tests/CreativeCoders.GitTool.Tests/Cli/Commands/BranchGroup/RemoveOrphaned/RemoveOrphanedLocalBranchesCommandTests.cs` | changed | `CreateSut` adapted to the new signature; pass-through test (AC-11) | #1 (`CreateSut`), #2 (AC-11) |

## 6. Tasks

### Task #1 — Provider: tracking-based default mode and compatibility mode

Goal: `OrphanedLocalBranchesProvider.GetOrphanedLocalBranches(bool includeUntracked)` implements
SPEC §3.1 for `false` and keeps today's §3.2 name matching for `true`, with both invariants
(IF-1, FR-1, FR-2, FR-3, AC-1…AC-10, NFR-2). The command keeps its current behaviour by calling
the new signature with `includeUntracked: true` until Task #2 wires the option.
Depends on: none
Parallel with: —
Status: ready

Consumes:
- `IGitBranch.IsRemote`, `IsTracking`, `TrackedBranch`, `Name.Canonical`, `Name.Friendly` (existing, `source/Git/CreativeCoders.Git.Abstractions/Branches/IGitBranch.cs`)
- `IGitRepository.Head`, `Branches`, `Remotes` (existing, used at `OrphanedLocalBranchesProvider.cs:17-25`)
- test factories `CreateBranch` / `CreateRepository` / `CreateRemotes` (existing, `OrphanedLocalBranchesProviderTests.cs:185-243`)

Provides:
- IF-1: `IReadOnlyCollection<IGitBranch> IOrphanedLocalBranchesProvider.GetOrphanedLocalBranches(bool includeUntracked)` — for #2
- `RemoveOrphanedLocalBranchesCommand.ExecuteAsync` calls `_orphanedLocalBranchesProvider.GetOrphanedLocalBranches(includeUntracked: true)` (temporary literal, replaced in #2) — for #2
- `RemoveOrphanedLocalBranchesCommandTests.CreateSut` fakes `GetOrphanedLocalBranches(A<bool>._)` — for #2
- extended factory `CreateBranch(string canonicalName, bool isRemote = false, bool isTracking = false, IGitBranch? trackedBranch = null)` in `OrphanedLocalBranchesProviderTests` — used within #1 only

Files:
`IOrphanedLocalBranchesProvider.cs` (changed), `OrphanedLocalBranchesProvider.cs` (changed),
`RemoveOrphanedLocalBranchesCommand.cs` (changed, call site only),
`OrphanedLocalBranchesProviderTests.cs` (changed), `RemoveOrphanedLocalBranchesCommandTests.cs`
(changed, `CreateSut` only)

Verifies:
```
AC-1  → acceptance  GetOrphanedLocalBranches_TrackingBranchWithMissingRemoteRef_ReturnsBranch
AC-2  → acceptance  GetOrphanedLocalBranches_TrackingBranchWithExistingRemoteRef_DoesNotReturnBranch
AC-3  → acceptance  GetOrphanedLocalBranches_LocalBranchWithoutUpstream_DoesNotReturnBranch
AC-4  → acceptance  GetOrphanedLocalBranches_IsTrackingWithoutTrackedBranch_DoesNotReturnBranch
AC-5  → acceptance  GetOrphanedLocalBranches_CurrentHeadBranchWithMissingRemoteRef_DoesNotReturnBranch
AC-6  → acceptance  GetOrphanedLocalBranches_RemoteBranch_DoesNotReturnBranch
AC-7  → acceptance  GetOrphanedLocalBranches_MultipleTrackingBranchesWithMissingRemoteRefs_ReturnsAllOfThem
AC-8  → acceptance  GetOrphanedLocalBranches_TrackingBranchOnSecondRemoteWithExistingRef_DoesNotReturnBranch
AC-9  → acceptance  the 9 existing tests, unchanged names, now calling GetOrphanedLocalBranches(includeUntracked: true):
                    GetOrphanedLocalBranches_LocalBranchWithoutRemoteCounterpart_ReturnsBranch
                    GetOrphanedLocalBranches_LocalBranchWithRemoteCounterpart_DoesNotReturnBranch
                    GetOrphanedLocalBranches_CurrentHeadBranchWithoutRemoteCounterpart_DoesNotReturnBranch
                    GetOrphanedLocalBranches_RemoteBranchWithoutLocalCounterpart_DoesNotReturnBranch
                    GetOrphanedLocalBranches_AllLocalBranchesHaveRemoteCounterpart_ReturnsEmptyCollection
                    GetOrphanedLocalBranches_MultipleOrphanedBranches_ReturnsAllOfThem
                    GetOrphanedLocalBranches_RemoteCounterpartDiffersInCase_DoesNotReturnBranch
                    GetOrphanedLocalBranches_CounterpartOnSecondRemote_DoesNotReturnBranch
                    GetOrphanedLocalBranches_RemoteBranchNameEndsWithLocalName_ReturnsBranch
AC-10 → acceptance  GetOrphanedLocalBranches_IncludeUntrackedWithLocalBranchWithoutUpstream_ReturnsBranch
FR-1  → unit        GetOrphanedLocalBranches_TrackingBranchWithMissingRemoteRef_ReturnsBranch (rule rows 1–5 positive) plus the AC-2…AC-8 tests (one negative per rule row)
FR-2  → unit        GetOrphanedLocalBranches_LocalBranchWithoutRemoteCounterpart_ReturnsBranch (includeUntracked: true) plus AC-10
FR-3  → unit        GetOrphanedLocalBranches_CurrentHeadBranchWithMissingRemoteRef_DoesNotReturnBranch and GetOrphanedLocalBranches_RemoteBranch_DoesNotReturnBranch (default mode); the existing HEAD and remote-branch tests (compatibility mode)
IF-1  → contract    automated — compiler: OrphanedLocalBranchesProviderTests and RemoveOrphanedLocalBranchesCommandTests build only against GetOrphanedLocalBranches(bool)
NFR-2 → automated   dotnet test — all pre-existing tests green (provider and command tests)
```

Todos:
- [ ] Test `GetOrphanedLocalBranches_TrackingBranchWithMissingRemoteRef_ReturnsBranch` for AC-1 / FR-1 — failing (does not compile until IF-1 exists)
  - prerequisite: extend `CreateBranch` in `OrphanedLocalBranchesProviderTests` with `bool isTracking = false, IGitBranch? trackedBranch = null` and configure `IsTracking` and `TrackedBranch` explicitly on the fake (see FakeItEasy caveat in §3; `Returns(null)` when `trackedBranch` is null). Remote-tracking fakes are created with `CreateBranch("refs/remotes/<remote>/<name>", true)` and passed as `trackedBranch`.
- [ ] Test `GetOrphanedLocalBranches_TrackingBranchWithExistingRemoteRef_DoesNotReturnBranch` for AC-2 — failing
- [ ] Test `GetOrphanedLocalBranches_LocalBranchWithoutUpstream_DoesNotReturnBranch` for AC-3 (`isTracking: false`, `trackedBranch: null`) — failing
- [ ] Test `GetOrphanedLocalBranches_IsTrackingWithoutTrackedBranch_DoesNotReturnBranch` for AC-4 (`isTracking: true`, `trackedBranch: null`) — failing
- [ ] Test `GetOrphanedLocalBranches_CurrentHeadBranchWithMissingRemoteRef_DoesNotReturnBranch` for AC-5 / FR-3 — failing
- [ ] Test `GetOrphanedLocalBranches_RemoteBranch_DoesNotReturnBranch` for AC-6 / FR-3 — failing
- [ ] Test `GetOrphanedLocalBranches_MultipleTrackingBranchesWithMissingRemoteRefs_ReturnsAllOfThem` for AC-7 — failing
- [ ] Test `GetOrphanedLocalBranches_TrackingBranchOnSecondRemoteWithExistingRef_DoesNotReturnBranch` for AC-8 (`remoteNames: ["origin", "upstream"]`, remote-tracking fake `refs/remotes/upstream/...` present in `branches`) — failing
- [ ] Test `GetOrphanedLocalBranches_IncludeUntrackedWithLocalBranchWithoutUpstream_ReturnsBranch` for AC-10 / FR-2 — failing
- [ ] Change the 9 existing tests to call `sut.GetOrphanedLocalBranches(includeUntracked: true)`; expectations and names unchanged (AC-9)
- [ ] Change `IOrphanedLocalBranchesProvider.GetOrphanedLocalBranches()` to `GetOrphanedLocalBranches(bool includeUntracked)`; update `<summary>`, add `<param name="includeUntracked">`, extend `<remarks>` to describe both modes and the HEAD invariant (IF-1)
- [ ] Implement in `OrphanedLocalBranchesProvider`: keep the existing name-matching block as the `includeUntracked` path verbatim (constraint 6); add the tracking-based path: set of `Name.Canonical` of all `IsRemote` branches (`StringComparer.OrdinalIgnoreCase`), return branches with `!IsRemote && !Equals(head) && IsTracking && TrackedBranch is not null && !set.Contains(TrackedBranch.Name.Canonical)` (FR-1, FR-3)
- [ ] Update the call site in `RemoveOrphanedLocalBranchesCommand.ExecuteAsync` to `GetOrphanedLocalBranches(includeUntracked: true)` — temporary, keeps today's behaviour; replaced in Task #2
- [ ] Update `CreateSut` in `RemoveOrphanedLocalBranchesCommandTests` to `A.CallTo(() => orphanedLocalBranchesProvider.GetOrphanedLocalBranches(A<bool>._)).Returns(orphanedBranches)`
- [ ] `dotnet build` and `dotnet test` green; all 13 existing command tests untouched apart from `CreateSut` (NFR-2)

Done when: all Verifies tests pass, the full test run is green, the interface documentation describes
both modes, and `gt branch remove-orphaned` behaves exactly as before (compatibility path via the
temporary literal).

### Task #2 — CLI option `--include-untracked`, pass-through, documentation, real-repository check

Goal: expose the switch as `-u` / `--include-untracked` on the command, pass its value to the
provider so that the tracking-based mode is the default, update the command description and README,
and run the manual verification the spec requires before the change is done
(IF-2, FR-4, AC-11, AC-12, NFR-1, NFR-2).
Depends on: #1
Parallel with: —
Status: ready

Consumes:
- IF-1 `GetOrphanedLocalBranches(bool includeUntracked)` — from #1
- call site `GetOrphanedLocalBranches(includeUntracked: true)` in `RemoveOrphanedLocalBranchesCommand.ExecuteAsync` — from #1 (to be replaced)
- `CreateSut` faking `GetOrphanedLocalBranches(A<bool>._)` — from #1
- `RemoveOrphanedLocalBranchesOptions.SkipFetchPrune` as documentation/attribute template (existing, `RemoveOrphanedLocalBranchesOptions.cs:22-35`)

Provides: — (end of plan)

Files:
`RemoveOrphanedLocalBranchesOptions.cs` (changed), `RemoveOrphanedLocalBranchesCommand.cs`
(changed), `README.md` (changed), `RemoveOrphanedLocalBranchesCommandTests.cs` (changed)

Verifies:
```
AC-11 → acceptance  ExecuteAsync_IncludeUntrackedOption_PassesValueToProvider  ([Theory] with [InlineData(true)] and [InlineData(false)])
FR-4  → acceptance  ExecuteAsync_IncludeUntrackedOption_PassesValueToProvider  (the false case is the default; the option is a plain pass-through)
IF-2  → manual      run `gt branch remove-orphaned --help`; the option is listed as `-u, --include-untracked` with the help text "Also treats local branches without a configured upstream as orphaned" — user
AC-12 → manual      real-repository check (SPEC §6 Verification, §3.3): see Todos — user
NFR-1 → manual      `dotnet build` on the branch; count of `warning` lines is not higher than on `main` — implementer
NFR-2 → automated   dotnet test — all pre-existing tests green
```

Todos:
- [ ] Test `ExecuteAsync_IncludeUntrackedOption_PassesValueToProvider(bool includeUntracked)` for AC-11 / FR-4 in `RemoveOrphanedLocalBranchesCommandTests`: `[Theory]`, `[InlineData(true)]`, `[InlineData(false)]`; arrange `new RemoveOrphanedLocalBranchesOptions { IncludeUntracked = includeUntracked }`, assert `A.CallTo(() => provider.GetOrphanedLocalBranches(includeUntracked)).MustHaveHappenedOnceExactly()` (the provider fake must be reachable from the test; extend `CreateSut` or create the fake inline in the style of `CreateRepository`) — failing (property does not exist)
- [ ] Add `public bool IncludeUntracked { get; set; }` to `RemoveOrphanedLocalBranchesOptions` with `[OptionParameter('u', "include-untracked", HelpText = "Also treats local branches without a configured upstream as orphaned")]` and XML docs (`<summary>`, `<value>`, `<remarks>`) in the shape of `SkipFetchPrune` (IF-2)
- [ ] Replace `GetOrphanedLocalBranches(includeUntracked: true)` in `RemoveOrphanedLocalBranchesCommand.ExecuteAsync` with `GetOrphanedLocalBranches(options.IncludeUntracked)` (FR-4)
- [ ] Update `[CliCommand(... Description = ...)]` on `RemoveOrphanedLocalBranchesCommand` — proposed text (G-1): `"Removes local branches whose remote counterpart was deleted"`
- [ ] Update `README.md` line 69 — proposed text (G-2): `gt branch remove-orphaned     # Delete local branches whose remote branch was deleted (-u: also never-pushed branches)`
- [ ] `dotnet build` and `dotnet test` green; compare the warning count with `main` (NFR-1, NFR-2)
- [ ] Manual check IF-2: `gt branch remove-orphaned --help` lists `-u, --include-untracked` with the help text
- [ ] Manual check AC-12 on a real repository (owner: user): (1) create a branch, push it, delete it on the remote, run `gt branch remove-orphaned` — the branch is reported; (2) a never-pushed local branch is **not** reported; (3) `gt branch remove-orphaned -u` reports both. If (1) fails, run `gt branch list` — if the pruned branch shows no tracked name, `TrackedBranch` is `null` after the prune (SPEC §3.3): **stop and report back**; do not extend `IGitRepository` / `IGitBranch` (SPEC §7 Ask first)
- [ ] XML docs of every changed member re-read against the new behaviour (constraint 8)

Done when: the AC-11 theory passes for both values, build and full test run are green with no new
warnings, README and command description describe the new default, and the manual checks IF-2 and
AC-12 are confirmed by the user (or the §3.3 stop rule has been triggered and reported).

## 7. Traceability Matrix

| ID | Kind | Task | Test type | Test name / manual step | Automated |
|---|---|---|---|---|---|
| AC-1 | behaviour | #1 | acceptance | `GetOrphanedLocalBranches_TrackingBranchWithMissingRemoteRef_ReturnsBranch` | yes |
| AC-2 | behaviour | #1 | acceptance | `GetOrphanedLocalBranches_TrackingBranchWithExistingRemoteRef_DoesNotReturnBranch` | yes |
| AC-3 | behaviour | #1 | acceptance | `GetOrphanedLocalBranches_LocalBranchWithoutUpstream_DoesNotReturnBranch` | yes |
| AC-4 | behaviour | #1 | acceptance | `GetOrphanedLocalBranches_IsTrackingWithoutTrackedBranch_DoesNotReturnBranch` | yes |
| AC-5 | behaviour | #1 | acceptance | `GetOrphanedLocalBranches_CurrentHeadBranchWithMissingRemoteRef_DoesNotReturnBranch` | yes |
| AC-6 | behaviour | #1 | acceptance | `GetOrphanedLocalBranches_RemoteBranch_DoesNotReturnBranch` | yes |
| AC-7 | behaviour | #1 | acceptance | `GetOrphanedLocalBranches_MultipleTrackingBranchesWithMissingRemoteRefs_ReturnsAllOfThem` | yes |
| AC-8 | behaviour | #1 | acceptance | `GetOrphanedLocalBranches_TrackingBranchOnSecondRemoteWithExistingRef_DoesNotReturnBranch` | yes |
| AC-9 | behaviour | #1 | acceptance | the 9 existing `OrphanedLocalBranchesProviderTests` tests (names listed in Task #1), called with `includeUntracked: true` | yes |
| AC-10 | behaviour | #1 | acceptance | `GetOrphanedLocalBranches_IncludeUntrackedWithLocalBranchWithoutUpstream_ReturnsBranch` | yes |
| AC-11 | behaviour | #2 | acceptance | `ExecuteAsync_IncludeUntrackedOption_PassesValueToProvider` (`[Theory]`, true/false) | yes |
| AC-12 | behaviour | #2 | manual | real repository: pushed-then-deleted branch reported; never-pushed branch not reported; `-u` reports both; §3.3 stop rule — user | no |
| FR-1 | rule | #1 | unit | `GetOrphanedLocalBranches_TrackingBranchWithMissingRemoteRef_ReturnsBranch` (+ AC-2…AC-8 tests as negatives per rule row) | yes |
| FR-2 | rule | #1 | unit | `GetOrphanedLocalBranches_LocalBranchWithoutRemoteCounterpart_ReturnsBranch` (`includeUntracked: true`) + `GetOrphanedLocalBranches_IncludeUntrackedWithLocalBranchWithoutUpstream_ReturnsBranch` | yes |
| FR-3 | rule | #1 | unit | `GetOrphanedLocalBranches_CurrentHeadBranchWithMissingRemoteRef_DoesNotReturnBranch`, `GetOrphanedLocalBranches_RemoteBranch_DoesNotReturnBranch` (+ existing HEAD / remote-branch tests for the compatibility mode) | yes |
| FR-4 | behaviour | #2 | acceptance | `ExecuteAsync_IncludeUntrackedOption_PassesValueToProvider` | yes |
| IF-1 | contract | #1 | contract | compiler: both test classes build only against `GetOrphanedLocalBranches(bool)` | yes |
| IF-2 | contract | #2 | manual | `gt branch remove-orphaned --help` lists `-u, --include-untracked` with the spec's help text — user | no |
| NFR-1 | quality | #2 | manual | `dotnet build` warning count on the branch ≤ warning count on `main` — implementer | no |
| NFR-2 | quality | #1 | automated | `dotnet test` — all pre-existing tests green after Task #1 | yes |
| NFR-2 | quality | #2 | automated | `dotnet test` — all pre-existing tests green after Task #2 | yes |

## 8. Clarification Log

| ID | Question | Decision | Rationale | Origin |
|---|---|---|---|---|
| C-0 | `SPEC.md` has no FR/AC/NFR IDs and no Decisions Log — run `create-dev-spec` first or plan directly? | Plan directly from `SPEC.md`; IDs assigned by the plan; no acceptance criteria had to be derived (SPEC §6 lists the cases) | The spec is detailed enough; only bookkeeping was missing | user's own |
| L-1 | Plan language | English | `SPEC.md` is English; `CLAUDE.md` requires English documentation | proposal |
| L-2 | Slug | `branch-remove-orphaned-tracking` | Names the command and the essence of the change | proposal |
| C-scope | One plan or several? | One plan, two tasks | One module, four production files; option and provider only make sense together | proposal |
| C-1 | When is the §3.3 risk (is `TrackedBranch` `null` after the remote ref was pruned?) checked on a real repository? | At the end, as a Definition-of-Done step of Task #2 (AC-12), with the spec's stop rule | Follows SPEC §6 literally; the user accepted the risk that Task #1 may need rework if the check fails | user's own (alternative "before the code, via `gt branch list`" was proposed and declined) |

## 9. Open Questions

n/a — every gap is either answered above or recorded as Spec Feedback. OQ-1 (SPEC §3.3) is
resolved by C-1 into the manual step AC-12 with its stop rule.

## 10. Spec Feedback

IDs assigned by the plan (all `origin: assigned by plan`):

- FR-1 → §3.1 default-mode rules 1–5 (rule)
- FR-2 → §3.2 compatibility mode, unchanged name matching (rule)
- FR-3 → §3 invariants: remote-tracking branches and the current HEAD are never returned (rule)
- FR-4 → §2/§4: the command passes `options.IncludeUntracked` to the provider; option absent = tracking-based mode (behaviour)
- IF-1 → §4: `IReadOnlyCollection<IGitBranch> GetOrphanedLocalBranches(bool includeUntracked)` (contract)
- IF-2 → §2: `[OptionParameter('u', "include-untracked", HelpText = "Also treats local branches without a configured upstream as orphaned")] public bool IncludeUntracked { get; set; }` (contract)
- NFR-1 → §6: `dotnet build` clean, no new warnings (quality)
- NFR-2 → §6/§7: all pre-existing tests green; compatibility mode behaviourally identical (quality)
- AC-1…AC-8 → §6 default-mode cases 1–8 (behaviour)
- AC-9 → §6: existing provider tests keep their expectations with `includeUntracked: true`
- AC-10 → §6: compatibility mode returns a branch without upstream
- AC-11 → §6: `ExecuteAsync` passes `IncludeUntracked` through, both values
- AC-12 → §6 Verification: manual real-repository check including §3.3
- OQ-1 → §3.3 known risk (trigger "before done") — decided by C-1

Clarifications to backport: C-0, L-1, L-2, C-scope, C-1 (see §8).

Gaps reported as notes:

- G-1 [contract incomplete] §4 asks to update the `[CliCommand(Description = …)]` text but gives none; the plan proposes `"Removes local branches whose remote counterpart was deleted"`.
- G-2 [contract incomplete] §4 asks to update README line 69 but gives no wording; the plan proposes `# Delete local branches whose remote branch was deleted (-u: also never-pushed branches)`.
- G-3 [due now] OQ-1 / §3.3 — answered by C-1 (manual DoD step with stop rule in Task #2).
- G-4 [test design] §6 says the fake factory "must be extended to configure `IsTracking` and `TrackedBranch`"; add that FakeItEasy returns a dummy fake for an unconfigured `TrackedBranch`, so "no upstream" fakes must configure `TrackedBranch` to `null` explicitly.
- G-5 [happy-path-only] FR-4 has no failure scenario; none exists (plain pass-through), and combinations with `--all` / `--skip-fetch-prune` remain covered by the existing command tests only. Suggest the spec states this explicitly.
- G-6 [contract] IF-2 (the option attribute) has no automated test because the project has none for `[OptionParameter]` attributes; it is verified manually via `--help`. Suggest §6 lists this manual step.
- G-7 [terminology] §6 says an existing test "additionally asserts" that a branch without upstream is returned in compatibility mode; all existing fakes already have no upstream, so the plan makes this a dedicated test (AC-10) instead of an extra assertion.
- G-8 [resolved] §7 "Ask first — if the CLI parser does not support the short name `'u'`": single-character short names are used elsewhere (`'a'`, `'v'`, `'c'`, `'b'`); no question needed.
- G-9 [context] §3.3 states "if it returns `null`, `IsTracking` is `false`"; consistent with `GitBranch.IsTracking => _branch.IsTracking` and LibGit2Sharp's definition. Reading LibGit2Sharp's `Branch.TrackedBranch` resolution suggests a `Branch` over a void reference (not `null`) when the upstream ref is missing; this remains unverified until AC-12.

## 11. Change Log

Frozen on 2026-09-06 (approval). Test names and interfaces above are fixed; changes during
implementation are recorded here, not applied silently.

| Date | Task | Change | Reason | Origin |
|---|---|---|---|---|
