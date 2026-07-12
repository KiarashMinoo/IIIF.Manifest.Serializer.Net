---
description: Pick up a GitHub issue end-to-end — resolve/create its branch, self-assign, implement per repo conventions, commit
argument-hint: <issue-url-or-number>
---

Ticket: $ARGUMENTS

Ask the user if anything below is ambiguous (issue not found, unclear scope, conflicting local changes) — don't guess.

1. **Resolve**: extract the issue number from `$ARGUMENTS` (bare number or URL). `gh repo view --json name,nameWithOwner`. `gh issue view <number> --json number,title,body,url,assignees,labels`.
2. **Branch**: `git status` — stash unrelated changes (`git stash push -u`) or ask, never discard silently. `git fetch origin`. `gh issue develop <number> --list`; if found, fetch + `git checkout <branch>`; else `gh issue develop <number> --checkout -n "feature/<number>-<kebab-title-slug>"` off the default branch (matches this repo's `feature/<id>-<slug>` convention).
3. **Assign**: if `assignees` is empty, `gh issue edit <number> --add-assignee @me`.
4. **Implement** per this repo's `CLAUDE.md`: module folder layout (`Domain/`/`Application/`/`Infrastructure/`/`Abstractions/` for new modules, flat for existing unless already touching), the multi-provider `DbContext` template + `generate-migrations.ps1` for any new persistence, `<Module>RouteConvention` for any new route, Controllers/Views/ViewComponents wired consistently, `npm ci && npm run build` in `assets/` if Vue/webpack changed. Confirm `dotnet build` passes.
5. **Commit** only the relevant files (no `git add -A`):
   ```
   git commit -m "$(cat <<'EOF'
   [<RepositoryName>#<IssueID>] <concise summary>

   <what changed, why, notable decisions>

   Co-Authored-By: Claude Sonnet 5 <noreply@anthropic.com>
   EOF
   )"
   ```
   Don't push or open a PR unless asked separately.