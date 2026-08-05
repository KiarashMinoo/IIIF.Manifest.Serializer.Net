# Changelog

All notable changes to this project will be documented in this file.
Format follows [Keep a Changelog](https://keepachangelog.com/en/1.1.0/).

## [3.0.15] — 2026-08-05

### 🐛 Bug Fixes

- IIIF.Manifest.Serializer.Net: split collection tombstones from live items, fix change-tracking bugs, cut hot-path allocations `(368a178)` — Kiarash Minoo

## [3.0.14] — 2026-08-04

### 🚀 Features

- add EF Core-compatible parameterless constructors to all model types `(4fa0431)` — Kiarash Minoo

### 🐛 Bug Fixes

- harden trackable change subscriptions `(3d59974)` — Kiarash Minoo

### ⚙️ CI / Tooling

- restrict publish-nuget.yml to the main branch `(5321198)` — Kiarash Minoo

### 📝 Documentation

- synchronize API and NuGet documentation `(6026fad)` — Kiarash Minoo

## [3.0.13] — 2026-07-26

### ♻️ Refactoring

- reorganize trackable infrastructure and seal leaf types `(57717fb)` — Kiarash Minoo

## [3.0.12] — 2026-07-26

### ♻️ Refactoring

- unify descriptor-based change tracking `(bc80c57)` — Kiarash Minoo

## [3.0.11] — 2026-07-26

### 🚀 Features

- add deep collection change tracking `(9f16d38)` — Kiarash Minoo

### 🐛 Bug Fixes

- update GitHub token secret reference in NuGet publish workflow `(780a882)` — Kiarash Minoo

### 📦 Dependencies

| Package | Old | New |
|---------|-----|-----|
| Microsoft.NET.Test.Sdk | 18.7.0 | 18.8.1 |
| AwesomeAssertions | 9.4.0 | 9.5.0 |
| System.Text.Json | 10.0.9 | 10.0.10 |

- Bump the test-packages group with 2 updates `(924e3b5)` — dependabot[bot]
- Bump the json-packages group with 1 update `(a19908b)` — dependabot[bot]

### ⚙️ CI / Tooling

- Bump actions/setup-dotnet from 5 to 6 in the github-actions group `(c672477)` — dependabot[bot]

## [3.0.10] — 2026-07-13

_No user-facing changes in this release._

## [3.0.9] — 2026-07-13

### ⚙️ CI / Tooling

- Switch NuGet publish workflow to classic API key `(ab1a63e)` — Kiarash Minoo

## [3.0.8] — 2026-07-13

### ⚙️ CI / Tooling

- Use secret for NuGet user in publish workflow `(34c141a)` — Kiarash Minoo

## [3.0.7] — 2026-07-13

### ⚙️ CI / Tooling

- Replace NUGET_USER secret with hardcoded username `(196b853)` — Kiarash Minoo

## [3.0.6] — 2026-07-13

### ⚙️ CI / Tooling

- Fix smoke test package path in NuGet publish `(fd5bae1)` — Kiarash Minoo

### 🏠 Chores

- bump version to 3.0.5 [skip ci] `(3636e99)` — github-actions[bot]

## [3.0.5] — 2026-07-13

### ⚙️ CI / Tooling

- Fix smoke test package path in NuGet publish `(fd5bae1)` — Kiarash Minoo

