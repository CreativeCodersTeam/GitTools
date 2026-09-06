# SPEC — `branch remove-orphaned`: tracking-based orphan detection

Status: draft, awaiting approval
Scope: single module, no capability map needed (one independently testable capability)

## 1. Objective

`gt branch remove-orphaned` currently treats **every** local branch as orphaned when no remote
branch of the same name exists. That is too aggressive: purely local branches which were never
pushed (scratch branches, work in progress, `wip/*`) have no remote counterpart by design and are
offered for deletion.

The command shall by default only report local branches which **were** connected to a remote and
whose remote counterpart is gone — i.e. a branch that has an upstream configured
(`branch.<name>.remote` / `branch.<name>.merge`) while the corresponding remote-tracking ref no
longer exists (typically after the remote branch was deleted and `fetch --prune` removed the ref).

A switch restores the previous, name-matching behaviour.

Target user: developers using `gt` to clean up local branches after merged pull requests.

### Non-goals

- No change to the fetch-prune step, the `--all` option, the interactive selection prompt, the
  deletion loop, the summary output, or the exit codes.
- No change to `IGitBranch`, `IGitRepository`, or any other type in `CreativeCoders.Git.Abstractions`.
- No new detection of "merged but still tracked" branches.

## 2. Commands / CLI surface

New option on `RemoveOrphanedLocalBranchesOptions`:

| Short | Long                   | Type   | Default | Effect |
|-------|------------------------|--------|---------|--------|
| `-u`  | `--include-untracked`  | `bool` | `false` | Restores the previous detection: a local branch is orphaned when no remote branch with the same friendly name exists on any remote — regardless of whether an upstream is configured. |

`HelpText`: `Also treats local branches without a configured upstream as orphaned`

Resulting usage:

```bash
gt branch remove-orphaned          # default: only branches whose configured upstream is gone
gt branch remove-orphaned -u       # previous behaviour: name matching against all remotes
gt branch remove-orphaned -a -u    # combines with the existing --all option
```

The option is orthogonal to `--all` and `--skip-fetch-prune`; all combinations stay valid.

## 3. Detection rules

Both modes keep the existing invariants: remote-tracking branches are never returned, and the
current HEAD branch is never returned even when it qualifies.

### 3.1 Default mode (`--include-untracked` not set)

A local branch is orphaned when **all** of the following hold:

1. `branch.IsRemote == false`
2. `!branch.Equals(gitRepository.Head)`
3. `branch.IsTracking == true`
4. `branch.TrackedBranch is not null`
5. No branch in `gitRepository.Branches` with `IsRemote == true` has
   `Name.Canonical == branch.TrackedBranch.Name.Canonical`
   (comparison `StringComparer.OrdinalIgnoreCase`, consistent with the existing name matching)

Conditions 3 and 4 are checked explicitly and independently — `IsTracking` and `TrackedBranch` are
separate members of `IGitBranch`, and the implementations behind them (including test fakes) are not
required to be consistent with each other.

Consequence: a branch that never had an upstream is **not** orphaned in this mode.

### 3.2 Compatibility mode (`--include-untracked` set)

Unchanged from today: strip the `<remote>/` prefix from every remote branch's friendly name, and
report every local non-HEAD branch whose friendly name is not in that set (case-insensitive).

### 3.3 Known risk — must be verified against a real repository

`GitBranch.TrackedBranch` is materialised from `LibGit2Sharp.Branch.TrackedBranch`, which resolves
the upstream **name** from git config. It is not established in this repository whether LibGit2Sharp
returns a `Branch` instance (with `Tip == null`) or `null` once the remote-tracking ref has been
pruned. If it returns `null`, `IsTracking` is `false` and rule 3.1 would never match, making the
default mode silently report nothing.

This must be verified before the change is considered done — see §6, Verification.
If the verification shows `TrackedBranch` becomes `null` after the prune, stop and report back: the
rule then needs a different data source (git config access, which `IGitRepository` does not expose
today) and that is a materially larger change than this spec covers.

## 4. Project structure — files touched

