> **Date:** 2026-09-06
> **Plan:** docs/plans/branch-remove-orphaned-tracking.md @ db492c2 (plan set: 1 of 1)
> **Spec:** none — plan-only run from `SPEC.md` (repository root, committed in db492c2) (D-1)
> **Branch:** feature/removeobsoletelocalbranches
> **Mode:** sub-agents (sequential)
> **Status:** complete

# `gt branch remove-orphaned` — tracking-based orphan detection with `--include-untracked`

## 1. Summary

The default detection of `gt branch remove-orphaned` now reports only local branches whose
configured upstream (`IsTracking` + `TrackedBranch`) no longer exists as a remote-tracking ref; the
new switch `-u` / `--include-untracked` restores the previous name-matching behaviour. Both tasks of
the plan are done (2 of 2), all 24 plan Todos are ticked, and the user confirmed the two manual
checks (`--help` output and the real-repository run including the SPEC §3.3 stop rule, which was
not triggered). Two review rounds were run: round 1 produced one Major finding (a missing test that
isolates SPEC §3.1 rule 3), which was fixed together with three lower-severity items; round 2
found only two nitpicks and the user accepted the changes. Next step for the user: review the diff
and commit. **Nothing has been committed.**

## 2. Preconditions

- Working tree at start: dirty (`SPEC.md`, `docs/` untracked) — decision D-2: committed first by the user (db492c2); clean when implementation started. No pre-existing uncommitted files were in review scope.
- Plan-set dependencies: n/a — single plan.
- Partial implementation found: no — all Todos unticked, both tasks `Status: ready`, provider still parameterless at start.

## 3. Consistency Check

| ID | Finding | Severity | IDs / tasks | Decision |
|---|---|---|---|---|
| K-1 | `SPEC.md` is now committed (db492c2) with content identical to what the plan read; its header still says "Status: draft, awaiting approval" | note | all | recorded — covered by plan C-0 |
| K-2 | Plan header cites `docs/draft/plan_branch-remove-orphaned-tracking.md`; `docs/draft/` is empty | note | — | recorded |
| K-3 | Commit db492c2's message describes an implementation, but the commit contains only `SPEC.md` and the plan; the code was unimplemented, matching `Status: ready` | note | #1, #2 | recorded |
| K-4 | All Global Constraints verified against the code (net10.0, package versions, no `TreatWarningsAsErrors`, `IGitBranch` members, README line 69); factory line references in plan §3 are off by two lines | note | — | recorded (cosmetic) |
| K-5 | `CLAUDE.md` and plan constraint 8 reference an `.editorconfig`; none exists in the repository, so `dotnet format` runs with SDK defaults only (found by review round 2) | note | constraint 8 | recorded |

No blocking findings. Known Spec Feedback carried over (not re-checked): all IDs assigned by the
plan (FR-1…4, IF-1…2, NFR-1…2, AC-1…12); G-1…G-9.

## 4. Skills

