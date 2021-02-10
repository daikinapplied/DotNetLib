param(
	[string]$apiKey="", 
	[string]$certIdentifier="",
	[string]$certIdType="subject",
	[string]$certStore="LocalMachine",
	[string]$nugetServer="https://daikinapplied.pkgs.visualstudio.com/_packaging/IT/nuget/v3/index.json",
	[string]$timeServer="http://timestamp.globalsign.com/?signature=sha2"
)

# ~~~[Introduce]~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Write-Host "~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~"
Write-Host "DotNetLib NuGet All Tool                                "
Write-Host "Developed by Daikin Applied                             "
Write-Host "~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~"

Write-Host "This tool calls Packaging and Publishing tools."
Write-Host ""

# ~~~[Main Body]~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

& .\NuGetPackage.ps1
& .\NuGetPublish $apiKey $certIdentifier $certIdType $certStore $nugetServer $timeServer

exit 0
# ~End~