| File | Change |
|------|--------|
| `source/GitTool/CreativeCoders.GitTool.Cli.Commands/BranchGroup/RemoveOrphaned/RemoveOrphanedLocalBranchesOptions.cs` | Add `IncludeUntracked` property with `[OptionParameter('u', "include-untracked", …)]` and XML docs matching the style of the existing properties. |
| `.../RemoveOrphaned/IOrphanedLocalBranchesProvider.cs` | Signature becomes `GetOrphanedLocalBranches(bool includeUntracked)`. Update `<summary>`, add `<param>`, update `<remarks>` to describe both modes. |
| `.../RemoveOrphaned/OrphanedLocalBranchesProvider.cs` | Implement both filters. Keep the existing name-matching code path for the compatibility mode; add the tracking-based path. |
| `.../RemoveOrphaned/RemoveOrphanedLocalBranchesCommand.cs` | Pass `options.IncludeUntracked` to the provider. Update the `[CliCommand(Description = …)]` text to describe the new default. |
| `README.md` (line 69) | Update the `gt branch remove-orphaned` line to describe the new default and mention `-u`. |
| `tests/CreativeCoders.GitTool.Tests/Cli/Commands/BranchGroup/RemoveOrphaned/OrphanedLocalBranchesProviderTests.cs` | Adapt existing tests to the new signature; add tests per §6. |
| `tests/.../RemoveOrphaned/RemoveOrphanedLocalBranchesCommandTests.cs` | Add a test that the option value reaches the provider. |

No new files, no new types, no DI registration changes.

## 5. Code style

Follows `CLAUDE.md` and the surrounding files — nothing new is introduced:

- File-scoped namespaces, primary constructors, `Ensure.NotNull(...)` in field initialisers.
- English XML documentation on all public members; documentation updated wherever behaviour changes.
- `is null` / `is not null`, never `== null`.
- Newline before every opening brace; final `return` on its own line.
- Production code uses `.ConfigureAwait(false)`; tests do not.
- Surgical changes only: the existing name-matching code is moved into its own branch of the
  detection, not rewritten or "improved".

## 6. Testing strategy

Framework: xUnit + FakeItEasy + AwesomeAssertions, matching the existing test files.

The fake `IGitBranch` factory in `OrphanedLocalBranchesProviderTests` must be extended to configure
`IsTracking` and `TrackedBranch`, so the two modes can be exercised without a real repository.

### `OrphanedLocalBranchesProviderTests` — new cases (default mode, `includeUntracked: false`)

1. Local branch with upstream configured, remote-tracking ref missing → returned.
2. Local branch with upstream configured, remote-tracking ref present → not returned.
3. Local branch without any upstream (`IsTracking == false`, `TrackedBranch is null`) → not returned.
4. `IsTracking == true` but `TrackedBranch is null` → not returned (rules 3 and 4 are independent).
5. Current HEAD branch with a missing upstream → not returned.
6. Remote branch → never returned.
7. Multiple qualifying branches → all returned.
8. Upstream on a second remote (`upstream/…`) whose ref still exists → not returned.

### `OrphanedLocalBranchesProviderTests` — existing cases

All current tests describe the compatibility mode; they are updated to call
`GetOrphanedLocalBranches(includeUntracked: true)` and keep their current expectations. At least one
of them additionally asserts that a branch **without** upstream is still returned in this mode — the
behavioural difference between the two modes.

### `RemoveOrphanedLocalBranchesCommandTests` — new case

`ExecuteAsync` passes `options.IncludeUntracked` through to
`IOrphanedLocalBranchesProvider.GetOrphanedLocalBranches`, verified for both values with FakeItEasy.

### Verification (Definition of Done)

- `dotnet build` clean, no new warnings.
- `dotnet test` green, including all pre-existing tests.
- Manual check against a real repository, covering §3.3: create a branch, push it, delete it on the
  remote, run `gt branch remove-orphaned` and confirm the branch is reported; confirm a never-pushed
  local branch is **not** reported; confirm `-u` reports both.

## 7. Boundaries

**Always**

- Keep `--all`, `--skip-fetch-prune`, prompt, deletion loop, summary and exit codes untouched.
- Keep the compatibility mode behaviourally identical to today's implementation.
- Update the XML docs of every member whose behaviour changes.
- Add tests for every new code path.

**Ask first**

- If §3.3 verification shows `TrackedBranch`/`IsTracking` cannot detect a pruned upstream — do not
  work around it by extending `IGitRepository` or `IGitBranch` without asking.
- If the CLI parser turns out not to support the short name `'u'` on this command.

**Never**

- Do not change types in `CreativeCoders.Git.Abstractions` or `CreativeCoders.Git`.
- Do not make the new mode the non-default, and do not add a third mode.
- Do not refactor adjacent code in the command (message building, delete loop) as a side effect.
- Do not commit; the user commits manually.
