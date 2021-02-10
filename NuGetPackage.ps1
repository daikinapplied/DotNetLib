# ~~~[Introduce]~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Write-Host "                                                        "
Write-Host "~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~"
Write-Host "DotNetLib NuGet Package Tool                            "
Write-Host "Developed by Daikin Applied                             "
Write-Host "~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~"
Write-Host "                                                        "
Write-Host "? Create .NET Framework NuGet packages.                 "
Write-Host "                                                        "
Write-Host

# ~~~[Globals]~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

$prodLocations = @("prd","Release")

# ~~~[Functions]~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Function BuildNuGet
{
	param (
		[string]$project,
		[string]$rootFolder
	)

	Write-Host "~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~"
	Set-Location "$rootFolder\$project"
	Write-Host "~~ Build: $project ~~"
	Write-Host

	$prodBits = ""
	foreach ($prodBits in $prodLocations)
	{
		try
		{
			$searchFile = "bin\$prodBits\$project.dll"
			Write-Host "Searching: $searchFile"
			$dllFile = Get-Item $searchFile
			if ($dllFile)
			{
				Write-Host "Found: $($dllFile.FullName)"
				$versionInfo = $dllFile.VersionInfo
				break
			}
		}
		catch [System.Exception]
		{
			$versionInfo = $null
		}
	}

	if ($versionInfo)
	{
		$version = $versionInfo.FileMajorPart.ToString() + "." + $versionInfo.FileMinorPart.ToString() + "." + $versionInfo.FileBuildPart.ToString()
		if ( $versionInfo.FilePrivatePart.ToString() -ne "0" ) 
		{ 
		   $version = $version + "." + $versionInfo.FilePrivatePart.ToString()
		}
	
		nuget pack $project.nuspec -Version $version -Prop Configuration=Release
		Move-Item "$project.$version.nupkg" "bin\$prodBits\" -Force
		Set-Location "$rootFolder"
		Write-Host
	}
	else
	{
		Set-Location "$rootFolder"
		Write-Host ":( Unable to find a package to deploy"
		exit 1
	}
}

# ~~~[Main Body]~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

# ~~ .NET Framework NuGet packaging (.NET Standard and .NET Core projects can build this automatically in Visual Studio 2017/2019) ~~
BuildNuGet "Daikin.DotNetLib.DotNetNuke" $PSScriptRoot
BuildNuGet "Daikin.DotNetLib.Windows" $PSScriptRoot
BuildNuGet "Daikin.DotNetLib.Serial" $PSScriptRoot

exit 0
# ~End~