**Runtime discovery:** 70+ skills in the runtime list, 10 candidates after matching against the
plan (C# / net10.0, xUnit, FakeItEasy, AwesomeAssertions, CLI command, XML docs, README).

| Skill | Category | Proposed use | Outcome |
|---|---|---|---|
| dotnet-fundamentals | knowledge (baseline) | implementation — all tasks | kept (D-3) |
| dotnet-tester | tester | implementation — all tasks | kept (D-3) |
| dotnet-xmldocs | documentation | implementation — #1, #2 | kept (D-3) |
| dotnet-reviewer | reviewer | review (Phase 6) | kept (D-3) |
| refactoring | refactoring | review (Phase 6) | kept (D-3) |
| dotnet-inspect | tooling | excluded — no dependency change | stayed excluded |
| implementer | workflow | excluded — nested workflow | stayed excluded |
| agent-skills:build | workflow | excluded — nested workflow, commits | stayed excluded |
| agent-skills:test-driven-development | knowledge (generic) | excluded — redundant with plan Todos | stayed excluded |
| dotnet-aspnet, dotnet-ef-core, dotnet-sdk-builder, dotnet-nuget-manager | knowledge / tooling | excluded — no matching files | stayed excluded |

Dropped by user: none. Added by user: none.

**Mapping (as confirmed, D-4) and as actually loaded:**

| Task | Mapped skills | Loaded (from implementer report) |
|---|---|---|
| All | dotnet-fundamentals, dotnet-tester | both, before first edit (Task #1, Task #2, rework) |
| #1 | + dotnet-xmldocs | loaded before first edit |
| #2 | + dotnet-xmldocs | loaded before first edit |
| Rework R-1…R-4 | dotnet-fundamentals, dotnet-tester, dotnet-xmldocs | all three, before first edit |
| Review rounds 1 and 2 | dotnet-reviewer, refactoring, dotnet-tester | all three, before reading code |

## 5. Implementation

| Task | Title | Status | Planned tests | Green | Files | Deviations | Attempts |
|---|---|---|---|---|---|---|---|
| #1 | Provider: tracking-based default mode and compatibility mode | done | 9 new + 9 existing (AC-1…AC-10), IF-1 (compiler), NFR-2 | 18/18 | IOrphanedLocalBranchesProvider.cs, OrphanedLocalBranchesProvider.cs, RemoveOrphanedLocalBranchesCommand.cs (call site), OrphanedLocalBranchesProviderTests.cs, RemoveOrphanedLocalBranchesCommandTests.cs (CreateSut) | 1 technical (Change Log row 1) | 1 |
| #2 | CLI option `--include-untracked`, pass-through, documentation, real-repository check | done | AC-11/FR-4 theory (2 cases), NFR-1, NFR-2; IF-2 and AC-12 manual | 2/2 automated; IF-2, AC-12 confirmed by user (D-6) | RemoveOrphanedLocalBranchesOptions.cs, RemoveOrphanedLocalBranchesCommand.cs, README.md, RemoveOrphanedLocalBranchesCommandTests.cs | 0 | 1 |
| Rework 1 | R-1…R-4 from review round 1 | done | + 1 test (rule 3) | 1/1 | OrphanedLocalBranchesProvider.cs, OrphanedLocalBranchesProviderTests.cs, RemoveOrphanedLocalBranchesCommandTests.cs | 3 (Change Log rows 2–4) | 1 |

Per-task verification by the main agent: build and full test run repeated; planned test names
grepped; `git status` compared with each task's `Files`; new tests read against the SPEC text;
`git diff -w` on the provider confirms the name-matching block is moved verbatim (constraint 6);
the rule-3 mutation (deleting `&& x.IsTracking`) was re-run by the main agent and by review
round 2 — exactly one test fails, the file was restored byte-identically both times.

Test counts: 145 at start → 154 after Task #1 → 156 after Task #2 → 157 after rework.
Build warnings: 1 pre-existing (`xUnit1012`, `VersionUtilsTests.cs:55`) at every stage; no new
warnings (NFR-1, compared against branch HEAD db492c2 because checking out `main` is a git write).

Final full run: `dotnet build --no-incremental --nologo` → 0 errors, 1 warning (baseline);
`dotnet test --nologo` → 157 passed, 0 failed, 0 skipped — on 2026-09-06 16:27.

The main agent removed one duplicate blank line the Task #2 sub-agent left in
`RemoveOrphanedLocalBranchesOptions.cs` (formatting only).

## 6. Deviations from the Plan

The Change Log rows added to `docs/plans/branch-remove-orphaned-tracking.md` during this run:

| Date | Task | Change | Reason | Origin |
|---|---|---|---|---|
| 2026-09-06 | #1 | Test `GetOrphanedLocalBranches_RemoteBranch_DoesNotReturnBranch` (AC-6): the remote-branch fake is additionally configured with `isTracking: true` and a `trackedBranch` whose ref is missing | Otherwise the test passes for the wrong reason (rule 3 fails before rule 1 is checked); with the extra setup `IsRemote` is the deciding condition. Test-only, same expectation | implement-dev-plan |
| 2026-09-06 | #1 | New test `GetOrphanedLocalBranches_NotTrackingWithTrackedBranchSet_DoesNotReturnBranch` (isTracking: false, TrackedBranch set with missing ref, default mode → not returned) added as a Verifies entry for FR-1 rule 3 | Review round 1 F-1 (major): no existing test isolated §3.1 rule 3; mutation check confirmed exactly this test fails without `&& x.IsTracking`. Per D-7 | implement-dev-plan |
| 2026-09-06 | #1 | `OrphanedLocalBranchesProvider.GetOrphanedLocalBranches(bool)` delegates to private helpers `GetOrphanedByName(branches, currentBranch)` (instance; contains the name-matching block verbatim) and `GetOrphanedByTracking(branches, currentBranch)` (static) instead of an `if (includeUntracked)` block | Review round 1 F-2 (suggestion), selected by the user (D-7); IF-1 signature unchanged, block moved byte-identical modulo indentation (constraint 6), no new types (constraint 4) | implement-dev-plan |
| 2026-09-06 | #2 | `RemoveOrphanedLocalBranchesCommandTests.CreateSut` gained an overload with `out IOrphanedLocalBranchesProvider`; the existing overload delegates to it; the AC-11 test uses the overload instead of an inline fake | Review round 1 F-3 (suggestion), selected by the user (D-7); single SUT construction path | implement-dev-plan |

No functional deviations. Row 3 is a user-selected shape change (D-7); the public contract IF-1
and every spec ID's behaviour are unchanged.

## 7. Review

| Round | Report | Findings (c/M/m/S/N) | Selected | Deferred | Rework result |
|---|---|---|---|---|---|
| 1 | docs/reviews/2026-09-06-feature-removeobsoletelocalbranches-uncommitted.md | 0/1/0/2/5 | F-1 (major), F-2, F-3, F-7 (D-7) | F-4, F-5, F-8 (D-7); F-6 blocked by constraint 6 | R-1…R-4 done, 157 tests green |
| 2 | docs/reviews/2026-09-06-feature-removeobsoletelocalbranches-uncommitted-r2.md | 0/0/0/0/2 | — | F-9, F-10 (D-8: accepted as is) | — |

Reviewer skills used: dotnet-reviewer, refactoring, dotnet-tester. Plan conformance in the final
round: 22/22 matrix rows verified (19 automated green; NFR-1 met vs. HEAD; IF-2 and AC-12
user-reported OK), 0 Change Log gaps, 0 scope findings; R-1…R-4 all confirmed resolved.

## 8. Decisions Log

| ID | Phase | Question | Decision | Origin |
|---|---|---|---|---|
| D-1 | 0 | Plan made from `SPEC.md`, no spec in `docs/specs/` — continue plan-only? | Continue plan-only; Clarification Log and Spec Feedback are the spec surrogate | proposal |
| D-2 | 0 | Dirty working tree (`SPEC.md`, `docs/`) | Commit first (user committed db492c2); re-check clean | proposal |
| D-3 | 2 | Skill set | Keep dotnet-fundamentals, dotnet-tester, dotnet-xmldocs; review set dotnet-reviewer, refactoring, dotnet-tester; no excluded skill overridden; no additions | proposal |
| D-4 | 3 | Task-to-skill mapping | Adopt as proposed | proposal |
| D-5 | 4 | Direct or sub-agents? | Sub-agents, sequential (direct was recommended; no `Parallel with` in the plan) | user's own |
| D-6 | 5 | Manual checks IF-2 and AC-12 (Task #2) | User ran both, reported OK; §3.3 stop rule not triggered; Task #2 set to done | user's own |
| D-7 | 7 | Review round 1 — which findings to fix? | Fix F-1, F-2, F-3, F-7; defer F-4, F-5, F-8. F-2 applied as private-helper split behind the unchanged IF-1 signature | user's own |
| D-8 | 7 | Review round 2 — fix F-9 / F-10 or accept? | Accept the changes as they are; F-9, F-10 deferred | proposal |

## 9. Open Items

- Deferred review findings (all nitpicks, in the round-1 and round-2 reports):
  - `F-4` — interface `<summary>` describes only the default mode — decision `D-7`
  - `F-5` — six new `<see langword="true" />` tags use the space form; the repo uses `<see langword="true"/>` — decision `D-7`
  - `F-8` — double blank line at `RemoveOrphanedLocalBranchesCommandTests.cs:174` — decision `D-7`
  - `F-9` — `GetOrphanedByName` parameter list not wrapped like its sibling — decision `D-8`
  - `F-10` — local-then-`out` assignment in `CreateSut` lacks an explanatory comment — decision `D-8`
  - `F-6` — duplicated `IsRemote` filter in both helpers — blocked by constraint 6 (verbatim block), not actionable
- Blocked or partial tasks: none
- Consistency notes not acted on: `K-1`…`K-5` (K-5: no `.editorconfig` in the repository although `CLAUDE.md` refers to one)
- Skills the user asked for that were not loadable: none
- Spec Feedback the plan carries that is still open: G-1, G-2 (wording now in code — spec could absorb it), G-4…G-7, G-9 (context notes for a spec revision); the rule-3 test case added by R-1 is a further candidate for SPEC §6

## 10. Hand-over

The changes are **uncommitted**. Suggested next steps:
1. `git diff` — review the changes against this report and the plan.
2. `git add` / `git commit` what you accept; the Change Log in the plan explains every
   difference from the frozen plan. `docs/reviews/` and this report are new files.
3. Open review findings and notes are listed in §9.
