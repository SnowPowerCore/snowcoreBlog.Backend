# Packs the snowcoreBlog backend shared packages into the local NuGet feed consumed
# via ..\..\local-nuget-feed (registered in the parent repo's nuget.config):
#
# Kernel / infrastructure layer:
#   snowcoreBlog.Backend.Shared.Core   - the shared kernel
#   snowcoreBlog.PublicApi             - service DTO/contract mirror (dependency of Infrastructure)
#   snowcoreBlog.Backend.Infrastructure - cross-service infrastructure (depends on the two above)
# Service-defaults / module layer:
#   snowcoreBlog.ServiceDefaults       - Aspire service defaults (OTel, service discovery)
#   snowcoreBlog.ApplicationLaunch(.Implementations) - application launch module pair
# Cross-service contract layer (consumed by other microservices as packages; a
# microservice only PROJECT-references its own <Service>.Core):
#   snowcoreBlog.Backend.IAM.Core
#   snowcoreBlog.Backend.Email.Core
#   snowcoreBlog.Backend.AspireYarpGateway.Core
#
# Usage:
#   pwsh -File pack-shared-packages.ps1            # packs with version 0.1.0
#   pwsh -File pack-shared-packages.ps1 0.2.0      # packs with an explicit version
#
# Bump the version TOGETHER with the matching PackageVersion entries in the parent repo's
# Directory.Packages.props whenever Infrastructure or any Core changes.
# Run this after changing any of the packed projects, then restore/build consumers.

param(
    [string]$Version = '0.1.0',
    # Packages must be built Release; Debug builds are never published to the feed.
    [ValidateSet('Release', 'Debug')]
    [string]$Configuration = 'Release'
)

$ErrorActionPreference = 'Stop'
$repo = $PSScriptRoot
# Feed lives one level ABOVE the Aspire root (e:\Repos\snowcoreBlog\local-nuget-feed),
# matching the ..\local-nuget-feed entry in the parent repo's nuget.config.
$feed = Join-Path (Split-Path (Split-Path $repo -Parent) -Parent) 'local-nuget-feed'
New-Item -ItemType Directory -Force -Path $feed | Out-Null

# Ordered: dependencies before dependents (Infrastructure last).
$projects = @(
    'Shared\Shared.Core\snowcoreBlog.Backend.Shared.Core.csproj',
    'snowcoreBlog.Universal\PublicApi\snowcoreBlog.PublicApi.csproj',
    'Shared\IAM.Core\snowcoreBlog.Backend.IAM.Core.csproj',
    'Shared\Email.Core\snowcoreBlog.Backend.Email.Core.csproj',
    'Shared\AspireYarpGateway.Core\snowcoreBlog.Backend.AspireYarpGateway.Core.csproj',
    'snowcoreBlog.Universal\ServiceDefaults\snowcoreBlog.ServiceDefaults.csproj',
    'snowcoreBlog.Universal\UniversalModules\ApplicationLaunch\snowcoreBlog.ApplicationLaunch.csproj',
    'snowcoreBlog.Universal\UniversalModules\ApplicationLaunch.Implementations\snowcoreBlog.ApplicationLaunch.Implementations.csproj',
    'Shared\Infrastructure\snowcoreBlog.Backend.Infrastructure.csproj'
)

Push-Location $repo
$failed = 0
foreach ($p in $projects) {
    Write-Host ">>> pack $p"
    dotnet pack $p -c $Configuration -p:Version=$Version -o $feed --nologo
    if ($LASTEXITCODE -ne 0) { $failed++; Write-Host "FAILED: $p" -ForegroundColor Red }
}
Pop-Location

if ($failed -gt 0) {
    Write-Host "$failed project(s) failed to pack." -ForegroundColor Red
    exit 1
}
Write-Host "All $($projects.Count) packed (v$Version) -> $feed" -ForegroundColor Green
