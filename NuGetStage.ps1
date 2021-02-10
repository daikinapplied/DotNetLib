param([string]$buildArtifactStagingDirectory)

# ~~~[Introduce]~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Write-Host "                                                        "
Write-Host "~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~"
Write-Host "DotNetLib Azure Pipelines Artifacts Staging Tool        "
Write-Host "Developed by Daikin Applied                             "
Write-Host "~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~"
Write-Host "                                                        "
Write-Host "? Move NuGet Packages from Stage to Artifacts Locations."
Write-Host "                                                        "

# ~~~[Globals]~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

$startupFolder = Get-Location
$scriptFolder = $PSScriptRoot

# ~~~[Main Body]~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Write-Host "Script Startup Folder: $startupFolder"
Write-Host "This Script Folder: $scriptFolder"
Write-Host "Artifacts Staging Folder: $buildArtifactStagingDirectory"

if ($buildArtifactStagingDirectory -ne 0)
{
	robocopy ".\" "$buildArtifactStagingDirectory" NuGet*.ps1 /NP
	robocopy ".\" "$buildArtifactStagingDirectory" Daikin*.nupkg /S /NP
}
else
{
	Write-Host ":( No parameters specified"
	exit 1;
}

exit 0
# ~End~
