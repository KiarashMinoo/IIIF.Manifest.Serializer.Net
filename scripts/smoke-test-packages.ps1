#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Package smoke test (issue #13, SDK Phase 7): packs the core + 3 extension NuGet packages,
    installs them into a throwaway clean console app (never a ProjectReference), and confirms a
    manifest can actually be built/serialized/deserialized using each extension - proving the
    packages work as *published artifacts*, not just as in-repo project references.

.PARAMETER Configuration
    Build/pack configuration. Defaults to Release (matches .github/workflows/publish-nuget.yml).

.PARAMETER SkipPack
    Skip the `dotnet pack` step and reuse whatever is already in -PackagesDir - useful when
    re-running just the smoke-app step after a pack already succeeded, or (as in CI) when a
    prior step already packed via the shared pack-solution.ps1 script.

.PARAMETER PackagesDir
    Directory (relative to the repo root) to pack into / read .nupkg files from. Defaults to
    artifacts/packages for standalone local use. When run as publish-nuget.yml's
    post-pack-command with -SkipPack, this must be set to artifacts/pkg instead, since that's
    where the shared pack-solution.ps1 script (used by reusable-ci.yml's Pack step) actually
    writes packed output -- the two scripts don't share a default directory name.

.EXAMPLE
    ./scripts/smoke-test-packages.ps1

.EXAMPLE
    ./scripts/smoke-test-packages.ps1 -Configuration Debug -SkipPack

.EXAMPLE
    ./scripts/smoke-test-packages.ps1 -SkipPack -PackagesDir artifacts/pkg
#>
param(
    [string]$Configuration = "Release",
    [switch]$SkipPack,
    [string]$VersionSuffix = "",
    [string]$PackagesDir = "artifacts/packages"
)

$ErrorActionPreference = "Stop"

$repoRoot = Resolve-Path "$PSScriptRoot/.."
$packagesDir = Join-Path $repoRoot $PackagesDir
$smokeDir = Join-Path $repoRoot "artifacts/smoke-test"

if (-not $SkipPack) {
    Write-Host "`n== Packing packages ($Configuration) ==" -ForegroundColor Cyan
    New-Item -ItemType Directory -Force -Path $packagesDir | Out-Null

    $projects = @(
        "src/IIIF.Manifest.Serializer.Net/IIIF.Manifest.Serializer.Net.csproj",
        "extensions/IIIF.Manifest.Serializer.Net.NavPlace/IIIF.Manifest.Serializer.Net.NavPlace.csproj",
        "extensions/IIIF.Manifest.Serializer.Net.Georeference/IIIF.Manifest.Serializer.Net.Georeference.csproj",
        "extensions/IIIF.Manifest.Serializer.Net.TextGranularity/IIIF.Manifest.Serializer.Net.TextGranularity.csproj"
    )

    $packArgs = @("--configuration", $Configuration, "--output", $packagesDir)
    if ($VersionSuffix) { $packArgs += @("-p:VersionSuffix=$VersionSuffix") }

    foreach ($project in $projects) {
        $fullPath = Join-Path $repoRoot $project
        dotnet pack $fullPath @packArgs
        if ($LASTEXITCODE -ne 0) { throw "dotnet pack failed for $project" }
    }
}

# Derive the actual packed version from the .nupkg filenames rather than parsing
# Directory.Build.props directly, so this works unmodified whether or not -VersionSuffix
# (matching publish-nuget.yml's prerelease channel) was used to pack them. Matched via regex
# (not Get-ChildItem -Filter's wildcard), since a naive "<id>.*.nupkg" glob for the core package
# id would also match every extension package's .nupkg (they all start with the same prefix).
function Get-PackedVersion([string]$packageId) {
    $pattern = "^$([regex]::Escape($packageId))\.(\d[\w.\-]*)$"
    $nupkg = Get-ChildItem -Path $packagesDir -Filter "*.nupkg" |
        Where-Object { $_.Name -notmatch '\.symbols\.nupkg$' -and $_.BaseName -match $pattern }
    if (-not $nupkg) { throw "No .nupkg found for '$packageId' in $packagesDir - run without -SkipPack first." }
    if ($nupkg -is [array]) { throw "Multiple .nupkg files matched '$packageId': $($nupkg.Name -join ', ')" }
    $null = $nupkg.BaseName -match $pattern
    return $Matches[1]
}

$coreVersion = Get-PackedVersion "IIIF.Manifest.Serializer.Net"
$navPlaceVersion = Get-PackedVersion "IIIF.Manifest.Serializer.Net.NavPlace"
$georeferenceVersion = Get-PackedVersion "IIIF.Manifest.Serializer.Net.Georeference"
$textGranularityVersion = Get-PackedVersion "IIIF.Manifest.Serializer.Net.TextGranularity"

Write-Host "Core package version: $coreVersion" -ForegroundColor Cyan
Write-Host "NavPlace package version: $navPlaceVersion" -ForegroundColor Cyan
Write-Host "Georeference package version: $georeferenceVersion" -ForegroundColor Cyan
Write-Host "TextGranularity package version: $textGranularityVersion" -ForegroundColor Cyan

Write-Host "`n== Creating clean smoke-test console app ==" -ForegroundColor Cyan
Remove-Item -Recurse -Force $smokeDir -ErrorAction SilentlyContinue
New-Item -ItemType Directory -Force -Path $smokeDir | Out-Null

Push-Location $smokeDir
try {
    dotnet new console -n IiifPackageSmoke --force | Out-Null
    if ($LASTEXITCODE -ne 0) { throw "dotnet new console failed" }

    Push-Location "IiifPackageSmoke"
    try {
        Write-Host "`n== Installing packages from local artifacts/packages (never a ProjectReference) ==" -ForegroundColor Cyan
        dotnet add package IIIF.Manifest.Serializer.Net --version $coreVersion --source $packagesDir
        if ($LASTEXITCODE -ne 0) { throw "Failed to install core package" }

        dotnet add package IIIF.Manifest.Serializer.Net.NavPlace --version $navPlaceVersion --source $packagesDir
        if ($LASTEXITCODE -ne 0) { throw "Failed to install NavPlace package" }

        dotnet add package IIIF.Manifest.Serializer.Net.Georeference --version $georeferenceVersion --source $packagesDir
        if ($LASTEXITCODE -ne 0) { throw "Failed to install Georeference package" }

        dotnet add package IIIF.Manifest.Serializer.Net.TextGranularity --version $textGranularityVersion --source $packagesDir
        if ($LASTEXITCODE -ne 0) { throw "Failed to install TextGranularity package" }

        $programSource = @'
using IIIF.Manifests.Serializer;
using IIIF.Manifests.Serializer.Extensions;
using IIIF.Manifests.Serializer.Nodes;
using IIIF.Manifests.Serializer.Nodes.Contents.Annotation;
using IIIF.Manifests.Serializer.Nodes.Contents.Image.Resource;
using IIIF.Manifests.Serializer.Properties;

// Core: build, serialize, and round-trip a Manifest with a painting Annotation.
var canvas = new Canvas("https://example.org/iiif/canvas/1", new Label("Page 1"), 1000, 800);
canvas.AddAnnotation(new Annotation(
    "https://example.org/iiif/annotation/1",
    new ImageResource("https://example.org/iiif/page1.jpg", "image/jpeg").SetHeight(1000).SetWidth(800),
    canvas.Id));

var manifest = new Manifest("https://example.org/iiif/manifest", new Label("Smoke Test")).AddItem(canvas);

// navPlace extension: attach geolocation and confirm it survives IiifSerializer's V3 path.
manifest.SetNavPlace(new NavPlace("https://example.org/iiif/navplace")
    .AddFeature(new Feature("https://example.org/iiif/feature/1")
        .SetGeometry(new Geometry(GeometryType.Point).AddCoordinate(new CoordinateItem(-73.9857, 40.7484)))));

var json = IiifSerializer.Serialize(manifest);
var roundTripped = IiifSerializer.DeserializeManifest(json);

if (roundTripped.Id != manifest.Id) throw new InvalidOperationException("Smoke test FAILED: manifest id did not round-trip.");
if (roundTripped.Items.Count != 1) throw new InvalidOperationException("Smoke test FAILED: canvas item did not round-trip.");
if (roundTripped.NavPlace is null) throw new InvalidOperationException("Smoke test FAILED: navPlace extension did not round-trip.");
if (!json.Contains("\"type\": \"Manifest\"")) throw new InvalidOperationException("Smoke test FAILED: unexpected V3 output shape.");

Console.WriteLine("PACKAGE SMOKE TEST PASSED");
Console.WriteLine(json);
'@
        Set-Content -Path "Program.cs" -Value $programSource -Encoding utf8

        Write-Host "`n== Running smoke-test app ==" -ForegroundColor Cyan
        $outputLines = dotnet run --configuration Release 2>&1
        $exitCode = $LASTEXITCODE
        $output = ($outputLines | Out-String)
        Write-Host $output

        if ($exitCode -ne 0) { throw "Smoke-test app exited with code $exitCode" }
        if ($output -notmatch "PACKAGE SMOKE TEST PASSED") { throw "Smoke-test app did not print the expected success marker" }
    }
    finally {
        Pop-Location
    }
}
finally {
    Pop-Location
    Remove-Item -Recurse -Force $smokeDir -ErrorAction SilentlyContinue
}

Write-Host "`nPackage smoke test PASSED for core ($coreVersion), NavPlace ($navPlaceVersion), Georeference ($georeferenceVersion), TextGranularity ($textGranularityVersion)." -ForegroundColor Green
